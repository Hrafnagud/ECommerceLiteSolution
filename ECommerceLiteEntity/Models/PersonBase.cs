using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceLiteEntity.Models
{
    public class PersonBase : IPerson
    {
        [Key]
        [Column(Order = 1)]
        [MinLength(11)]
        [StringLength(11, ErrorMessage = "TR ID must contain 11 digits")]
        public string TRID { get; set; }

        public DateTime LastActiveTime { get; set; }
    }
}
