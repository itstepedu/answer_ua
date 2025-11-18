using System;
using AnswerUA.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace answer_ua.Data
{
    public class ShopDbContext : DbContext
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options)
            : base(options)
        {
        }

        public DbSet<BrandTargetCategories> BrandTargetCategories { get; set; }
        public DbSet<Brands> Brands { get; set; }
         //public DbSet<Genders> Genders { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductColors> ProductColors { get; set; }
        public DbSet<ProductSizes> ProductSizes { get; set; }
        public DbSet<ProductTypes> ProductTypes { get; set; }
        public DbSet<Subcategories> Subcategories { get; set; }
        public DbSet<TargetCategories> TargetCategories { get; set; }

        public DbSet<ProductImages> ProductImages { get; set; }
        public DbSet<TargetCategoryProductType> TargetCategoryProductType { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TargetCategoryProductType>()
                .HasKey(t => new { t.TargetCategoriesId, t.ProductTypesId });

            modelBuilder.Entity<TargetCategoryProductType>()
                .HasOne(t => t.TargetCategories)
                .WithMany(tc => tc.TargetCategoryProductType)
                .HasForeignKey(t => t.TargetCategoriesId);

            modelBuilder.Entity<TargetCategoryProductType>()
                .HasOne(t => t.ProductTypes)
                .WithMany(pt => pt.TargetCategoryProductType)
                .HasForeignKey(t => t.ProductTypesId);
        }
        // protected override void OnModelCreating(ModelBuilder builder)
        // {
        //     base.OnModelCreating(builder);

        //     var orderItems = _cont




        // }
    }
}