using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.ViewModels
{
    public class SearchBoxesViewModel : BaseViewModel
    {
        public List<Box> Boxes { get; set; }
        public SelectList StatusOptions { get; internal set; }
    }

    public class Box
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string IntroUrl { get; set; }

        public double Price { get; set; }

        public bool IsDeleted { get; set; }

        public short Status { get; set; }

        public string StatusName { get; set; }

        public int BoxCategory_Id { get; set; }
        
        public bool IsChecked { get; set; }

        public string CategoryName  { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ReleaseDate { get; set; }

        public AppCategoryViewModel Category { get; set; }

    }

    public class AppCategoryViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public short Status { get; set; }

        public string ImageUrl { get; set; }

        public int ParentCategoryId { get; set; }

        public bool IsDeleted { get; set; }

        public int? Sorting { get; set; }

    }
}