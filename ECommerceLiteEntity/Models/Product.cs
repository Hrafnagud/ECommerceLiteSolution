using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceLiteEntity.Models
{
    [Table("Products")]
    public class Product : TheBase<int>
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Product name must contain 2-50 characters.")]
        public string ProductName { get; set; }

        [StringLength(500, ErrorMessage = "Product description must contain utmost 500 characters")]
        public string Description { get; set; }

        [StringLength(8, ErrorMessage = "Product code can be 8 digit/character long!")]
        [Index(IsUnique = true)]
        public string ProductCode { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price{ get; set; }

        [Required]
        public int Quantity { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]

        public virtual Category Category { get; set; }
        public virtual List<ProductPicture> ProductPictureList { get; set; }

    }
}
