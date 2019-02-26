using BasketWebPanel.Areas.Dashboard.Models;
using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static BasketWebPanel.Utility;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        [HttpPost]
        public JsonResult DeleteImageOnEdit()
        {
            return Json("Success");
        }

        [HttpPost]
        public JsonResult UploadImage(HttpPostedFileBase file)
        {
            if (Request.Files.Count == 1)
            {
                Request.RequestContext.HttpContext.Session.Remove("AddProductImage");
                Request.RequestContext.HttpContext.Session.Add("AddProductImage", Request.Files[0]);

                Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
                Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", false);
            }
            return Json("Success");
        }

        [HttpPost]
        public JsonResult DeleteImage()
        {
            Request.RequestContext.HttpContext.Session.Remove("AddProductImage");
            Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", true);
            return Json("Session Cleared");
        }

        public JsonResult FetchCategories(int storeId) // its a GET, not a POST
        {
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetAllCategoriesByStoreId", User, GetRequest: true, parameters: "StoreId=" + storeId));
            var responseCategories = response.GetValue("Result").ToObject<List<CategoryBindingModel>>();
            var tempCats = responseCategories.ToList();

            foreach (var cat in responseCategories)
            {
                cat.Name = cat.GetFormattedBreadCrumb(tempCats);
            }

            responseCategories.Insert(0, new CategoryBindingModel { Id = 0, Name = "None" });

            return Json(responseCategories, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(int? ProductId)
        {
            Request.RequestContext.HttpContext.Session.Remove("AddProductImage");
            Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
            AddProductViewModel model = new AddProductViewModel();

            model.SetSharedData(User);

            //Providing StoresList

            if (ProductId.HasValue)
            {
                var responseProduct = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetEntityById", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.Product, "Id=" + ProductId.Value));
                if (responseProduct == null || responseProduct is Error)
                { }
                else
                {
                    model.Product = responseProduct.GetValue("Result").ToObject<ProductBindingModel>();
                    model.CategoryOptions = Utility.GetCategoryOptions(User, "", model.Product.Category_Id);
                    model.StoreOptions = Utility.GetStoresOptions(User, "", model.Product.Store_Id);
                }
            }
            else
            {
                //Providing CategoryList
                model.CategoryOptions = Utility.GetCategoryOptions(User, "None");
                model.StoreOptions = Utility.GetStoresOptions(User, "None");
            }


            model.WeightOptions = Utility.GetWeightOptions();


            //return PartialView("_AddProduct", model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AddProductViewModel model)
        {
            try
            {
                model.Product.Description = model.Product.Description ?? "";
                if (!ModelState.IsValid)
                {
                    return View(model);
                }


                MultipartFormDataContent content = new MultipartFormDataContent();
                bool FileAttached = (Request.RequestContext.HttpContext.Session["AddProductImage"] != null);
                bool ImageDeletedOnEdit = false;
                var imgDeleteSessionValue = Request.RequestContext.HttpContext.Session["ImageDeletedOnEdit"];
                if (imgDeleteSessionValue != null)
                {
                    ImageDeletedOnEdit = Convert.ToBoolean(imgDeleteSessionValue);
                }
                byte[] fileData = null;
                var ImageFile = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddProductImage"];
                if (FileAttached)
                {
                    using (var binaryReader = new BinaryReader(ImageFile.InputStream))
                    {

                        fileData = binaryReader.ReadBytes(ImageFile.ContentLength);
                    }
                }
                else if (model.Product.ImageUrl == null || ImageDeletedOnEdit)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Please Choose an image to upload.");
                }

                ByteArrayContent fileContent;

                if (FileAttached)
                {
                    fileContent = new ByteArrayContent(fileData);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = ImageFile.FileName };
                    content.Add(fileContent);
                }
                if (model.Product.Id > 0)
                {
                    content.Add(new StringContent(model.Product.Id.ToString()), "Id");
                }

                content.Add(new StringContent(model.Product.Name), "Name");
                content.Add(new StringContent(model.Product.Price.ToString()), "Price");
                content.Add(new StringContent(model.Product.Category_Id.ToString()), "Category_Id");
                content.Add(new StringContent(model.StoreId.ToString()), "Store_Id");
                content.Add(new StringContent(model.Product.Description), "Description");
                content.Add(new StringContent(Convert.ToString(model.Product.DiscountPrice)), "DiscountPrice");
                content.Add(new StringContent(Convert.ToString(model.Product.DiscountPercentage)), "DiscountPercentage");


                content.Add(new StringContent(Convert.ToString(ImageDeletedOnEdit)), "ImageDeletedOnEdit");
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/AddProduct", User, isMultipart: true, multipartContent: content));

                if (response.ToString().Contains("UnAuthorized"))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "UnAuthorized Error");
                }
                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.MethodNotAllowed, (response as Error).ErrorMessage);
                }
                else
                {
                    if (model.Product.Id > 0)
                        TempData["SuccessMessage"] = "The product has been updated successfully.";
                    else
                        TempData["SuccessMessage"] = "The product has been added successfully.";

                    return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult ManageProducts()
        {
            Global.sharedDataModel.SetSharedData(User);
            return View(Global.sharedDataModel);
        }

        public ActionResult SearchProduct()
        {
            SearchProductModel returnModel = new SearchProductModel();

            var responseStores = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetAllStores", User, GetRequest: true));
            if (responseStores == null || responseStores is Error)
            {
            }
            else
            {
                var Stores = responseStores.GetValue("Result").ToObject<List<StoreBindingModel>>();
                IEnumerable<SelectListItem> selectList = from store in Stores
                                                         select new SelectListItem
                                                         {
                                                             Selected = false,
                                                             Text = store.Name,
                                                             Value = store.Id.ToString()
                                                         };
                Stores.Insert(0, new StoreBindingModel { Id = 0, Name = "All" });

                returnModel.StoreOptions = new SelectList(selectList);
            }

            returnModel.SetSharedData(User);
            //returnModel. returnModel.StoreId
            if (returnModel.Role == RoleTypes.Agent)
            {
                returnModel.StoreId = (returnModel as BaseViewModel).StoreId;

            }



            return PartialView("_SearchProduct", returnModel);
        }

        public ActionResult SearchProductResults(SearchProductModel model)
        {
            SearchProductsViewModel returnModel = new SearchProductsViewModel();
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetAllOffers", User, null, true));
            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                returnModel = response.GetValue("Result").ToObject<SearchProductsViewModel>();
            }
            returnModel.SetSharedData(User);
            return PartialView("_SearchProductResults", returnModel);
        }

        public JsonResult DeleteProduct(int ProductId)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/DeleteEntity", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.Product, "Id=" + ProductId));
                if (response is Error)
                    return Json("An error has occurred, error code : 500", JsonRequestBehavior.AllowGet);
                else
                    return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult ManageRequests()
        {
            Global.sharedDataModel.SetSharedData(User);
            return View(Global.sharedDataModel);
        }

        public JsonResult FetchStores(int CategoryId) // its a GET, not a POST
        {
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetStoreByCategoryIdForAdmin", User, GetRequest: true, parameters: "Category_Id=" + CategoryId));
            var responseCategories = response.GetValue("Result").ToObject<List<StoreViewModel>>();
            var tempCats = responseCategories.ToList();

            //foreach (var cat in responseCategories)
            //{
            //    cat.Name = cat.GetFormattedBreadCrumb(tempCats);
            //}

            responseCategories.Insert(0, new StoreViewModel { Id = 0, Name = "None" });

            return Json(responseCategories, JsonRequestBehavior.AllowGet);
        }


    }
}