﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretData.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; } 
        public DateTime Date { get; set; }
        public Product Product { get; set; }
    }
}
