using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretData.ViewModels
{
    public enum OrderState
    {
        //[Display(Name ="Waiting")]
        //Waiting,
        //[Display(Name ="Completed")]
        //Completed
        Pending, // Ödemesi tamamlanmamış
        Completed, // Ödemesi tamamlanmış
        Shipped, // Kargoya verildi
        Delivered, // Teslim edildi
        Cancelled

    }
}
