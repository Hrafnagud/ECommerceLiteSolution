using ECommerceBusinessLogicLayer.Repository;
using ECommerceLiteEntity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ECommerceLiteUI.Models
{
    public class ProductViewModel
    {
        CategoryRepo categoryRepo= new CategoryRepo();
        ProductPictureRepo productPictureRepo = new ProductPictureRepo();
        public int Id { get; set; }
        public DateTime RegisterDate { get; set; } = DateTime.Now;
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Length of product name can have charachters between 2-50!(both included)")]
        public string ProductName { get; set; }
        [StringLength(500, ErrorMessage = "Product Description can have utmost 500 characters!")]
        public string Description { get; set; }

        [StringLength(8, ErrorMessage = "Product code can have utmost 8 characters/digits!")]
        public string ProductCode { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public int CategoryId { get; set; }

        private Category _category = new Category();
        public Category CategoryOfProduct { get; set; }


        public List<ProductPicture> PicturesOfProduct { get; set; }

        public List<string> ProductPicturesList { get; set; } = new List<string>();
        public List<HttpPostedFileBase> Files { get; set; } = new List<HttpPostedFileBase>();

        public void SetCategory()
        {
            if (CategoryId > 0)
            {
                CategoryOfProduct = categoryRepo.GetById(CategoryId);
            }
        }

        public void SetProductPictures()
        {
            if (Id > 0)
            {
                PicturesOfProduct = productPictureRepo.Queryable().Where(x => x.ProductId == Id).ToList();
            }
        }

    }

}
