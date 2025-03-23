using ETicaretDal.Abstract;
using ETicaretData.Context;
using ETicaretData.Entities;
using ETicaretData.Helpers;
using ETicaretData.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Data;

namespace ETicaretSitesiUI.Controllers
{
    [Authorize(Roles = "user")]
    public class CardController : Controller
    {
        private readonly IOrderDal _orderDal;
        private readonly IProductDal _productDal;
        private readonly ETicaretContext _context;

        public CardController(IOrderDal orderDal, IProductDal productDal, ETicaretContext context)
        {
            _orderDal = orderDal;
            _productDal = productDal;
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            var card = SesionHelper.GetObjectFromJson<List<CardItem>>
                (HttpContext.Session, "Card");
            if (card == null)
            {
                return View();
            }
            ViewBag.Total = card.Sum(x => x.Product.Price * x.Quantity).ToString("c");
            SesionHelper.Count = card.Count();
            return View(card);


        }
        public IActionResult Buy(int id)
        {
            if (SesionHelper.GetObjectFromJson<List<CardItem>>(HttpContext.Session, "Card") == null)
            {
                var cart = new List<CardItem>();
                cart.Add(new CardItem { Product = _productDal.Get(id), Quantity = 1 });
                SesionHelper.SetObjectAsJson(HttpContext.Session, "Card", cart);
            }
            else
            {
                var cart = SesionHelper.GetObjectFromJson<List<CardItem>>(HttpContext.Session, "Card");
                //
                int index = isExists(cart, id);
                if (index < 0)
                {
                    cart.Add(new CardItem { Product = _productDal.Get(id), Quantity = 1 });
                }
                else
                {
                    cart[index].Quantity++;
                }
                SesionHelper.SetObjectAsJson(HttpContext.Session, "Card", cart);
            }
            return RedirectToAction("Index");
        }
        public IActionResult CheckOut()
        {
            return View(new ShippingDetails());
        }

        [HttpPost]
        public IActionResult CheckOut(ShippingDetails details, string paymentMethodId)
        {
            var Cart = SesionHelper.GetObjectFromJson<List<CardItem>>(HttpContext.Session, "Card");

            if (Cart == null)
            {
                ModelState.AddModelError("Urun Yok", "Sepetinizde urun yok...");
            }

            if (ModelState.IsValid)
            {
                var paymentResult = ProcessStripePayment(Cart, details, paymentMethodId);

                if (paymentResult.Status == "succeeded")
                {
                    // Ödeme başarılı ise siparişi kaydet
                    SaveOrder(Cart, details);
                    Cart.Clear();
                    SesionHelper.SetObjectAsJson(HttpContext.Session, "Card", Cart);
                    return RedirectToAction("Success"); // Ödeme başarılı sayfasına yönlendir
                }
                else
                {
                    // Ödeme başarısız ise hata mesajını göster
                    ModelState.AddModelError("PaymentError", paymentResult.ErrorMessage);
                    return View(details);
                }
            }

            return View(details);

        }


        private PaymentResult ProcessStripePayment(List<CardItem> cart, ShippingDetails details, string paymentMethodId)
        {
            try
            {
                StripeConfiguration.ApiKey = "sk_test_51R4qkrCieUQDIHra8jr2XCrcMPJ9l1Q2UIPEtIkmK6cYJu0LCUGQLWM5a7njD3PrVR3j0hlGmeOUEWNHgkXb8mbE00MBgiDTpI";
                // Sepet toplam tutarını hesapla
                decimal totalAmount = cart.Sum(item => item.Product.Price * item.Quantity);

                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(totalAmount * 100), // Stripe amount is in cents
                    Currency = "try",
                    PaymentMethod = paymentMethodId, // Test kartı için
                    Confirm = true,
                    Description = $"Sipariş - {details.Name}",
                    ReceiptEmail = details.Email,
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                        AllowRedirects = "never" // Yönlendirme tabanlı ödeme yöntemlerini devre dışı bırak
                    },
                    Shipping = new ChargeShippingOptions
                    {
                        Name = details.UserName,
                        Address = new AddressOptions
                        {
                            Line1 = details.Address,
                            City = details.City,
                            Country = details.Country,
                            PostalCode = details.ZipCode
                        }
                    }
                };

                var service = new PaymentIntentService();
                PaymentIntent intent = service.Create(options);

                return new PaymentResult
                {
                    Status = intent.Status,
                    PaymentIntentId = intent.Id
                };
            }
            catch (StripeException ex)
            {
                return new PaymentResult
                {
                    Status = "failed",
                    ErrorMessage = ex.StripeError.Message
                };
            }
        }

        public class PaymentResult
        {
            public string Status { get; set; }
            public string PaymentIntentId { get; set; }
            public string ErrorMessage { get; set; }
        }
        [HttpPost]
        public IActionResult ProcessPayment([FromBody] PaymentRequest request)
        {
            try
            {
                if (request == null)
                {
                    return Json(new { status = "failed", error = "Geçersiz istek." });
                }

                var Cart = SesionHelper.GetObjectFromJson<List<CardItem>>(HttpContext.Session, "Card");

                if (Cart == null || !Cart.Any())
                {
                    return Json(new { status = "failed", error = "Sepetinizde ürün yok." });
                }

                // Form verilerini al
                var shippingDetails = new ShippingDetails
                {
                    Name = request.Name,
                    Email = request.Email,
                    Address = request.Address,
                    City = request.City,
                    Country = request.Country,
                    ZipCode = request.ZipCode,
                    UserName = request.UserName,
                    AddressTitle = request.AddressTitle,
                    ShippingInfo = request.ShippingInfo
                };

                var paymentResult = ProcessStripePayment(Cart, shippingDetails, request.PaymentMethodId);

                if (paymentResult.Status == "succeeded")
                {
                    try
                    {
                        SaveOrder(Cart, shippingDetails);
                        Cart.Clear();
                        SesionHelper.SetObjectAsJson(HttpContext.Session, "Card", Cart);
                        return Json(new { status = "succeeded", redirectUrl = Url.Action("Success", "Card") });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { status = "failed", error = "Sipariş kaydedilirken bir hata oluştu: " + ex.Message });
                    }
                }
                else
                {
                    return Json(new { status = "failed", error = paymentResult.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = "failed", error = "Ödeme işlemi sırasında bir hata oluştu: " + ex.Message });
            }
        }

        public class PaymentRequest
        {
            public string PaymentMethodId { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public string ZipCode { get; set; }
            public string UserName { get; set; }
            public string AddressTitle { get; set; }
            public string ShippingInfo { get; set; }
        }
        public IActionResult Success()
        {
            var lastOrder = _context.Orders
        .Include(o => o.OrderLines)
        .ThenInclude(ol => ol.Product)
        .OrderByDescending(o => o.OrderDate)
        .FirstOrDefault();

            if (lastOrder == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Order'ı OrderViewModel'e dönüştür
            var orderViewModel = new OrderViewModel
            {
                OrderId = lastOrder.Id,
                OrderNumber = lastOrder.OrderNumber,
                OrderDate = lastOrder.OrderDate,
                Total = lastOrder.Total,
                AddressTitle = lastOrder.AddressTitle,
                Address = lastOrder.Address,
                City = lastOrder.City,
                ShippingInfo = lastOrder.ShippingInfo,
                OrderLines = lastOrder.OrderLines.Select(ol => new OrderLineViewModel
                {
                    Id = ol.Id,
                    Quantity = ol.Quantity,
                    Price = ol.Price,
                    ProductName = ol.Product.Name
                }).ToList()
            };

            return View(orderViewModel);
        }

        [HttpGet]
        public ActionResult Remove(int id)
        {
            //if (id==null && ==null)
            //{
            //    return RedirectToAction("Error", "Home");
            //}
            var Cart = SesionHelper.GetObjectFromJson<List<CardItem>>(HttpContext.Session, "Card");
            int index = isExists(Cart, id);
            Cart.RemoveAt(index);
            SesionHelper.SetObjectAsJson(HttpContext.Session, "Card", Cart);
            return RedirectToAction("Index");
        }

        //public ActionResult RemoveConfirmed(int id)
        //{

        //}

        private void SaveOrder(List<CardItem> cart, ShippingDetails details)
        {

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var orderNumber = Guid.NewGuid().ToString("N");

                    var order = new Order
                    {
                        OrderNumber = orderNumber,
                        Total = cart.Sum(x => x.Product.Price * x.Quantity),
                        OrderDate = DateTime.Now,
                        orderState = OrderState.Completed,
                        UserName = details.UserName,
                        AddressTitle = details.AddressTitle,
                        Address = details.Address,
                        City = details.City,
                        ShippingInfo = details.ShippingInfo,
                        IsActive = true,
                        OrderLines = new List<OrderLine>()
                    };

                    foreach (var item in cart)
                    {
                        var orderLine = new OrderLine
                        {
                            Quantity = item.Quantity,
                            Price = item.Product.Price * item.Quantity,
                            ProductId = item.Product.Id,
                            OrderId = order.Id
                        };
                        order.OrderLines.Add(orderLine);
                    }

                    _context.Orders.Add(order);
                    _context.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Sipariş kaydedilirken bir hata oluştu: " + ex.Message);
                }
            }
        }


        private int isExists(List<CardItem> cart, int id)
        {
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.Id.Equals(id))
                {
                    return i;

                }

            }
            return -1;
        }

    }
}
