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

        // GET: Admin
        public ActionResult Dashboard()
        {
            var orderList = orderRepo.GetAll();
            var newOrderCount = orderList.Where(x => x.RegisterDate >= DateTime.Now.AddMonths(-1)).ToList().Count();
            ViewBag.NewOrderCount = newOrderCount;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Test()
        {
            return View();
        }
    }
}