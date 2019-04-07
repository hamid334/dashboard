using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.BindingModels
{
    public class BoxBindingModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Category_Id { get; set; }
        
    }
    public class BoxCategoreisView
    {
        public List<BoxBindingModel> Boxes {get;set;}
    }
  
}