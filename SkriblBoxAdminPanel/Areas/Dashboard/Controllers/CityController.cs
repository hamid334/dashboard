using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static BasketWebPanel.Utility;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    public class CityController : Controller
    {
        // GET: Dashboard/City
        public ActionResult Index(int? CityId)
        {
           
            CityContainer model = new CityContainer();

        
            model.SetSharedData(User);

         if (CityId.HasValue)
            {
                var responseProduct = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetEntityById", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.City, "Id=" + CityId.Value));
                if (responseProduct == null || responseProduct is Error)
                    ;
                else
                {
                    model.City = responseProduct.GetValue("Result").ToObject<CityBindingModel>();
                }
            }
          
            model.SetSharedData(User);
            return View(model);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(CityContainer model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View(model);
                }


                var response = await ApiCall.CallApi("api/Admin/AddCity", User,model.City);
                if (response.ToString().Contains("UnAuthorized"))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
                }
                else if (response.ToString().Contains("UnAuthorized"))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "UnAuthorized Error");
                }

                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
                }
                else
                {
                    if (model.City.Id > 0)
                        TempData["SuccessMessage"] = "The City has been updated successfully.";
                    else
                        TempData["SuccessMessage"] = "The City has been added successfully.";

                    return RedirectToAction("ManageCities");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async  Task<ActionResult> ManageCities()
        {
            CitiesViewModel returnModel = new CitiesViewModel();
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Common/GetAllCities", User, null, true, false, null, "Page=0&Items=1000"));
            if (response == null || response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                returnModel.Cities = response.GetValue("Result").ToObject<List<CityBindingModel>>().OrderBy(x => x.CityName).ToList();
            }
            returnModel.SetSharedData(User);
            return View(returnModel);
        }
        public JsonResult DeleteCity(int? CityId)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/DeleteEntity", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.City, "Id=" + CityId.Value));
                if (response is Error)
                    return Json("An error has occurred, error code : 500", JsonRequestBehavior.AllowGet);
                else
                    return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}