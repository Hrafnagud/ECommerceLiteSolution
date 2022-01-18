using ECommerceBusinessLogicLayer.Account;
using ECommerceBusinessLogicLayer.Repository;
using ECommerceBusinessLogicLayer.Settings;
using ECommerceLiteEntity.Enums;
using ECommerceLiteEntity.IdentityModels;
using ECommerceLiteEntity.Models;
using ECommerceLiteEntity.ViewModels;
using ECommerceLiteUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ECommerceLiteUI.Controllers
{
    public class AccountController : BaseController
    {
        //Removed Index method that comes with AccountController
        PassiveUserRepo passiveUserRepo = new PassiveUserRepo();


        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var userManager = MembershipTools.NewUserManager();
                var userStore = MembershipTools.NewUserStore();
                var checkUserTRID = userStore.Context.Set<Customer>().FirstOrDefault(x => x.TRID == model.TRID)?.TRID;

                if (checkUserTRID != null)
                {
                    ModelState.AddModelError(null, "This TRID has already been registered to the system!");
                    return View(model);
                }

                var checkUserEmail = userStore.Context.Set<ApplicationUser>().FirstOrDefault(x => x.Email == model.Email)?.Email;

                if (checkUserEmail != null)
                {
                    ModelState.AddModelError(null, "This Email has already been registered to the system!");
                    return View(model);
                }

                var activationCode = Guid.NewGuid().ToString().Replace("-", "");
                var newUser = new ApplicationUser()
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.Email,
                    ActivationCode = activationCode
                };
                var theResult = userManager.CreateAsync(newUser, model.Password);
                if (theResult.Result.Succeeded)
                {
                    //If new agent recorded to Aspnet table, new record will be saved into passive table. If activation steps are followed by the suer, record will be
                    //removed from passive tables and added to related table.
                    await userManager.AddToRoleAsync(newUser.Id, IdentityRoles.Passive.ToString());
                    PassiveUser newPassiveUser = new PassiveUser()
                    {
                        TRID = model.TRID,
                        UserId = newUser.Id,
                        TargetRole = IdentityRoles.Customer
                    };
                    passiveUserRepo.Insert(newPassiveUser);

                    string siteUrl = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);

                    await SiteSettings.SendMail(new MailModel() { 
                        To = newUser.Email,
                        Subject = "ECommerceLite Activation Link",
                        Message = $"Hello {newUser.Name} {newUser.Surname}!,<br>You can <b><a href='{siteUrl}/Account/Activation?code={activationCode}'>click</a></b> here to activate."
                    });

                    return RedirectToAction("Login", "Account", new { email = $"{newUser.Email}" });
                }

                else
                {
                    ModelState.AddModelError(null, "Something went wrong! Good luck debugging an asynchronous method!");
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                //TODO: Log ex
                ModelState.AddModelError(null, "Something went wrong! Good luck debugging an asynchronous method!");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Activation(string code)
        {
            try
            {
                var userStore = MembershipTools.NewUserStore();
                var theResult = userStore.Context.Set<ApplicationUser>().FirstOrDefault(x => x.ActivationCode == code);
                if (theResult == null)
                {
                    ViewBag.ActivationResult = "Activation failed!";
                    return View();
                }

                if (theResult.EmailConfirmed)
                {
                    ViewBag.ActivationResult = "Mail address has been confirmed!";
                    return View();
                }
                theResult.EmailConfirmed = true;
                await userStore.UpdateAsync(theResult);
                await userStore.Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                //TODO: ex Loglama
                ModelState.AddModelError("", "Unexpected error has occured!");
                return View();
            }
        }

    }
}