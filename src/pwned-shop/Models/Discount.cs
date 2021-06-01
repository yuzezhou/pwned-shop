using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pwned_shop.Models
{
    public class Discount
    {
        [Key]
        [Required]
        [MaxLength(8)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string PromoCode { get; set; }
        [Required]
        public float DiscountPercent { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [MaxLength(20)]
        public string Remarks { get; set; }

        public virtual ICollection<Order>Orders { get; set; }
    }
}
