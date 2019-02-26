using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class AddNewPostViewModel : BaseViewModel
    {
        [Required]
        public string ImageUrl { get; set; }
    }
}