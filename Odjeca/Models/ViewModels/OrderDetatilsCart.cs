using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odjeca.Models.ViewModels
{
    public class OrderDetatilsCart
    {
        public List<ShoppingCart> listCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
