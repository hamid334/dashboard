using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class AddHowItWorksViewModel : BaseViewModel
    {
        [Url]
        [Required(ErrorMessage = "This field is required")]
        public string Url { get; set; }

        public string HowItWorksDescription { get; set; }
    }
}