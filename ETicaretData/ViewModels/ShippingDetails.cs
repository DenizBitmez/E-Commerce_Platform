using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretData.ViewModels
{
    public class ShippingDetails
    {
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string ExpireMonth { get; set; }
        public string ExpireYear { get; set; }
        public string Cvc { get; set; }
        public decimal? TotalAmount { get; set; }
        [Required]
        public string UserName { get; set; } // Kullanıcı adı

        [Required]
        public string Email { get; set; } // E-posta

        [Required]
        public string AddressTitle { get; set; } // Adres başlığı (örn: Ev Adresi, İş Adresi)

        [Required]
        public string Address { get; set; } // Adres

        [Required]
        public string City { get; set; } // Şehir

        [Required]
        public string Country { get; set; } // Ülke

        [Required]
        public string ZipCode { get; set; } // Posta kodu

        public string ShippingInfo { get; set; } // Kargo bilgisi (örn: Standart Kargo, Hızlı Kargo)

        public string Name { get; set; }
    }
}
