using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pwned_shop.BindingModels
{
    public class CartUpdate
    {
        public int ProductId { get; set; }
        public int Qty { get; set; }
    }
}
