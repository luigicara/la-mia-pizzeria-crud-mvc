using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using la_mia_pizzeria_static.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace la_mia_pizzeria_static
{
    public class PizzaContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Pizza> Pizzas { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Ingredient> Ingredients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=db_pizzas;Integrated Security=True;TrustServerCertificate=true");
        }

        public PizzaContext() : base() { }

        public PizzaContext(DbContextOptions<PizzaContext> dbContextOptions) : base(dbContextOptions) { }
    }
}
