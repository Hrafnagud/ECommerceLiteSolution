using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceLiteEntity.Models
{
    [Table("ProductPictures")]
    public class ProductPicture : TheBase<int>
    {
        public int ProductId { get; set; }
        [StringLength(400, ErrorMessage = "Image attached to product can be utmost 400 characters long!")]
        public string ProductPicture1 { get; set; }
        [StringLength(400, ErrorMessage = "Image attached to product can be utmost 400 characters long!")]

        public string ProductPicture2 { get; set; }
        [StringLength(400, ErrorMessage = "Image attached to product can be utmost 400 characters long!")]

        public string ProductPicture3 { get; set; }
        [StringLength(400, ErrorMessage = "Image attached to product can be utmost 400 characters long!")]

        public string ProductPicture4 { get; set; }
        [StringLength(400, ErrorMessage = "Image attached to product can be utmost 400 characters long!")]

        public string ProductPicture5 { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }

}
