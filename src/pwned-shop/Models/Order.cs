using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pwned_shop.Models
{
    public class Order
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
        [MaxLength(8)]
        public string PromoCode { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public virtual Discount Discount { get; set; }

        public virtual User User { get; set; }

    }
}
