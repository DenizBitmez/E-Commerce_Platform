using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretData.ViewModels
{
    public class DashboardViewModel
    {
        public List<CategorySalesViewModel> CategorySales { get; set; }
        public List<DailySalesViewModel> Last7DaysSales { get; set; }
        public List<TopSellingProductsViewModel> TopSellingProducts { get; set; }

        public List<StockStatusViewModel> StockStatus { get; set; }
    }
}
