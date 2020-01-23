using ApiProjectModul.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProjectModul.AppDataAccessLayer
{
    public class AppDataBase : DbContext
    {
        public AppDataBase(DbContextOptions<AppDataBase> options) : base(options)
        {

        }
        public DbSet<Composition> Compositions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Composition>().HasData(
            new Composition() { Id = 1, Calories = 1000, Type = "Starter", Name = "Qanburger", Created = DateTime.Now },
            new Composition() { Id = 2, Calories = 1100, Type = "Main", Name = "Hamburger", Created = DateTime.Now },
            new Composition() { Id = 3, Calories = 1200, Type = "Dessert", Name = "Spaghetti", Created = DateTime.Now },
            new Composition() { Id = 4, Calories = 1500, Type = "Starter", Name = "Pizza", Created = DateTime.Now },
            new Composition() { Id = 5, Calories = 2000, Type = "Doner", Name = "Doner", Created = DateTime.Now },
            new Composition() { Id = 6, Calories = 2500, Type = "Sushi", Name = "Sushi", Created = DateTime.Now },
            new Composition() { Id = 7, Calories = 3000, Type = "Tonbalik", Name = "Tonbalik", Created = DateTime.Now },
            new Composition() { Id = 8, Calories = 2000, Type = "Qutab", Name = "Qutab", Created = DateTime.Now },
            new Composition() { Id = 9, Calories = 2500, Type = "Perashki", Name = "Perashki", Created = DateTime.Now },
            new Composition() { Id = 10, Calories = 1500, Type = "Corekarasi", Name = "Corekarasi", Created = DateTime.Now },
            new Composition() { Id = 11, Calories = 2500, Type = "Qogal", Name = "Qogal", Created = DateTime.Now },
            new Composition() { Id = 12, Calories = 1000, Type = "Kasbmali", Name = "Kasbmali", Created = DateTime.Now }
                );
        }
    }
}
