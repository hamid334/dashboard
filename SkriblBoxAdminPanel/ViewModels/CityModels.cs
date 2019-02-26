using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class CityModels
    {

    }

    public class CitiesViewModel : BaseViewModel
    {
        public CitiesViewModel()
        {
            Cities = new List<CityBindingModel>();
        }
        public List<CityBindingModel> Cities { get; set; }
    }

    public class CityContainer : BaseViewModel
    {
        public CityContainer()
        {
            City = new CityBindingModel();
        }

        public CityBindingModel City { get; set; }
    }

    public class CityBindingModel
    {
        public int Id { get; set; }

        public string CityName { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}