﻿using ECommerceBusinessLogicLayer.Account;
using ECommerceLiteEntity.Enums;
using ECommerceLiteEntity.IdentityModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ECommerceLiteUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var roleManager = MembershipTools.NewRoleManager();
            var roles = Enum.GetNames(typeof(IdentityRoles));
            foreach (var role in roles)
            {
                if (!roleManager.RoleExists(role))
                    roleManager.Create(new ApplicationRole()
                    {
                        Name = role
                    });
            }
        }
    }
}
