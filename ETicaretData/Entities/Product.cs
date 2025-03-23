using ETicaretData.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretData.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Image { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public bool IsHome { get; set; }//Anasayfa 
        public bool IsApproved { get; set; }//satışta mı
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public ICollection<ReviewViewModel> Reviews { get; set; }
        public Product()
        {
        }

    }
}
