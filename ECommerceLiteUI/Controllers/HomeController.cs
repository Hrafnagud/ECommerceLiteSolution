using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerceBusinessLogicLayer.Repository;
using Mapster;
using ECommerceLiteUI.Models;
using ECommerceLiteEntity.Models;
using ECommerceBusinessLogicLayer.Account;
using QRCoder;
using System.Drawing;
using ECommerceLiteEntity.ViewModels;
using ECommerceBusinessLogicLayer.Settings;
using System.Threading.Tasks;

namespace ECommerceLiteUI.Controllers
{
    public class HomeController : BaseController
    {
        CategoryRepo categoryRepo = new CategoryRepo();
        ProductRepo productRepo = new ProductRepo();
        OrderRepo orderRepo = new OrderRepo();
        OrderDetailRepo orderDetailRepo = new OrderDetailRepo();
        CustomerRepo customerRepo = new CustomerRepo();

        public ActionResult Index()
        {
            var categoryList = categoryRepo.Queryable().Where(x => x.BaseCategoryId == null).ToList();
            ViewBag.CategoryList = categoryList;
            var productList = productRepo.Queryable().Where(x => x.Quantity >= 1).ToList();
            List<ProductViewModel> model = new List<ProductViewModel>();
            foreach (var item in productList)
            {
                model.Add(item.Adapt<ProductViewModel>());
            }

            foreach (var item in model)
            {
                item.SetCategory();
                item.SetProductPictures();
            }
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult AddToCart(int id)
        {
            try
            {
                var shoppingCart = Session["ShoppingCart"] as List<CartViewModel>;
                if (shoppingCart == null)
                {
                    shoppingCart = new List<CartViewModel>();
                }
                if (id > 0)
                {
                    var product = productRepo.GetById(id);
                    if (product == null)
                    {
                        TempData["AddToCart"] = "Product insertion has been failed. Please try again!";
                        return RedirectToAction("Index", "Home");
                    }
                    var productAddToCart = product.Adapt<CartViewModel>();
                    if (shoppingCart.Count(x => x.Id == productAddToCart.Id) > 0)
                    {
                        shoppingCart.FirstOrDefault(x => x.Id == productAddToCart.Id).Quantity++;
                    }
                    else
                    {
                        productAddToCart.Quantity = 1;
                        shoppingCart.Add(productAddToCart);
                    }
                    Session["ShoppingCart"] = shoppingCart;
                    TempData["AddToCart"] = "Product has been added.";
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["AddToCart"] = "Product insertion has been failed. Please try again!";
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public async Task<ActionResult> Buy()
        {
            try
            {
                var shoppingCart = Session["ShoppingCart"] as List<CartViewModel>;
                if (shoppingCart != null)
                {
                    if (shoppingCart.Count > 0)
                    {
                        var user = MembershipTools.GetUser();
                        var customer = customerRepo.Queryable().FirstOrDefault(x => x.UserId == user.Id);

                        Order newOrder = new Order()
                        {
                            CustomerTRID = customer.TRID,
                            RegisterDate = DateTime.Now,
                            OrderNumber = "1234567"
                        };

                        int orderInsertResult = orderRepo.Insert(newOrder);

                        if (orderInsertResult > 0)
                        {
                            bool isSuccess = false;
                            foreach (var item in shoppingCart)
                            {
                                OrderDetail newOrderDetail = new OrderDetail()
                                {
                                    OrderId = newOrder.Id,
                                    ProductId = item.Id,
                                    Discount = 0,
                                    ProductPrice = item.Price,
                                    Quantity = item.Quantity,
                                    RegisterDate = DateTime.Now
                                };
                                if (newOrderDetail.Discount > 0)
                                {
                                    newOrderDetail.TotalPrice = newOrderDetail.Quantity * (newOrderDetail.ProductPrice - (newOrderDetail.ProductPrice * Convert.ToDecimal(newOrderDetail.Discount/100)));
                                }
                                else
                                {
                                    newOrderDetail.TotalPrice = newOrderDetail.Quantity * newOrderDetail.ProductPrice;
                                }
                                int detailInsertResult = orderDetailRepo.Insert(newOrderDetail);
                                if (detailInsertResult > 0)
                                {
                                    isSuccess = true;
                                }
                            }
                            if (isSuccess)
                            {
                                //QR Email
                                #region SendEmail

                                QRCodeGenerator QRGenerator = new QRCodeGenerator();
                                QRCodeData QRData = QRGenerator.CreateQrCode(newOrder.OrderNumber, QRCodeGenerator.ECCLevel.Q);
                                QRCode QRCode = new QRCode(QRData);
                                Bitmap QRBitmap = QRCode.GetGraphic(60);
                                byte[] bitmapArray = BitmapToByteArray(QRBitmap);
                                string QRUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(bitmapArray));

                                List<OrderDetail> orderDetailList = new List<OrderDetail>();
                                orderDetailList = orderDetailRepo.Queryable().Where(x => x.OrderId == newOrder.Id).ToList();

                                string message = $"Merhaba {user.Name} {user.Surname} <br/>" +
                                                   $"{orderDetailList.Count} adet ürünlerinizin siparişini aldık.<br/>" +
                                                   $"Toplam Tutar:{orderDetailList.Sum(x => x.TotalPrice).ToString()} ₺ <br/> <br/>" +
                                                   $"<table><tr><th>Ürün Adı</th><th>Adet</th><th>Birim Fiyat</th><th>Toplam</th></tr>";
                                foreach (var item in orderDetailList)
                                {
                                    message += $"<tr><td>{productRepo.GetById(item.ProductId).ProductName}</td><td>{item.Quantity}</td><td>{item.TotalPrice}</td></tr>";
                                }
                                message += "</table><br/>Siparişinize ait QR kodunuz: <br/>";
                                message += $"<a href='/Home/Order/{newOrder.Id}'><img src=\"{QRUri}\" height=250px;  width=250px; class='img-thumbnail' /></a>";
                                await SiteSettings.SendMail(new MailModel()
                                {
                                    To = user.Email,
                                    Subject = "ECommerceLite - Siparişiniz alındı",
                                    Message = message

                                });

                                #endregion
                                return RedirectToAction("Order", "Home", new { id = newOrder.Id });
                            }
                            else
                            {
                                //
                            }
                        }
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                //ex log
                //Result will be redirected to Homepage via TempData 
                return RedirectToAction("Index", "Home");
            }
        }



        [Authorize]
        public ActionResult Order(int? id)
        {
            try
            {
                if (id > 0)
                {
                    Order customerOrder = orderRepo.GetById(id.Value);
                    List<OrderDetail> orderDetails = new List<OrderDetail>();
                    if (customerOrder != null)
                    {
                        orderDetails = orderDetailRepo.Queryable().Where(x => x.OrderId == customerOrder.Id).ToList();
                        foreach (var item in orderDetails)
                        {
                            item.Product = productRepo.GetById(item.ProductId);
                        }
                        ViewBag.OrderSuccess = "Your order has been successfully created!";
                        Session["ShoppingCart"] = null;
                        return View(orderDetails);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Product not found! Try again.");
                        return View(orderDetails);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Product not found! Try again.");
                    return View(new List<OrderDetail>());
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unexpected error has occured!");
                return View(new List<OrderDetail>());
            }
        }
    }
}