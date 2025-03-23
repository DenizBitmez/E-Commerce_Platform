using ETicaretData.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretData.ViewModels
{
    public class OrderLineViewModel
    {
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public int Id { get; set; }
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
