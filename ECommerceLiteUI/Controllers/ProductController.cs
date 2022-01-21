using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerceLiteEntity.Models;
using ECommerceBusinessLogicLayer.Repository;
namespace ECommerceLiteUI.Controllers
{
    public class ProductController : Controller
    {

        ProductRepo productRepo = new ProductRepo();
        CategoryRepo categoryRepo = new CategoryRepo();

        public ActionResult ProductList()
        {
            var allProductList = productRepo.GetAll();
            return View(allProductList);
        }


        [HttpGet]
        public ActionResult Create()
        {
            List<SelectListItem> allCategories = new List<SelectListItem>();
            categoryRepo.GetAll().ToList().ForEach(x => allCategories.Add(new SelectListItem()
            {
                Text = x.CategoryName,
                Value = x.Id.ToString()
            }));
            ViewBag.CategoryList= allCategories;
            return View();
        }

        [HttpPost]
        public ActionResult Create(Product model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Invalid data entry!");
                }

                int insertResult = productRepo.Insert(model);

                if (insertResult >  0)
                {
                    return RedirectToAction("ProductList", "Product");
                }

                else
                {
                    ModelState.AddModelError("", "Error has occured during insertion. Try again!");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error has occured: " + ex.Message);
                return View();
            }
        }
    }
}