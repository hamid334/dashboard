using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [Range(1, 10000, ErrorMessage = "Please enter a valid delivery fee")]
        [RegularExpression(MyRegularExpressions.Price, ErrorMessage = "Please enter a valid delivery fee")]
        public double DeliveryFee { get; set; } = 0;

        [Required]
        public string Currency { get; set; } = "";
        public string HowItWorksUrl { get; set; } = "";
        public string HowItWorksDescription { get; set; } = "";
        public string AboutUs { get; set; } = "";


        public string BannerImage { get; set; } = "";
        public string InstagramImage { get; set; } = "";

        [Required(ErrorMessage = "This field is required")]
        [Range(1, 10000, ErrorMessage = "Please enter a valid delivery threshold")]
        [RegularExpression(MyRegularExpressions.Price, ErrorMessage = "Please enter a valid delivery threshold")]
        public double FreeDeliveryThreshold { get; set; } = 0;


    }
}