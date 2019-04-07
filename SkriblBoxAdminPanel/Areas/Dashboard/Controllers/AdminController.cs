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
    public class AdminController : Controller
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
                Request.RequestContext.HttpContext.Session.Remove("AddAdminImage");
                Request.RequestContext.HttpContext.Session.Add("AddAdminImage", Request.Files[0]);

                Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
                Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", false);
            }
            return Json("Success");
        }

        [HttpPost]
        public JsonResult DeleteImage()
        {
            Request.RequestContext.HttpContext.Session.Remove("AddAdminImage");
            Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", true);
            return Json("Session Cleared");
        }

        public ActionResult Index(int? AdminId)
        {
            Request.RequestContext.HttpContext.Session.Remove("AddAdminImage");
            Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
            AddAdminViewModel model = new AddAdminViewModel();

            model.StoreOptions = Utility.GetStoresOptions(User);
            model.SetSharedData(User);

            if (model.Role == RoleTypes.Agent)
            {
                AdminId = model.Id;
            }

            if (AdminId.HasValue)
            {
                var responseAdmin = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetEntityById", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.Admin, "Id=" + AdminId.Value));
                if (responseAdmin == null || responseAdmin is Error)
                { }
                else
                {
                    model.Admin = responseAdmin.GetValue("Result").ToObject<AdminViewModel>();
                }
            }


            model.RoleOptions = new SelectList(
                new List<SelectListItem> {
                    new SelectListItem { Text = Utility.RoleTypes.Agent.ToString(), Value = Utility.RoleTypes.Agent.ToString("D") }
                    //new SelectListItem { Text = Utility.RoleTypes.SuperAdmin.ToString(), Value = Utility.RoleTypes.SuperAdmin.ToString("D") }
                });


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AddAdminViewModel model)
        {
            model.SetSharedData(User);

            if (model.Admin.Id == 0)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
            }


            MultipartFormDataContent content;

            bool FileAttached = (Request.RequestContext.HttpContext.Session["AddAdminImage"] != null);
            bool ImageDeletedOnEdit = false;
            var imgDeleteSessionValue = Request.RequestContext.HttpContext.Session["ImageDeletedOnEdit"];
            if (imgDeleteSessionValue != null)
            {
                ImageDeletedOnEdit = Convert.ToBoolean(imgDeleteSessionValue);
            }
            byte[] fileData = null;
            var ImageFile = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddAdminImage"];
            if (FileAttached)
            {
                using (var binaryReader = new BinaryReader(ImageFile.InputStream))
                {

                    fileData = binaryReader.ReadBytes(ImageFile.ContentLength);
                }
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

            if (model.Admin.Id > 0)
                content.Add(new StringContent(model.Admin.Id.ToString()), "Id");

            if (model.Admin.Store_Id.HasValue)
                content.Add(new StringContent(model.Admin.Store_Id.Value.ToString()), "Store_Id");

            content.Add(new StringContent(model.Admin.FirstName), "FirstName");
            content.Add(new StringContent(model.Admin.LastName), "LastName");
            content.Add(new StringContent(model.Admin.Email), "Email");
            content.Add(new StringContent(model.Admin.Phone), "Phone");

            content.Add(new StringContent(model.Admin.Role.ToString()), "Role");


            if (model.Admin.Id == 0)
                content.Add(new StringContent(model.Admin.Password), "Password");

            content.Add(new StringContent(Convert.ToString(ImageDeletedOnEdit)), "ImageDeletedOnEdit");
            response = await ApiCall.CallApi("api/Admin/AddAdmin", User, isMultipart: true, multipartContent: content);

            if (firstCall && response.ToString().Contains("UnAuthorized"))
            {
                firstCall = false;
                goto callAgain;
            }
            else if (response.ToString().Contains("UnAuthorized"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "UnAuthorized Error");
            }

            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                model.Admin = response.GetValue("Result").ToObject<AdminViewModel>();
                var claimIdentity = ((ClaimsIdentity)User.Identity);
                if (model.Admin.Id == Convert.ToInt32(claimIdentity.Claims.FirstOrDefault(x => x.Type == "AdminId").Value))
                {
                    User.AddUpdateClaim("FullName", model.Admin.FirstName + " " + model.Admin.LastName);
                    User.AddUpdateClaim("ProfilePictureUrl", model.Admin.ImageUrl);
                }
                model.SetSharedData(User);

                return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Index");
            }
        }

        public ActionResult ManageAdmins()
        {
            Global.sharedDataModel.SetSharedData(User);
            return View(Global.sharedDataModel);
        }

        public ActionResult SearchAdmin()
        {
            SearchAdminModel returnModel = new SearchAdminModel();

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
            return PartialView("_SearchAdmin", returnModel);
        }

        public ActionResult SearchAdminResults(SearchAdminModel model)
        {
            SearchAdminsViewModel returnModel = new SearchAdminsViewModel();
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/SearchAdmins", User, null, true, false, null, "FirstName=" + model.FirstName + "", "LastName=" + model.LastName, "Email=" + model.Email, "Phone=" + model.Phone, "StoreId=" + model.StoreId));
            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                returnModel = response.GetValue("Result").ToObject<SearchAdminsViewModel>();
            }
            foreach (var admin in returnModel.Admins)
            {
                admin.RoleName = ((RoleTypes)admin.Role).ToString();
            }
            returnModel.SetSharedData(User);
            return PartialView("_SearchAdminResults", returnModel);
        }


        public ActionResult UserCard(int? UserID)

        {
            var model = new RequestCardModel();

            if (UserID.HasValue)
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/User/GetBulkCardDetails", User, null, true, false, null,"User_Id=" + UserID.Value));


                if (response is Error || response == null) 
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Internal Server Error");
                else
                    model.RequestCard = response.GetValue("Result").ToObject<List<RequestCardModel>>();
                //model.User.FullName = model.User.FirstName + " " + model.User.LastName;
               
                // model.RequestCard= (RequestCardModel)response.GetValue("Result").ToObject<>();



                // model.SetSharedData(User);

            }
            return View(model);
        }


        public JsonResult DeleteAdmin(int AdminId)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/DeleteEntity", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.Admin, "Id=" + AdminId));
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

        public ActionResult ManageSubAdmin()
        {
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetAgents", User, null, true, false, null));

            var model = new AdminSearchResultViewModel();

            if (response is Error)
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
            else
                model = response.GetValue("Result").ToObject<AdminSearchResultViewModel>();

            foreach (var user in model.Agents)
            {
                user.StatusName = user.IsDeleted ? "Blocked" : "Active";
                if (user.ImageUrl == null || user.ImageUrl == "")
                    user.ImageUrl = "UserImages/Default.png";
            }
            model.StatusOptions = Utility.GetUserStatusOptions();

            model.SetSharedData(User);

            return View(model);
        }

        public ActionResult ManageMerchant()
        {
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetMerchants", User, null, true, false, null));

            SearchUserViewModel model = new SearchUserViewModel();

            if (response is Error)
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
            else
                model = response.GetValue("Result").ToObject<SearchUserViewModel>();

            foreach (var user in model.Users)
            {
                user.StatusName = user.IsDeleted ? "Blocked" : "Active";
                if (user.ProfilePictureUrl == null || user.ProfilePictureUrl == "")
                    user.ProfilePictureUrl = "UserImages/Default.png";
            }
            model.StatusOptions = Utility.GetUserStatusOptions();

            model.SetSharedData(User);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAgentStatuses(List<ChangeUserStatusModel> selectedUsers)
        {
            try
            {
                if (selectedUsers == null)
                {
                    return new HttpStatusCodeResult((int)HttpStatusCode.Forbidden, "Select a user to save");
                }

                ChangeUserStatusListModel postModel = new ChangeUserStatusListModel();
                postModel.Users = selectedUsers;

                var apiResponse = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/ChangeAdminStatuses", User, postModel));

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
    }
}