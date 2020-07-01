using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odjeca.Models.ViewModels
{
    public class StoreViewModel
    {
        public IEnumerable<StoreItem> StoreItem { get; set; }
        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<Discount> Discount { get; set; }
        public IEnumerable<Brands> Brands { get; set; }
    }
}
