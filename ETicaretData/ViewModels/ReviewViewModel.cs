using ETicaretData.Context;
using ETicaretData.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretData.ViewModels
{
    public class ReviewViewModel
    {
        private readonly ETicaretContext _context;

        public ReviewViewModel()
        {
        }

        public int Id { get; set; }
        public int ProductId { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }  
        public string Comment { get; set; }  
        public int Rating { get; set; }
        public DateTime Date { get; set; }

    }
}
