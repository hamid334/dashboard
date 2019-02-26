using BasketWebPanel.Areas.Dashboard.Models;
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
    public class NotificationsController : Controller
    {
        // GET: Dashboard/Notifications
        public ActionResult Index(int? NotificationId)
        {
            AddNotificationsViewModel model = new AddNotificationsViewModel();

            model.SetSharedData(User);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(NotificationBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                model.TargetAudience = 2; //Users Only

                var apiResponse = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/AddNotification", User, model));

                if (apiResponse == null || apiResponse is Error)
                    return new HttpStatusCodeResult(500, "Internal Server Error");
                else
                {
                    TempData["SuccessMessage"] = "The notification has been sent successfully.";
                    model.SetSharedData(User);
                    return Json("Success");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex));
            }
        }

        public ActionResult ManageNotifications()
        {
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/SearchNotifications", User, null, true, false, null));

            SearchNotificationsViewModel model = new SearchNotificationsViewModel();

            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
            else
            {
                model = response.GetValue("Result").ToObject<SearchNotificationsViewModel>();
            }

            foreach (var notification in model.Notifications)
            {
                switch (notification.TargetAudienceType)
                {
                    case 1:
                        notification.TargetAudience = "Users & Deliverers";
                        break;
                    case 2:
                        notification.TargetAudience = "Users Only";
                        break;
                    case 3:
                        notification.TargetAudience = "Deliverers Only";
                        break;
                }
            }

            model.SetSharedData(User);

            return View(model);
        }
    }
}