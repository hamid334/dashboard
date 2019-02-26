using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.ViewModels
{
    public class AddBoxViewModel : BaseViewModel
    {

        [Required(ErrorMessage = "This field is required")]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime ReleaseDate { get; set; }


        public SelectList Categories { get; internal set; }

        public int Category_Id { get; set; }


        public MerchantViewModel Merchant { get; set; }

    }

    public class MerchantViewModel
    {
        [Required(ErrorMessage = "This field is required")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(100)]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "This field is required")]
        public string Area { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Country { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Phone { get; set; }

    }
    public class Video
    {
        [Url]
        public string VideoUrl { get; set; }
    }
}