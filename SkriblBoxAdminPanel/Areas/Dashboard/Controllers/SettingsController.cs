using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using Newtonsoft.Json;
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
using static BasketWebPanel.Utility;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        // GET: Dashboard/Settings
        public ActionResult Index()
        {
            Request.RequestContext.HttpContext.Session.Remove("AddBannerImage");
            Request.RequestContext.HttpContext.Session.Remove("AddInstagramImage");
            SettingsViewModel returnModel = new SettingsViewModel();
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Settings/GetSettings", User, GetRequest: true));

            if (response == null || response is Error)
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            else
                returnModel = response.GetValue("Result").ToObject<SettingsViewModel>();

            returnModel.SetSharedData(User);
            return View("Index", returnModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(SettingsViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool firstCall = true;

            MultipartFormDataContent content;
            bool BannerImageAttached = (Request.RequestContext.HttpContext.Session["AddBannerImage"] != null);
            bool InstaImageAttached = (Request.RequestContext.HttpContext.Session["AddInstagramImage"] != null);
            ByteArrayContent fileContent;
            JObject response;
            callAgain: content = new MultipartFormDataContent();

            if (BannerImageAttached)
            {
                bool ImageDeletedOnEdit = false;
                var imgDeleteSessionValue = Request.RequestContext.HttpContext.Session["ImageDeletedOnEdit"];
                if (imgDeleteSessionValue != null)
                {
                    ImageDeletedOnEdit = Convert.ToBoolean(imgDeleteSessionValue);
                }
                byte[] fileDataBannerImage = null;
                byte[] fileDataInstagramImage = null;

                var ImageFileBanner = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddBannerImage"];
                var ImageFileInstagram = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddInstagramImage"];

                if (BannerImageAttached)
                    using (var binaryReader = new BinaryReader(ImageFileBanner.InputStream))
                    {
                        fileDataBannerImage = binaryReader.ReadBytes(ImageFileBanner.ContentLength);
                    }

                if (InstaImageAttached)
                    using (var binaryReader = new BinaryReader(ImageFileInstagram.InputStream))
                    {
                        fileDataInstagramImage = binaryReader.ReadBytes(ImageFileInstagram.ContentLength);
                    }

                if (BannerImageAttached)
                {
                    fileContent = new ByteArrayContent(fileDataBannerImage);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "Banner" + Path.GetExtension(ImageFileBanner.FileName) };
                    content.Add(fileContent, "Banner");
                }

                if (InstaImageAttached)
                {
                    fileContent = new ByteArrayContent(fileDataInstagramImage);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "Instagram" + Path.GetExtension(ImageFileInstagram.FileName) };
                    content.Add(fileContent, "Instagram");
                }

            }

            content.Add(new StringContent(model.Id.ToString()), "Id");
            content.Add(new StringContent(model.Currency), "Currency");
            content.Add(new StringContent(Convert.ToString(model.DeliveryFee)), "DeliveryFee");

            content.Add(new StringContent(Convert.ToString(model.FreeDeliveryThreshold)), "FreeDeliveryThreshold");

            response = await ApiCall.CallApi("api/Settings/SetSettings", User, isMultipart: true, multipartContent: content);
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
                Request.RequestContext.HttpContext.Session.Remove("AddBannerImage");
                TempData["SuccessMessage"] = "Settings updated successfully.";
                return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult GeneralSettings()
        {
            Global.sharedDataModel.SetSharedData(User);
            return View(Global.sharedDataModel);
        }

        [HttpPost]
        public JsonResult UploadImage(HttpPostedFileBase file, int Type)
        {
            if (Request.Files.Count > 0)
            {
                if (Type == 1)
                {
                    Request.RequestContext.HttpContext.Session.Remove("AddBannerImage");
                    Request.RequestContext.HttpContext.Session.Add("AddBannerImage", Request.Files[0]);
                }
                else if (Type == 2)
                {
                    Request.RequestContext.HttpContext.Session.Remove("AddInstagramImage");
                    Request.RequestContext.HttpContext.Session.Add("AddInstagramImage", Request.Files[0]);
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult DeleteImage(int Type)
        {
            if (Type == 1)
            {
                Request.RequestContext.HttpContext.Session.Remove("AddBannerImage");
                Request.RequestContext.HttpContext.Session.Add("AddBannerImage", true);
            }
            else if (Type == 2)
            {
                Request.RequestContext.HttpContext.Session.Remove("AddInstagramImage");
                Request.RequestContext.HttpContext.Session.Add("AddInstagramImage", true);
            }
            return Json("Session Cleared", JsonRequestBehavior.AllowGet);
        }
    }
}