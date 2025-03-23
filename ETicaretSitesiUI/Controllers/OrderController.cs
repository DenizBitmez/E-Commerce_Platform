using ETicaretDal.Abstract;
using ETicaretData.Context;
using ETicaretData.Entities;
using ETicaretData.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
using System.Text.Json;

namespace ETicaretSitesiUI.Controllers
{
	public class OrderController : Controller
	{
		private readonly IOrderDal _dal;
		private readonly ETicaretContext _context;

		public OrderController(IOrderDal dal, ETicaretContext context)
		{
			_dal = dal;
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			var orders = await _context.Orders
				.Include(o => o.OrderLines)
				.ToListAsync();

			var orderItems = orders.Select(o => new OrderViewModel
			{
				OrderId = o.Id,
				OrderNumber = o.OrderNumber,
				OrderDate = o.OrderDate,
				Total = o.Total,
				AddressTitle = o.AddressTitle,
				City = o.City,
				orderState = o.orderState,
				Address = o.Address
			}).ToList();

			return View(orderItems);

		}
        [HttpGet]
        public async Task<IActionResult> UpdateOrderStatus(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            // Order entity'sini OrderViewModel'e dönüştür
            var orderViewModel = new OrderViewModel
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Total = order.Total,
                AddressTitle = order.AddressTitle,
                Address = order.Address,
                City = order.City,
                orderState = order.orderState
            };

            // Enum değerlerini dropdown için hazırla
            ViewBag.OrderStatusList = Enum.GetValues(typeof(OrderState))
                                          .Cast<OrderState>()
                                          .Select(e => new SelectListItem
                                          {
                                              Value = e.ToString(),
                                              Text = e.ToString()
                                          })
                                          .ToList();

            return View(orderViewModel); // OrderViewModel'i view'a gönder
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(OrderViewModel model)
        {
                var order = await _context.Orders.FindAsync(model.OrderId);
                if (order == null)
                {
                    return NotFound();
                }

                // OrderViewModel'den Order entity'sine verileri aktar
                order.orderState = model.orderState;

                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            
        }

    }
}
