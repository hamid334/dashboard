using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class FavouritesViewModel : BaseViewModel
    {
        public FavouritesViewModel()
        {
            Favourites = new List<Favourite>();
        }
        public List<Favourite> Favourites { get; set; }
        public int TotalRecords { get; set; }
    }
    public partial class Favourite
    {
        public int Id { get; set; }

        public int Product_Id { get; set; }

        public int User_ID { get; set; }

        public bool IsFavourite { get; set; }

        public bool IsDeleted { get; set; }

        public virtual Product Product { get; set; }

    }

    public partial class Product
    {

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        public double? DiscountPrice { get; set; }

        public double? DiscountPercentage { get; set; }

        //[Required]
        public string Description { get; set; }

        [NotMapped]
        public string Weight { get; set; }

        public int WeightUnit { get; set; }

        public double WeightInGrams { get; set; }

        public double WeightInKiloGrams { get; set; }

        public string ImageUrl { get; set; }

        public string VideoUrl { get; set; }

        public short Status { get; set; }

        //public int SubCategory_Id { get; set; }


        [ForeignKey("Store")]
        public int Store_Id { get; set; }

        public bool IsDeleted { get; set; }

        public string Size { get; set; }

        [ForeignKey("Category")]
        public int? Category_Id { get; set; }

        public int OrderedCount { get; set; }

        public double AverageRating { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}