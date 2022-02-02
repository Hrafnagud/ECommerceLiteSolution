using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceLiteEntity.Models;

namespace ECommerceLiteEntity.ViewModels
{
    public class ProductCountModel
    {
        public Category BaseCategory { get; set; }
        public int ProductCount { get; set; }
    }
}
