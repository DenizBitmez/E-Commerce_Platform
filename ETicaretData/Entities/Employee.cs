using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretData.Entities
{
    public class Employee
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
        public string Position { get; set; } // Örneğin: "Manager", "Sales", "Support"

        public DateTime HireDate { get; set; } // İşe başlama tarihi

        public string Address { get; set; }

        public string PhoneNumber { get; set; }
    }

}

