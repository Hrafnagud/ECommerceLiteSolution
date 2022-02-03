using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerceBusinessLogicLayer.Repository;

namespace ECommerceLiteUI.Controllers
{
    public class ChartController : Controller
    {
        CategoryRepo categoryRepo = new CategoryRepo();
        // GET: Chart
        public ActionResult VisualizePieChartResult()
        {
            //Receive data to visualize in the piechart.
            //To send this data to ajax operation in Dashboard, we'll proceed with Json.
            var data = categoryRepo.GetBaseCategoriesProductCount();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


    }
}