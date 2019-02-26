using BasketWebPanel.BindingModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    public class CardRequestController : Controller
    {
        [Authorize]
        public ActionResult ManageCardRequest()
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/User/GetCardRequest", User, null, true));

                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
                }

                var boxes = response.GetValue("Result").ToObject<RequestCardBindingModel>();
                boxes.SetSharedData(User);
                return View(boxes);
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }
    }
}