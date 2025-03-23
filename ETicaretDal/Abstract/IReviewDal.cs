using ETicaretBusiness.Abstract;
using ETicaretData.Entities;
using ETicaretData.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretDal.Abstract
{
    public interface IReviewDal : IGenericRepository<Review>
    {
        //void AddReview(Review review);
        IEnumerable<ReviewViewModel> GetReviewsForProduct(int productId);
    }
}
