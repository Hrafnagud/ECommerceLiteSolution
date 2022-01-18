using ECommerceBusinessLogicLayer.Account;
using ECommerceBusinessLogicLayer.Repository;
using ECommerceBusinessLogicLayer.Settings;
using ECommerceLiteEntity.Enums;
using ECommerceLiteEntity.IdentityModels;
using ECommerceLiteEntity.Models;
using ECommerceLiteEntity.ViewModels;
using ECommerceLiteUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
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
        CustomerRepo customerRepo = new CustomerRepo();
        PassiveUserRepo passiveUserRepo = new PassiveUserRepo();
        UserManager<ApplicationUser> userManager = MembershipTools.NewUserManager();
        UserStore<ApplicationUser> userStore = MembershipTools.NewUserStore();
        RoleManager<ApplicationRole> roleManager = MembershipTools.NewRoleManager();


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
                var user = userStore.Context.Set<ApplicationUser>().FirstOrDefault(x => x.ActivationCode == code);
                if (user == null)
                {
                    ViewBag.ActivationResult = "Activation failed!";
                    return View();
                }

                if (user.EmailConfirmed)
                {
                    ViewBag.ActivationResult = "Mail address has already been confirmed!";
                    return View();
                }
                user.EmailConfirmed = true;
                await userStore.UpdateAsync(user);
                await userStore.Context.SaveChangesAsync();

                PassiveUser passiveUser = passiveUserRepo.Queryable().FirstOrDefault(x => x.UserId == user.Id);
                if (passiveUser != null)
                {
                    if (passiveUser.TargetRole == IdentityRoles.Customer)
                    {
                        //new customer to be created and saved. Then User's passive role will be removed and customer role will be added.
                        Customer newCustomer = new Customer()
                        {
                            TRID = passiveUser.TRID,
                            UserId = user.Id,
                        };
                        customerRepo.Insert(newCustomer);
                        //Now customer is not a passive user. So record can be removed from the passive table.
                        passiveUserRepo.Delete(passiveUser);
                        //Adding customer role
                        userManager.RemoveFromRole(user.Id, IdentityRoles.Passive.ToString());
                        userManager.AddToRole(user.Id, IdentityRoles.Customer.ToString());
                        ViewBag.ActivationResult = $"Hello {user.Name} {user.Surname}, Activation process had been successfuly done!";
                        return View();
                    }
                }
                return View();
            }
            catch (Exception ex)
            {

                //TODO: ex Loglama
                ModelState.AddModelError("", "Unexpected error has occured!");
                return View();
            }
        }

        [HttpGet]
        public ActionResult Login(string ReturnUrl, string email)
        {
            try
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    var url = ReturnUrl.Split('/');
                }
                var model = new LoginViewModel()
                {
                    ReturnUrl = ReturnUrl
                };
                return View(model);
            }
            catch (Exception ex)
            {
                //log ex
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) {
                    return View(model);
                }

                var user = await userManager.FindAsync(model.Email, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Make sure you entered email and password correctly.");
                    return View(model);
                }
                if (user.Roles.FirstOrDefault().RoleId == roleManager.FindByName(Enum.GetName(typeof(IdentityRoles), IdentityRoles.Passive )).Id)
                {
                    ViewBag.Result = "Sistemi kullanabilmeniz için üyeliğinizi aktifleştirmeniz gerekmektedir. Emailinize gönderilen aktivasyon linkine tıklayarak aktifleştirme işlemini yapabilirsiniz. ";
                    return View(model);
                }
                var authManager = HttpContext.GetOwinContext().Authentication;
                var userIdentity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                authManager.SignIn(new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe
                }, userIdentity);
                if (user.Roles.FirstOrDefault().RoleId == roleManager.FindByName(Enum.GetName(typeof(IdentityRoles), IdentityRoles.Admin)).Id)
                {
                    return RedirectToAction("Index", "Admin");

                }
                if (user.Roles.FirstOrDefault().RoleId == roleManager.FindByName(Enum.GetName(typeof(IdentityRoles), IdentityRoles.Customer)).Id)
                {
                    return RedirectToAction("Index", "Home");

                }

                if (string.IsNullOrEmpty(model.ReturnUrl))
                    return RedirectToAction("Index", "Home");

                var url = model.ReturnUrl.Split('/');
                if (url.Length == 4)
                    return RedirectToAction(url[2], url[1], new { id = url[3] });
                else
                    return RedirectToAction(url[2], url[1]);
            }
            catch (Exception ex)
            {
                //Todo log ex
                ModelState.AddModelError(null, "Unexpected error has occured!");
                return View(model);
            }
        }
    }
}