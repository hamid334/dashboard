using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.Areas.Dashboard.Models
{
    public class AddNotificationsViewModel : BaseViewModel
    {
        public AddNotificationsViewModel()
        {
            TargetOptions = new SelectList(new List<SelectListItem>());
            Notification = new NotificationBindingModel();
        }

        public SelectList TargetOptions { get; set; }

        public NotificationBindingModel Notification;

    }
}