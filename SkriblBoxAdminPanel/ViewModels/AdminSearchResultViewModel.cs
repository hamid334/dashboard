using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.ViewModels
{
    public class AdminSearchResultViewModel : BaseViewModel
    {
        public AdminSearchResultViewModel()
        {
            Agents = new List<AdminBindingModel>();
        }

        public List<AdminBindingModel> Agents { get; set; }

        public SelectList StatusOptions { get; internal set; }
    }

    public class AdminBindingModel : BaseViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public short Role { get; set; }
        public bool IsDeleted { get; set; }

        public string ImageUrl { get; set; }
        public string StatusName { get; set; }
    }
}