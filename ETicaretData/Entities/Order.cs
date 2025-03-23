using ETicaretData.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretData.Entities
{
    public class Order
    {
        public int Id { get; set; }
        [Required]
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public OrderState orderState { get; set; }
        [Required]
        public string UserName { get; set; }
        public string? AddressTitle { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        //public int EmployeeId { get; set; }
        //public Employee Employee { get; set; }
        public bool? IsActive { get; set; }

        public string ?ShippingInfo { get; set; }
        public virtual List<OrderLine> OrderLines { get; set; }
    }
}
