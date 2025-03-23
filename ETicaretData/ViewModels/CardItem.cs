using ETicaretData.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretData.ViewModels
{
    public class CardItem
    {
        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
