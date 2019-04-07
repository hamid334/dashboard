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
    public class UsersController : Controller
    {
        // GET: Dashboard/Users
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManageUsers()
        {
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetUsers", User, null, true, false, null));
          
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

                user.IsCard = user.SignInType == 0 ? true : false;
            }
            model.StatusOptions = Utility.GetUserStatusOptions();

            model.SetSharedData(User);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveUserStatuses(List<ChangeUserStatusModel> selectedUsers)
        {
            try
            {
                if (selectedUsers == null)
                {
                    return new HttpStatusCodeResult((int)HttpStatusCode.Forbidden, "Select a user to save");
                }

                ChangeUserStatusListModel postModel = new ChangeUserStatusListModel();
                postModel.Users = selectedUsers;

                var apiResponse = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/ChangeUserStatuses", User, postModel));

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
        public ActionResult MyCard(int? UserID)

        {
            UserBindingModel users = null;
            var model = new RequestCardModel();
            var res = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetUser", User, null, true, false, null, "UserId=" + UserID, "SignInType=0"));


            if (res is Error)
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
            else
                users = res.GetValue("Result").ToObject<UserBindingModel>();
            if (users.Email != User.Identity.Name)
            {
                return View(model);
            }
            else
            { }
            //foreach (var item in users)
            //{
            //    if (item.Email == User.Identity.Name) 
            //    {
            //        UserID = item.Id;
            //    }
            //}
            if (UserID.HasValue)
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/User/GetBulkCardDetails", User, null, true, false, null, "User_Id=" + UserID.Value));


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
        public ActionResult GetUser(int UserId)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetUser", User, null, true, false, null, "UserId=" + UserId, "SignInType=0"));

                UserBindingModel model = null;

                if (response is Error)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
                else
                    model = response.GetValue("Result").ToObject<UserBindingModel>();

                if (model.ProfilePictureUrl == null || model.ProfilePictureUrl == "")
                    model.ProfilePictureUrl = "UserImages/Default.png";
                foreach (var subscription in model.UserSubscriptions)
                {
                    subscription.Box.CategoryName = Utility.GetBoxCategoryName(subscription.Box.BoxCategory_Id);

                }

                foreach (var order in model.Orders)
                {
                    order.PaymentMethodName = Utility.GetPaymentMethodName(order.PaymentMethod);
                    order.PaymentStatusName = Utility.GetPaymentStatusName(order.PaymentStatus);
                }


                model.UserAddresses = model.UserAddresses.Where(x => x.IsDeleted == false).ToList();
                model.PaymentCards = model.PaymentCards.Where(x => x.IsDeleted == false).ToList();
                model.SetSharedData(User);

                return View("User", model);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex), "Internal Server Error");
            }
        }
    }
}