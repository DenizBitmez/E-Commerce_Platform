using ETicaretData.Context;
using ETicaretData.Entities;
using ETicaretData.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETicaretSitesiUI.Controllers
{
    public class OrderLineController : Controller
    {
        private readonly ETicaretContext _context;

        public OrderLineController(ETicaretContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var orderLines = _context.OrderLines.Select(ol => new OrderLineViewModel
            {
                ProductName =ol.Product.Name,
                Quantity = ol.Quantity,
                Price = ol.Price
            }).ToList();

            return View(orderLines);

        }

      
    }
}
