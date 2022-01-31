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

namespace ECommerceLiteUI.Controllers
{
    public class ProductController : Controller
    {

        ProductRepo productRepo = new ProductRepo();
        CategoryRepo categoryRepo = new CategoryRepo();
        ProductPictureRepo productPictureRepo = new ProductPictureRepo();

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
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductViewModel model)
        {
            try
            {
                List<SelectListItem> allCategories = new List<SelectListItem>();
                categoryRepo.GetAll().ToList().ForEach(x => allCategories.Add(new SelectListItem()
                {
                    Text = x.CategoryName,
                    Value = x.Id.ToString()
                }));
                ViewBag.CategoryList = allCategories;

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
                //ex log
                return View(model);
            }
        }
    }
}