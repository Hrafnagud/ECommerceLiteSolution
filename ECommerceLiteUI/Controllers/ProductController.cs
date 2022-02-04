using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerceLiteEntity.Models;
using ECommerceBusinessLogicLayer.Repository;
using ECommerceLiteUI.Models;
using Mapster;
using ECommerceBusinessLogicLayer.Settings;
using System.IO;
using PagedList;
using ECommerceBusinessLogicLayer.Account;

namespace ECommerceLiteUI.Controllers
{
    public class ProductController : Controller
    {

        ProductRepo productRepo = new ProductRepo();
        CategoryRepo categoryRepo = new CategoryRepo();
        ProductPictureRepo productPictureRepo = new ProductPictureRepo();

        public ActionResult ProductList(int page = 1, string search="", bool isNew = false)
        {
            List<SelectListItem> subCategories = new List<SelectListItem>();
            categoryRepo.Queryable()
                .Where(x => x.BaseCategoryId != null)
                .ToList().ForEach(x => subCategories.Add(new SelectListItem()
                {
                    Text = x.CategoryName,
                    Value = x.Id.ToString()
                }));
            ViewBag.CategoryList = subCategories;

            List<Product> allProductList = new List<Product>();
            if (string.IsNullOrEmpty(search))
            {
                allProductList = productRepo.GetAll();
            }
            else
            {
                allProductList = productRepo.Queryable().Where(x => x.ProductName.Contains(search)).ToList();
            }

            if (isNew)
            {
                allProductList = productRepo.GetAll();
                allProductList = allProductList.Where(x =>
                x.RegisterDate >= DateTime.Now.AddDays(-1)).ToList();

            }

            var user = MembershipTools.GetFullName();
            LogManager.LogMessage("Arrived!", userInfo: user, pageInfo: "/Product/ProductList");
            return View(allProductList.ToPagedList(page, 3));
        }


        [HttpGet]
        public ActionResult Create()
        {
            List<SelectListItem> subCategories = new List<SelectListItem>();
            categoryRepo.Queryable().Where(x => x.BaseCategoryId != null).ToList().ForEach(x => subCategories.Add(new SelectListItem()
            {
                Text = x.CategoryName,
                Value = x.Id.ToString()
            }));
            ViewBag.CategoryList= subCategories;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductViewModel model)
        {
            try
            {
                List<SelectListItem> subCategories = new List<SelectListItem>();
                categoryRepo.Queryable().Where(x => x.BaseCategoryId != null).ToList().ForEach(x => subCategories.Add(new SelectListItem()
                {
                    Text = x.CategoryName,
                    Value = x.Id.ToString()
                }));
                ViewBag.CategoryList = subCategories;

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Wrong Entry!");
                    return View(model);
                }
                //Mapping will be performed as following
                Product newProduct = model.Adapt<Product>();
                int insertResult = productRepo.Insert(newProduct);

                if (insertResult > 0)
                {
                    //Product images will also be added.
                    if (model.Files.Any())  //If anything selected as productImage there will be an image to add
                    {
                        ProductPicture productPicture = new ProductPicture();
                        productPicture.ProductId = newProduct.Id;
                        productPicture.RegisterDate = DateTime.Now;
                        int counter = 1;

                        foreach (var item in model.Files)
                        {
                            if (item != null && item.ContentType.Contains("image") && item.ContentLength > 0)
                            {

                                string filename = SiteSettings.UrlFormatConverter(model.ProductName).ToLower().Replace("-", "");
                                string extName = Path.GetExtension(item.FileName);

                                string guid = Guid.NewGuid().ToString().Replace("-", "");
                                var directoryPath = Server.MapPath($"~/ProductPictures/{filename}/{model.ProductCode}");
                                var filePath = Server.MapPath($"~/ProductPictures/{filename}/{model.ProductCode}/") + filename + counter + "-" + guid + extName;
                                if (!Directory.Exists(directoryPath))
                                {
                                    Directory.CreateDirectory(directoryPath);
                                }
                                item.SaveAs(filePath);
                                if (counter == 1)
                                {
                                    productPicture.ProductPicture1 = $"/ProductPictures/{filename}/{model.ProductCode}/" +filename + counter + "-" + guid + extName;
                                }
                                else if (counter == 2)
                                {
                                    productPicture.ProductPicture2 = $"/ProductPictures/{filename}/{model.ProductCode}/" +filename + counter + "-" + guid + extName;
                                }
                                else if (counter == 3)
                                {
                                    productPicture.ProductPicture3 = $"/ProductPictures/{filename}/{model.ProductCode}/" +filename + counter + "-" + guid + extName;
                                }
                                else if (counter == 4)
                                {
                                    productPicture.ProductPicture4 = $"/ProductPictures/{filename}/{model.ProductCode}/"+filename + counter + "-" + guid + extName;
                                }
                                else if (counter == 5)
                                {
                                    productPicture.ProductPicture5 = $"/ProductPictures/{filename}/{model.ProductCode}/" +filename + counter + "-" + guid + extName;
                                }
                            }
                            counter++;
                        }

                        int pictureInsertResult = productPictureRepo.Insert(productPicture);
                        if (pictureInsertResult > 0)
                        {
                            return RedirectToAction("ProductList", "Product");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Product has been inserted but error has occured during image insertion. Try again!");
                            return View(model);
                        }
                    }
                    else
                    {
                        return RedirectToAction("ProductList", "Product");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "An error has occured during product insertion. Try again!");
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unexpected error has occured!");
                var user = MembershipTools.GetFullName();
                LogManager.LogMessage(ex.ToString(), userInfo: user, pageInfo: "Product/Create");
                return View(model);
            }
        }

        public ActionResult CategoryProducts()
        {
            try
            {
                var list = categoryRepo.GetBaseCategoriesProductCount();
                return View(list);
            }
            catch (Exception ex)
            {
                var user = MembershipTools.GetFullName();
                LogManager.LogMessage(ex.ToString(), userInfo: user, pageInfo: "Product/CategoryProduct");
                return View();  //Tomorrow we will be modifying this section
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ProductModalError"] = "Veri girişleri düzgün olmalıdır!";
                    return RedirectToAction("ProductList", "Product");
                }
                var product = productRepo.GetById(model.Id);
                product.ProductName = model.ProductName;
                product.Description = model.Description;
                product.Quantity = model.Quantity;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;
                //if (model.Files.Any())
                //{
                //    //Önce sildik
                //    var pictureList =
                //          productPictureRepo.Queryable()
                //          .Where(x => x.ProductId == model.Id).ToList();
                //    foreach (var item in pictureList)
                //    {
                //        productPictureRepo.Delete(item);
                //    }
                //    //sonra yeni eklediklerini oluşturacağız.
                //    ProductPicture productPicture = new ProductPicture();
                //    productPicture.ProductId = model.Id;
                //    productPicture.RegisterDate = DateTime.Now;
                //    int counter = 1;
                //    foreach (var item in model.Files)
                //    {

                //        if (item != null && item.ContentType.Contains("image") && item.ContentLength > 0)
                //        {

                //            string filename = SiteSettings.UrlFormatConverter(model.ProductName).ToLower().Replace("-", "");
                //            string extName = Path
                //                .GetExtension(item.FileName);

                //            string guid = Guid.NewGuid()
                //                .ToString().Replace("-", "");
                //            var directoryPath = Server.MapPath($"~/ProductPictures/{filename}/{model.ProductCode}");
                //            var filePath = Server.MapPath($"~/ProductPictures/{filename}/{model.ProductCode}/") + filename + counter + "-" + guid + extName;
                //            if (!Directory.Exists(directoryPath))
                //            {
                //                Directory.CreateDirectory(directoryPath);
                //            }
                //            item.SaveAs(filePath);
                //            if (counter == 1)
                //            {
                //                productPicture.ProductPicture1 = $"/ProductPictures/{filename}/{model.ProductCode}/" + filename + counter + "-" + guid + extName;
                //            }
                //            if (counter == 2)
                //            {
                //                productPicture.ProductPicture2 = $"/ProductPictures/{filename}/{model.ProductCode}/" + filename + counter + "-" + guid + extName;
                //            }
                //            if (counter == 3)
                //            {
                //                productPicture.ProductPicture3 = $"/ProductPictures/{filename}/{model.ProductCode}/" + filename + counter + "-" + guid + extName;
                //            }
                //            if (counter == 4)
                //            {
                //                productPicture.ProductPicture4 = $"/ProductPictures/{filename}/{model.ProductCode}/" + filename + counter + "-" + guid + extName;
                //            }
                //            if (counter == 5)
                //            {
                //                productPicture.ProductPicture5 = $"/ProductPictures/{filename}/{model.ProductCode}/" + filename + counter + "-" + guid + extName;
                //            }


                //        }
                //        counter++;
                //    }

                //    int pictureInsertResult =
                //        productPictureRepo.Insert(productPicture);
                //    if (pictureInsertResult == 0)
                //    {
                //        TempData["ProductModalError"] = "Ürün eklendi ama ürüne ait fotoğraflar eklenirken bir hata oluştu. Fotoğraf eklemek için tekrar deneyiniz!";
                //    }
                //    else
                //    {
                //        TempData["ProductModalError"] = string.Empty;
                //    }
                //}

                int updateResult = productRepo.Update();
                if (updateResult > 0)
                {
                    return RedirectToAction("ProductList", "Product");
                }
                else
                {
                    //geçici
                    return RedirectToAction("ProductList", "Product");

                }

            }
            catch (Exception ex)
            {
                var user = MembershipTools.GetFullName();
                LogManager.LogMessage(ex.ToString(),
                    userInfo: user, pageInfo: "Product/Edit");
                //geçici
                return RedirectToAction("ProductList", "Product");
            }
        }

        public JsonResult GetProductDetails(int id)
        {
            var product = productRepo.GetById(id);
            if (product != null)
            {
                var data = product.Adapt<ProductViewModel>();
                return Json(new { isSuccess=true, data }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
    }
}