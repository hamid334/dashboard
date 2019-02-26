
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class SubscriptionListViewModel : BaseViewModel
    {
        public SubscriptionListViewModel()
        {
            Subscriptions = new List<SubscriptionViewModel>();
        }
        public  List<SubscriptionViewModel> Subscriptions { get; set; }
        public  Boolean is_detail { get; set; }
    }

    public class SubscriptionViewModel
    {
      
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime SubscriptionDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int Box_Id { get; set; }

        public int Type { get; set; }

        public string Email { get; set; }
        
        public string ActivationCode { get; set; }

        public int Status { get; set; }

        public string Phone { get; set; }

        public string Name { get; set; }

        public int BoxCategory_Id { get; set; }

        public string BoxCategoryName { get; set; }

        public double Price { get; set; }

        public string FullName { get; set; }

        public string ProfilePictureUrl { get; set; }

 


    }

  
}