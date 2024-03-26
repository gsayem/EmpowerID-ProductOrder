using EmpowerID.Models;
using Microsoft.EntityFrameworkCore;

namespace EmpowerID.Repository
{
    public partial class EmpowerIDDBContext
    {
        public DbSet<Product> Products { set; get; }
        private void ProductModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasKey(w => w.Id);
            modelBuilder.Entity<Product>().Property(o => o.Name).IsRequired();
            modelBuilder.Entity<Product>().Property(o => o.CategoryId).IsRequired();
            modelBuilder.Entity<Product>().Property(o => o.Description).HasColumnType("nvarchar(MAX)");

            modelBuilder.Entity<Product>().Property(o => o.DateAddded).HasColumnType("datetime");
            modelBuilder.Entity<Product>().Property(o => o.Price).HasColumnType("decimal(18,4)");

            modelBuilder.Entity<Product>().HasOne(p => p.Category).WithMany(p => p.Products);
            modelBuilder.Entity<Product>().HasMany(p => p.ProductOrders).WithOne(p => p.Product);

            modelBuilder.Entity<Product>().HasIndex(w => w.Name).IsUnique().HasDatabaseName("ProductName_Unique_NonClustered_Index");
            modelBuilder.Entity<Product>().HasIndex(w => w.Price).HasDatabaseName("ProductPrice_Non_Clustered_Index");
            //modelBuilder.Entity<Product>().HasIndex(w => w.Description).HasDatabaseName("ProductDescription_Non_Clustered_Index");
            modelBuilder.Entity<Product>().HasIndex(w => w.DateAddded).HasDatabaseName("ProductDateAddded_Non_Clustered_Index");
        }
    }
}
