using BasketWebPanel.Areas.Dashboard.Models;
using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.ViewModels
{
    public class SearchProductsViewModel : BaseViewModel
    {
        public SearchProductsViewModel()
        {
            Products = new List<ProductViewModel>();
        }
        public List<ProductViewModel> Products { get; set; }
    }
    public class ProductViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public double? DiscountPrice { get; set; }

        public double? DiscountPercentage { get; set; }

        //[Required]
        public string Description { get; set; }

        public string Weight { get; set; }

        public int WeightUnit { get; set; }

        public double WeightInGrams { get; set; }

        public double WeightInKiloGrams { get; set; }

        public string ImageUrl { get; set; }

        public string VideoUrl { get; set; }

        public short Status { get; set; }

        public int Store_Id { get; set; }

        public bool IsDeleted { get; set; }

        public string Size { get; set; }

        public int? Category_Id { get; set; }

        public bool IsFavourite { get; set; }

        public double AverageRating { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}