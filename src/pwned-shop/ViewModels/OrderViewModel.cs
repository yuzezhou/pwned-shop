using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pwned_shop.ViewModels
{
    public class OrderViewModel
    {
        public string ImgURL { get; set; }
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public DateTime Timestamp { get; set; }
        public string ActivationCode { get; set; }
    }
    
}
