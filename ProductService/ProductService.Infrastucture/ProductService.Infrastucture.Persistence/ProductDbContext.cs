﻿using Microsoft.EntityFrameworkCore;
using ProductService.Core.Domain.Models;

namespace ProductService.Infrastucture.Persistence
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductDbContext).Assembly);
        }
    }
}
