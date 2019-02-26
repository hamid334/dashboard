using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    [Authorize]
    public class SubscriptionController : Controller
    {
        // GET: Dashboard/Subscription
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManageSubscriptions()
        {
            Global.sharedDataModel.SetSharedData(User);
            return View(Global.sharedDataModel);
        }
        public ActionResult SearchSubscriptionResult(SearchSubscriptionModel model)
        {
            SubscriptionListViewModel returnModel = new SubscriptionListViewModel();
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Subscriptions/SearchSubscriptions", User, null, true, false, null, "SubscriptionId=" + model.SubscriptionId, "BoxId=" + model.BoxId+ "&UserId="+model.UserId));
            if (response == null || response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                returnModel = response.GetValue("Result").ToObject<SubscriptionListViewModel>();
           
            }

            returnModel.SetSharedData(User);

            if (returnModel.is_detail)
            {
                return View("Subscription", returnModel);
            }
            

            return PartialView("_SearchSubscriptionResult", returnModel);
        }
    }
}