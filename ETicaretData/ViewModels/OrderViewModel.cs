using ETicaretData.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ETicaretData.ViewModels
{
    public class OrderViewModel
    { 
        public List<OrderLineViewModel>? OrderLines { get; set; }
        public OrderState orderState { get; set; }
        public string? AddressTitle { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }

        public string? OrderNumber { get; set; }
        public decimal Total {  get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public string? ShippingInfo { get; set; }

    }
}
