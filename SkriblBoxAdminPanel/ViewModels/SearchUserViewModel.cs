using BasketWebPanel.Areas.Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.ViewModels
{
    public class SearchUserViewModel : BaseViewModel
    {
        public SearchUserViewModel()
        {
            Users = new List<UserBindingModel>();
        }

        public List<UserBindingModel> Users { get; set; }

        public SelectList StatusOptions { get; internal set; }
    }

    public class UserBindingModel : BaseViewModel
    {
        public UserBindingModel()
        {
            UserAddresses = new List<UserAddressBindingModel>();
            PaymentCards = new List<UserPaymentCardBindingModel>();
            Feedback = new List<FeedbackViewModel>();
            UserSubscriptions = new List<UserSubscriptionViewModel>();
            Orders = new List<OrderViewModel>();
        }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string AccountType { get; set; }

        public string ZipCode { get; set; }

        public string DateofBirth { get; set; }

        public short? SignInType { get; set; }

        public string UserName { get; set; }

        public short? Status { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool PhoneConfirmed { get; set; }

        public bool IsDeleted { get; set; }

        public string StatusName { get; set; }

        public bool IsChecked { get; set; }

        public List<UserAddressBindingModel> UserAddresses;

        public List<UserPaymentCardBindingModel> PaymentCards { get; set; }

        public List<FeedbackViewModel> Feedback { get; set; } 

        public List<UserSubscriptionViewModel> UserSubscriptions { get; set; }

        public List<OrderViewModel> Orders { get; set; }

        public List<FavouriteViewModel> Favourites { get; set; }
        
    }

    public class UserPaymentCardBindingModel
    {
        public int Id { get; set; }

        public string CardNumber { get; set; }

        public DateTime ExpiryDate { get; set; }

        public string CCV { get; set; }

        public string NameOnCard { get; set; }

        public int CardType { get; set; } //1 for Credit, 2 for Debit

        public int User_ID { get; set; }

        public bool IsDeleted { get; set; }
    }

    public class UserAddressBindingModel
    {
        public int Id { get; set; }

        public int User_ID { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string StreetName { get; set; }

        public string Floor { get; set; }

        public string Apartment { get; set; }

        public string NearestLandmark { get; set; }

        public string BuildingName { get; set; }

        public short Type { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsPrimary { get; set; }
    }

    public class FeedbackViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int? UserId { get; set; }
        public UserViewModel User { get; set; }
        public int? Store_Id { get; set; }
        public StoreViewModel Store { get; set; }
    }

    public class FavouriteViewModel
    {
        public int Id { get; set; }

        public int Product_Id { get; set; }

        public int User_ID { get; set; }

        public bool IsDeleted { get; set; }

        public virtual SearchProductViewModel Product { get; set; }
    }

    public class UserSubscriptionViewModel
    {
        public int Id { get; set; }

        public int User_Id { get; set; }

        public int Box_Id { get; set; }

        public int Type { get; set; }

        public string ActivationCode { get; set; }

        public int Status { get; set; }

        public DateTime SubscriptionDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool IsDeleted { get; set; }

        public Box Box { get; set; }
    }

    //public class Favourites
    //{
    //    public int Id { get; set; }

    //    public int Product_Id { get; set; }

    //    public int User_ID { get; set; }

    //    public bool IsDeleted { get; set; }

    //    public ProductViewModel Product { get; set; }
    //}


    //public class ProductViewModel
    //{
    //    public int Id { get; set; }

    //    public string Name { get; set; }

    //    public double Price { get; set; }

    //    //[Required]
    //    public string Description { get; set; }


    //    public int WeightUnit { get; set; }

    //    public double WeightInGrams { get; set; }

    //    public double WeightInKiloGrams { get; set; }

    //    public string ImageUrl { get; set; }

    //    public string VideoUrl { get; set; }

    //    public short Status { get; set; }
        
    //    public int Category_Id { get; set; }

    //    public int Store_Id { get; set; }

    //    public bool IsDeleted { get; set; }

    //    public string Size { get; set; }

    //}

}