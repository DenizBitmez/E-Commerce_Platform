using ETicaretBusiness.Concreate;
using ETicaretDal.Abstract;
using ETicaretData.Context;
using ETicaretData.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretDal.Concreate
{
    public class ProductDal : GenericRepository<Product, ETicaretContext>, IProductDal
    {
    }
}
