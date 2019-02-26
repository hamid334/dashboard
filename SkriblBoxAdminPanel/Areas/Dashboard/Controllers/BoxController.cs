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
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static BasketWebPanel.Utility;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    [Authorize]
    public class BoxController : Controller
    {
        // GET: Dashboard/Box
        public ActionResult Index(int? BoxId)
        {
            try
            {
                AddBoxViewModel model = new AddBoxViewModel();
                //model.BoxVideos.Add(new Video { VideoUrl = "" });
                model.ReleaseDate = DateTime.Now;

                if (BoxId > 0)
                {
                    var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetEntityById", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.Box, "Id=" + BoxId.Value));
                    model = response.GetValue("Result").ToObject<AddBoxViewModel>();
                }
                else
                {
                    model.Id = 0;
                }

                List<SelectListItem> catOptions = new List<SelectListItem>();


                model.Categories = Utility.GetCategoryOptions(User);


                model.SetSharedData(User);
                return View(model);
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        [HttpPost]
        public JsonResult UploadImage(HttpPostedFileBase file)
        {
            if (Request.Files.Count == 1)
            {
                Request.RequestContext.HttpContext.Session.Remove("AddMarchantImage");
                Request.RequestContext.HttpContext.Session.Add("AddMarchantImage", Request.Files[0]);

                Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
                Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", false);
                return Json("Success");
            }
            return Json("Fail");
        }

        [HttpPost]
        public JsonResult DeleteImage()
        {
            Request.RequestContext.HttpContext.Session.Remove("AddMarchantImage");
            Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", true);
            return Json("Session Cleared");
        }

        [HttpPost]
        public async Task<ActionResult> Index(AddBoxViewModel model)
        {
            //if (model.BoxVideos == null || model.BoxVideos.Count == 0 || model.BoxVideos.First().VideoUrl == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Please add at least one video url.");
            //}

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //model.BoxVideos.RemoveAll(x => String.IsNullOrEmpty(x.VideoUrl));


            MultipartFormDataContent content;
            bool FileAttached = (Request.RequestContext.HttpContext.Session["AddMerchantImage"] != null);
            bool ImageDeletedOnEdit = false;
            var imgDeleteSessionValue = Request.RequestContext.HttpContext.Session["ImageDeletedOnEdit"];
            if (imgDeleteSessionValue != null)
            {
                ImageDeletedOnEdit = Convert.ToBoolean(imgDeleteSessionValue);
            }
            byte[] fileData = null;
            var ImageFile = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddMarchantImage"];
            if (FileAttached)
                using (var binaryReader = new BinaryReader(ImageFile.InputStream))
                {
                    fileData = binaryReader.ReadBytes(ImageFile.ContentLength);
                }

            ByteArrayContent fileContent;
            JObject response;

            bool firstCall = true;
            callAgain: content = new MultipartFormDataContent();
            if (FileAttached)
            {
                fileContent = new ByteArrayContent(fileData);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = ImageFile.FileName };
                content.Add(fileContent);
            }

            if (model.Id > 0)
            {
                var boxResponse = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/AddBox", User, model));

                if (boxResponse is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (boxResponse as Error).ErrorMessage);
                }

                if (model.Id > 0)
                    TempData["SuccessMessage"] = "The box has been updated successfully.";
                else
                    TempData["SuccessMessage"] = "The box has been added successfully.";

                return Json("Success");
            }
            else
            {
                content.Add(new StringContent(model.Name), "Title");
                content.Add(new StringContent(Convert.ToString(model.Category_Id)), "CategoryId");
                content.Add(new StringContent(Convert.ToString(model.Description)), "Description");
                content.Add(new StringContent(Convert.ToString(model.Merchant.FirstName)), "FirstName");
                content.Add(new StringContent(Convert.ToString(model.Merchant.LastName)), "LastName");
                content.Add(new StringContent(Convert.ToString(model.Merchant.Email)), "Email");
                content.Add(new StringContent(Convert.ToString(model.Merchant.Phone)), "Phone");
                content.Add(new StringContent(Convert.ToString(model.Merchant.Country)), "Country");
                content.Add(new StringContent(Convert.ToString(model.Merchant.City)), "City");
                content.Add(new StringContent(Convert.ToString(model.Merchant.Area)), "Area");

                response = await ApiCall.CallApi("api/Admin/AddMerchant", User, isMultipart: true, multipartContent: content);
                if (firstCall && Convert.ToString(response).Contains("UnAuthorized"))
                {
                    firstCall = false;
                    goto callAgain;
                }
                else if (Convert.ToString(response).Contains("UnAuthorized"))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "UnAuthorized Error");
                }

                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
                }
                else
                {
                    model.SetSharedData(User);
                    if (model.Role == RoleTypes.SuperAdmin)
                    {
                        if (model.Id > 0)
                            TempData["SuccessMessage"] = "The Merchant has been updated successfully.";
                        else
                            TempData["SuccessMessage"] = "The Merchant has been added successfully.";
                    }

                    return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult ManageBoxes()
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetAllBoxes", User, null, true));

                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
                }

                var boxes = response.GetValue("Result").ToObject<SearchBoxesViewModel>();

                foreach (var box in boxes.Boxes)
                {
                    box.StatusName = Utility.GetStatusName(box.Status);
                    box.CategoryName = ((BoxCategoryOptions)box.BoxCategory_Id).ToString();
                }

                boxes.StatusOptions = Utility.GetBoxStatusOptions();
                boxes.SetSharedData(User);

                return View(boxes);
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveBoxStatuses(List<ChangeBoxStatusModel> selectedBoxes)
        {
            try
            {
                if (selectedBoxes == null)
                {
                    return new HttpStatusCodeResult((int)HttpStatusCode.Forbidden, "Select a box to save");
                }

                ChangeBoxStatusListModel postModel = new ChangeBoxStatusListModel();
                postModel.Boxes = selectedBoxes;

                var apiResponse = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/ChangeBoxStatuses", User, postModel));

                if (apiResponse == null || apiResponse is Error)
                    return new HttpStatusCodeResult(500, "Internal Server Error");
                else
                {
                    return Json("Success");
                }

            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex), "Internal Server Error");
            }
        }

        public JsonResult DeleteBox(int BoxId)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/DeleteEntity", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.Box, "Id=" + BoxId));
                if (response is Error)
                    return Json("An error has occurred, error code : 500", JsonRequestBehavior.AllowGet);
                else
                    return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}