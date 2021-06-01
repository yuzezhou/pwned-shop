using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace pwned_shop.Models
{
    public class Review
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        [MaxLength(5)]
        public int ProductId { get; set; }
        [Required]
        public DateTime ReviewDate { get; set; }
        [Required]
        [MaxLength(200)]
        public string Content { get; set; }
        [Required]
        public int StarAssigned { get; set; }

        public virtual User User { get; set; }
        public virtual Product Product { get; set; }
    }
}
