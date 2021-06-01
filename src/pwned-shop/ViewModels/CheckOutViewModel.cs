using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pwned_shop.ViewModels
{
    public class CheckOutViewModel
    {
        public string ImgURL { get; set; }
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public string ActivationCode { get; set; }
        public int Qty { get; set; }
        public float UnitPrice { get; set; }
        public float Discount { get; set; }
    }
}
