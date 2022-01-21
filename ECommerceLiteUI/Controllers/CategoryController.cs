using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerceBusinessLogicLayer.Repository;
using ECommerceLiteEntity.Models;

namespace ECommerceLiteUI.Controllers
{
    public class CategoryController : Controller
    {
        CategoryRepo categoryRepo = new CategoryRepo();
        
        public ActionResult CategoryList()
        {
            var allCategories = categoryRepo.Queryable().Where(x => x.BaseCategoryId == null).ToList();
            ViewBag.CategoryCount = allCategories.Count;
            TempData["CurrentWindow"] = "base";
            return View(allCategories);
        }

        public ActionResult Create(int? id, bool IsSendFromSubCategory = false)
        {
            ViewBag.CategoryName = string.Empty;    //Betul's alternative
            if (id != null)
            {

                Category model = new Category()
                {
                    Id = id.Value
                };
                ViewBag.CategoryName= categoryRepo.GetById(id.Value).CategoryName;
                return View(model);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create (Category model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Invalid data entry. Please try again");
                    return View(model);
                }

                Category newCategory = new Category()
                {
                    CategoryName = model.CategoryName,
                    CategoryDescription = model.CategoryDescription,
                    RegisterDate = DateTime.Now,
                    BaseCategoryId = null
                };

                if (model.Id > 0)
                {
                    newCategory.BaseCategoryId = model.Id;
                }

                int insertResult = categoryRepo.Insert(newCategory);

                if (insertResult > 0 && model.Id == 0)
                {
                    return RedirectToAction("CategoryList", "Category");
                }
                else if (insertResult > 0 && model.Id > 0)
                {
                    return RedirectToAction("SubCategoryList", "Category", new { id = model.Id });
                }
                else
                {
                    throw new Exception("Error Has occured while adding new category. Error Message: ");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public ActionResult SubCategoryList(int id)
        {
            var subCategories= categoryRepo.Queryable().Where(x => x.BaseCategoryId != null && x.BaseCategoryId == id).ToList();
            ViewBag.CategoryId = id;
            ViewBag.CategoryName = categoryRepo.GetById(id).CategoryName;
            ViewBag.SubCategoryCount = subCategories.Count;
            TempData["CurrentWindow"] = "sub";
            return View(subCategories);
        }
    }
}