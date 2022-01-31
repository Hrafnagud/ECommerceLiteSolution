using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ECommerceLiteUI.Models
{
    public class CartViewModel
    {
        public int Id { get; set; }
        public DateTime RegisterDate { get; set; } = DateTime.Now;
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Length of product name can have characters between 2-50 (Both Included)!")]
        public string ProductName { get; set; }
        [StringLength(500, ErrorMessage = "Product description can contain utmost 500 characters!")]
        public string Description { get; set; }

        [StringLength(8, ErrorMessage = "Product code contains utmost 8 digits!")]
        public string ProductCode { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public int CategoryId { get; set; }

    }
}