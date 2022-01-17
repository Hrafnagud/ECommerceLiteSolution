using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceLiteEntity.Models
{
    [Table("Orders")]
    public class Order : TheBase<int>
    {
        [Required]
        [StringLength(7,ErrorMessage = "Order number is required")]
        public string OrderNumber { get; set; }
        public string CustomerTRID { get; set; }
        [ForeignKey("CustomerTRID")]
        public virtual Customer Customer { get; set; }
        public virtual List<OrderDetail> OrderDetailList { get; set; }
    }
}
