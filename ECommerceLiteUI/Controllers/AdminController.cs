using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerceBusinessLogicLayer.Repository;

namespace ECommerceLiteUI.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        OrderRepo orderRepo = new OrderRepo();
        CategoryRepo categoryRepo = new CategoryRepo();

        // GET: Admin
        public ActionResult Dashboard()
        {
            var orderList = orderRepo.GetAll();
            var newOrderCount = orderList.Where(x => x.RegisterDate >= DateTime.Now.AddMonths(-1)).ToList().Count();
            ViewBag.NewOrderCount = newOrderCount;
            var model = categoryRepo.GetBaseCategoriesProductCount();
            return View();
        }

        public ActionResult Dashboard2()
        {
            var orderList =
               orderRepo.GetAll();
            //1 aylık sipariş sayısı
            var newOrderCount = orderList.Where(x => x.RegisterDate >= DateTime.Now.AddMonths(-1)).ToList().Count;
            ViewBag.NewOrderCount = newOrderCount;

            var model = categoryRepo.GetBaseCategoriesProductCount();
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Test()
        {
            return View();
        }
    }
}