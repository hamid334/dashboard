using BasketWebPanel.DomainModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.ViewModels
{
    public partial class UserViewModels
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]

        public int Id { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        public string StreetAddress { get; set; }

        public string Area { get; set; }
        public int CategoryId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }

        public string InstagramUrl { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(200)]
        public string FullName { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public string AccountType { get; set; }

        public string ZipCode { get; set; }

        public string DateofBirth { get; set; }

        public short? SignInType { get; set; }

        public string UserName { get; set; }

        public short? Status { get; set; }

        public bool EmailConfirmed { get; set; }

        public string Nationality { get; set; }

        public bool PhoneConfirmed { get; set; }



        public bool IsNotificationsOn { get; set; }

        [NotMapped]
        public Token Token { get; set; }

        //[NotMapped]
        //public Settings BasketSettings { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class AddAdminViewModel : BaseViewModel
    {
        public AddAdminViewModel()
        {
            Admin = new AdminViewModel();
        }
        public AdminViewModel Admin { get; set; }

        public SelectList StoreOptions { get; internal set; }

        public SelectList RoleOptions { get; internal set; }
    }

    public class AdminViewModel : BaseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.Text)]
        [RegularExpression(MyRegularExpressions.Name, ErrorMessage = "Only alphabets are allowed")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.Text)]
        [RegularExpression(MyRegularExpressions.Name, ErrorMessage = "Only alphabets are allowed")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "PhoneNumber")]
        [StringLength(maximumLength: 15, MinimumLength = 10, ErrorMessage = "Phone Number length should be at least 10 digits")]
        public string Phone { get; set; }

        [Required]
        public short Role { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The Password and Confirm Password don't match")]
        public string ConfirmPassword { get; set; }

        public string AccountNo { get; set; }

        public int? Store_Id { get; set; }

        public short? Status { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public Token Token { get; set; }
    }
}