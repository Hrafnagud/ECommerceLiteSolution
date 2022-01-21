using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerceBusinessLogicLayer.Repository;

namespace ECommerceLiteUI.Controllers
{
    public class CategoryController : Controller
    {
        CategoryRepo categoryRepo = new CategoryRepo();
        
        public ActionResult CategoryList()
        {
            var allCategories = categoryRepo.Queryable().Where(x => x.BaseCategoryId == null).ToList(); ;
            ViewBag.CategoryCount = allCategories.Count;
            return View(allCategories);
        }
    }
}