using ECommerceBusinessLogicLayer.Account;
using ECommerceBusinessLogicLayer.Repository;
using ECommerceLiteEntity.IdentityModels;
using ECommerceLiteUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerceLiteUI.Controllers
{
    public class PartialsController : BaseController
    {

        CategoryRepo categoryRepo = new CategoryRepo();
        ProductRepo productRepo = new ProductRepo();

        public PartialViewResult AdminSideBarResult()
        {
            //TODO: fullname will be received
            TempData["Fullname"] = "";
            return PartialView("_PartialAdminSideBar");
        }
        public PartialViewResult AdminSideBarMenuResult()
        {
            return PartialView("_PartialAdminSideBarMenu");
        }

        public PartialViewResult UserFullnameOnHomePage()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var loggedInUser = MembershipTools.GetUser();
                return PartialView("_PartialUserFullNameOnHomePage", loggedInUser);
            }
            return PartialView("_PartialUserFullNameOnHomePage", null);
        }

        public PartialViewResult ShoppingCart()
        {
            var shoppingCart = Session["ShoppingCart"] as List<CartViewModel>;

            if (shoppingCart == null)
            {
                return PartialView("_PartialShoppingCart", new List<CartViewModel>());
            }
            else
            {
                return PartialView("_PartialShoppingCart", shoppingCart);
            }
        }

        public ActionResult AdminSideBarCategories()
        {
            TempData["AllCategoriesCount"] = categoryRepo.Queryable().Where(x => x.BaseCategory == null).ToList().Count;
            return PartialView("_PartialAdminSideBarCategories");
        }

        public ActionResult AdminSideBarProducts()
        {
            TempData["AllProductsCount"] = productRepo.GetAll().ToList().Count; //Product counts will be obtained.
            return PartialView("_PartialAdminSideBarProducts");     //Create new partial view under View => Shared with the same name partial view
        }
    }
}