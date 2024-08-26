using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RMS.Models;

namespace RMS.Data
{
    public class GroceryStoreContext : DbContext
    {
        public GroceryStoreContext (DbContextOptions<GroceryStoreContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; } = default!;
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductCategories> ProductCategories { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    }
}
