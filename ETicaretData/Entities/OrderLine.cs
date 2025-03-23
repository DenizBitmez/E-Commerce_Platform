using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretData.Entities
{
    public class OrderLine
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        [Required]
        public int OrderId { get; set; } // Null olamaz

        public virtual Order Order { get; set; } // Navigation property

        [Required]
        public int ProductId { get; set; } // Null olamaz

        public virtual Product Product { get; set; }

    }
}
