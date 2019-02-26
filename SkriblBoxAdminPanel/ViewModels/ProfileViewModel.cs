using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.Text)]
        [RegularExpression(MyRegularExpressions.Name, ErrorMessage = "Only alphabets are allowed")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.Text)]
        [RegularExpression(MyRegularExpressions.Name, ErrorMessage = "Only alphabets are allowed")]
        public string LastName { get; set; }
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "PhoneNumber")]
        [StringLength(maximumLength: 15, MinimumLength = 10, ErrorMessage = "Phone Number length should be at least 10 digits")]
        public string Phone { get; set; }

        [Required]
        public string ImageUrl { get; set; }

    }
}