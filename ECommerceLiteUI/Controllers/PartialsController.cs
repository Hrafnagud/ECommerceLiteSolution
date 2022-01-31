﻿using ECommerceBusinessLogicLayer.Account;
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
    }
}