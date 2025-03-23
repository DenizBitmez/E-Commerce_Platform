using ETicaretData.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretData.ViewModels
{
   public class ListViewModel
    {
        public ListViewModel()
        {
            Products = new List<Product>();
        }
        public List<Product>Products { get; set; }
        public List<Category>Categories { get; set; }
    }
}
