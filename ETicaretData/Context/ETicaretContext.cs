using ETicaretData.Entities;
using ETicaretData.Identity;
using ETicaretData.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretData.Context
{
    public class ETicaretContext : IdentityDbContext<AppUser, AppRole, int>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server =DENIZBITMEZ\\SQLEXPRESS; Database = CoreData; Trusted_Connection = True;").LogTo(Console.WriteLine, LogLevel.Information);
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<ReviewViewModel> Reviews { get; set; }
        public DbSet<Employee> Employees { get; set; }
    }
}
