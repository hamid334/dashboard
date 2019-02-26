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
using static BasketWebPanel.Utility;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    public class PostsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            AddNewPostViewModel model = new AddNewPostViewModel();
            model.SetSharedData(User);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AddNewPostViewModel model)
        {
            try
            {
                MultipartFormDataContent content;

                bool FileAttached = (Request.RequestContext.HttpContext.Session["AddPostImage"] != null);


                byte[] fileData = null;
                var ImageFile = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddPostImage"];
                if (FileAttached)
                {
                    using (var binaryReader = new BinaryReader(ImageFile.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(ImageFile.ContentLength);
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Please Choose an image to upload.");
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

                response = await ApiCall.CallApi("api/Admin/AddPost", User, isMultipart: true, multipartContent: content);
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
                    TempData["SuccessMessage"] = "The post has been published successfully.";

                    return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex), "Internal Server Error");
            }
        }


        [HttpGet]
        public async Task<ActionResult> ManagePosts()
        {
            try
            {
                var response = await ApiCall.CallApi("api/Admin/GetPosts", User, GetRequest: true);

                if (response is Error)
                {
                    return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
                }

                var model = response.GetValue("Result").ToObject<SearchPostsViewModel>();

                model.SetSharedData(User);

                return View(model);

            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex), "Internal Server Error");
            }
        }

        public JsonResult DeletePost(int PostId)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/DeleteEntity", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.Post, "Id=" + PostId));
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

        [HttpPost]
        public JsonResult UploadImage(HttpPostedFileBase file)
        {
            if (Request.Files.Count == 1)
            {
                Request.RequestContext.HttpContext.Session.Remove("AddPostImage");
                Request.RequestContext.HttpContext.Session.Add("AddPostImage", Request.Files[0]);

                Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
                Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", false);
            }
            return Json("Success");
        }


        [HttpPost]
        public JsonResult DeleteImage()
        {
            Request.RequestContext.HttpContext.Session.Remove("AddPostImage");
            Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", true);
            return Json("Session Cleared");
        }
    }
}
