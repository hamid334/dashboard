﻿using System.ComponentModel.DataAnnotations;

namespace BasketWebPanel.BindingModels
{
    public class ProductBindingModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Range(0, 10000, ErrorMessage = "Please enter a valid price")]
        [RegularExpression(MyRegularExpressions.Price, ErrorMessage = "Please enter a valid price")]
        public double Price { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Description { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public string VideoUrl { get; set; }

        public short Status { get; set; }

        //[Required]
        //[Range(1, int.MaxValue, ErrorMessage = "Please select category")]
        public int Category_Id { get; set; }

        public int Store_Id = Global.StoreId;

        [Required(ErrorMessage = "This field is required")]
        public double DiscountPrice { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public double? DiscountPercentage { get; set; }

        public string Size { get; set; }

        public virtual CategoryBindingModel Category { get; set; }

        public virtual StoreBindingModel Store { get; set; }

        public int BoxId { get; set; }
    }
}