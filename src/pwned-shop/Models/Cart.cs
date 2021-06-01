using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pwned_shop.Models
{
    public class Cart
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Qty { get; set; }

        public virtual User User { get; set; }
        public virtual Product Product { get; set; }
    }
}
