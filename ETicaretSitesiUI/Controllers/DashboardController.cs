using ETicaretData.Context;
using ETicaretData.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETicaretSitesiUI.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ETicaretContext _context;

        public DashboardController(ETicaretContext context)
        {
            _context = context;
        }

        public async Task< IActionResult> Index()
        {
            var categorySales = await GetCategorySalesAsync();
            var last7DaysSales = await GetLast7DaysSalesAsync();
            var topSellingProducts = await GetTopSellingProductsAsync();
            var stockStatus = await GetStockStatusAsync();

            var viewModel = new DashboardViewModel
            {
                CategorySales = categorySales,
                Last7DaysSales = last7DaysSales,
                TopSellingProducts = topSellingProducts,
                StockStatus = stockStatus
            };

            return View(viewModel);
        }

        private async Task<List<CategorySalesViewModel>> GetCategorySalesAsync()
        {
            var categorySales = await _context.OrderLines
                .Include(ol => ol.Product)
                .ThenInclude(p => p.Category)
                .GroupBy(ol => ol.Product.Category.Name)
                .Select(g => new CategorySalesViewModel
                {
                    CategoryName = g.Key,
                    TotalSales = g.Sum(ol => ol.Price * ol.Quantity)
                })
                .ToListAsync();

            return categorySales;
        }

        // Son 7 günün satışlarını getir
        private async Task<List<DailySalesViewModel>> GetLast7DaysSalesAsync()
        {
            var last7DaysSales = await _context.Orders
                .Where(o => o.OrderDate >= DateTime.Now.AddDays(-7))
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new DailySalesViewModel
                {
                    Date = g.Key,
                    TotalSales = g.Sum(o => o.Total)
                })
                .ToListAsync();

            return last7DaysSales;
        }

        // En çok satılan ürünleri getir
        private async Task<List<TopSellingProductsViewModel>> GetTopSellingProductsAsync()
        {
            var topSellingProducts = await _context.OrderLines
                .Include(ol => ol.Product)
                .GroupBy(ol => ol.Product.Name)
                .Select(g => new TopSellingProductsViewModel
                {
                    ProductName = g.Key,
                    SalesCount = g.Sum(ol => ol.Quantity)
                })
                .OrderByDescending(p => p.SalesCount)
                .Take(5)
                .ToListAsync();

            return topSellingProducts;
        }

        // Stok durumunu getir
        private async Task<List<StockStatusViewModel>> GetStockStatusAsync()
        {
            var stockStatus = await _context.Products
                .Where(p => p.Stock > 0) // Sadece stokta olan ürünleri getir
                .OrderByDescending(p => p.Stock)
                .Take(10) // En fazla stokta olan 10 ürünü getir
                .Select(p => new StockStatusViewModel
                {
                    ProductName = p.Name,
                    Stock = p.Stock
                })
                .ToListAsync();

            return stockStatus;
        }
    }
}
