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

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    [Authorize]
    public class VideosController : Controller
    {
        // GET: Dashboard/Videos
        public async Task<ActionResult> Index()
        {
            var response = await ApiCall.CallApi("api/Videos/GetHowItWorksVideoUrl", User, null, true);
            AddHowItWorksViewModel model = response.GetValue("Result").ToObject<AddHowItWorksViewModel>();

            model.SetSharedData(User);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(AddHowItWorksViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var response = await ApiCall.CallApi("api/Videos/AddHowItWorks", User, model);

                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
                }
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
        
        public async Task<ActionResult> UploadVideo(HttpPostedFileBase file)
        {
            if (Request.Files.Count == 1)
            {
                byte[] fileData = null;
                MultipartFormDataContent content;
                ByteArrayContent fileContent;
                JObject response;

                bool firstCall = true;
                callAgain: content = new MultipartFormDataContent();

                using (var binaryReader = new BinaryReader(Request.Files[0].InputStream))
                {
                    fileData = binaryReader.ReadBytes(Request.Files[0].ContentLength);
                }

                fileContent = new ByteArrayContent(fileData);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = Request.Files[0].FileName };
                content.Add(fileContent);

                response = await ApiCall.CallApi("api/Videos/AddHowItWorksVideo", User, isMultipart: true, multipartContent: content);

                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
                }
                else
                {
                    return Json("Success");
                    //return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError,"Internal Server Error");

        }

        public ActionResult DeleteVideo()
        {
            return Json("Success");
        }
    }
}