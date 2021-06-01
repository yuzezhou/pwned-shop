using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace pwned_shop.Models
{
    public class OrderDetail
    {
        [Required]
        public string OrderId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Key]
        [Required]
        [MaxLength(36)]
        public string ActivationCode { get; set; }
        [MaxLength(40)]
        public string GiftTo { get; set; }

        public virtual Product Product { get; set; }
        public virtual Order Order { get; set; }
    }
}
