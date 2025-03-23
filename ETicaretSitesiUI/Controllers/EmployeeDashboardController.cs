using ETicaretData.Context;
using ETicaretData.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETicaretSitesiUI.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeDashboardController : Controller
    {
        private readonly ETicaretContext _context;

        public EmployeeDashboardController(ETicaretContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Select(o => new OrderViewModel
                {
                    OrderId = o.Id,
                    OrderDate = o.OrderDate,
                    Total = o.Total,
                    orderState = o.orderState
                })
                .ToListAsync();

            return View(orders);
        }

        // Kategorileri listeleme
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryItem
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToListAsync();

            return View(categories);
        }
    }
}