using ETicaretBusiness.Concreate;
using ETicaretDal.Abstract;
using ETicaretData.Context;
using ETicaretData.Entities;
using ETicaretData.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretDal.Concreate
{
    public class ReviewDal : GenericRepository<Review, ETicaretContext>, IReviewDal
    {
        private readonly ETicaretContext _context;

        public ReviewDal(ETicaretContext context)
        {
            _context = context;
        }

        public void AddReview(ReviewViewModel review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
        }

        public IEnumerable<ReviewViewModel> GetReviewsForProduct(int productId)
        {
            var reviews = _context.Reviews
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.Date)
                .ToList();

            // Review nesnelerini ReviewViewModel nesnelerine dönüştürme
            var reviewViewModels = reviews.Select(r => new ReviewViewModel
            {
                Id = r.Id,
                ProductId = r.ProductId,
                UserName = r.UserName,
                Comment = r.Comment,
                Rating = r.Rating,
                Date = r.Date
            }).ToList();

            return reviewViewModels;
        }
    }
}
