using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.Areas.Dashboard.Models
{
    public class SearchBoxesViewModel
    {
        public SearchBoxesViewModel()
        {
            Boxes = new List<Box>();
        }
        public List<Box> Boxes { get; set; }
        public Box Box { get; set; }
    }
    public class Box
    {
        public Box()
        {

        }

        public int Id { get; set; }

        public string Name { get; set; }

      
        public int Category_Id { get; set; }

        public int MerchantId { get; set; }

        public virtual AddCategoryViewModel Category { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Description { get; set; }

        public short Status { get; set; }

    }
}