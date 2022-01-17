using ECommerceLiteEntity.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceLiteEntity.IdentityModels
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(maximumLength:25,MinimumLength = 2, ErrorMessage = "Name can consist 2-25 characters!")]
        [Display(Name="Name")]
        [Required]
        public string Name { get; set; }

        [StringLength(maximumLength: 25, MinimumLength = 2, ErrorMessage = "Surname can consist 2-25 characters!")]
        [Display(Name = "Surname")]
        [Required]
        public string Surname { get; set; }

        [Display(Name = "Register Date")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime RegisterDate { get; set; } = DateTime.Now;
        public string ActivationCode { get; set; }

        public virtual List<Customer> CustomerList { get; set; }
        public virtual List<Admin> AdminList{ get; set; }
        public virtual List<PassiveUser> PassiveUserList{ get; set; }


    }
}
