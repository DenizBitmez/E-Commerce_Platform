using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretData.ViewModels
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Position { get; set; }

        [Display(Name = "İşe Başlama Tarihi")]
        public DateTime HireDate { get; set; }

        public string Address { get; set; }

        [Display(Name = "Telefon Numarası")]
        public string PhoneNumber { get; set; }

    }
}
