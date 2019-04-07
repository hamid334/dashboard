using BasketWebPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.BindingModels
{
    public class RequestCardBindingModel : BaseViewModel
    {
        public List<RequestCardModel> RequestCard { get; set; }
    }
    public class RequestCardModel 
    {
        public RequestCardModel()
        {
            RequestCard = new List<RequestCardModel>();
        }
        public List<RequestCardModel> RequestCard { get; set; }
        public int Id { get; set; }
        public string DeliveryAddress { get; set; }

        public string DeliveryState { get; set; }

        public string DeliveryCity { get; set; }

        public double DeliveryZipCode { get; set; }

        public string NomineeName { get; set; }

        public DateTime? NomineeDOB { get; set; }

        public string NomineeGender { get; set; }

        public string NomineeRelationship { get; set; }

        public string CardNumber { get; set; }

        public string CVV { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public string Signature { get; set; }

        public int User_Id { get; set; }

        public User User { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class User
    {

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public short? SignInType { get; set; }

        public string Nationality { get; set; }

    }
}