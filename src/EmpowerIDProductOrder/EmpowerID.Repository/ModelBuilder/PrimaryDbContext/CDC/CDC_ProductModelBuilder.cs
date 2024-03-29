using EmpowerID.Common.Enums;
using EmpowerID.Models.CDC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EmpowerID.Repository
{
    public partial class EmpowerIDDBContext
    {
        public DbSet<CDC_Product> CDC_Products { set; get; }

        readonly ValueConverter dataStatusValueConverter = new DataStatusValueConverter();

        private void CDC_ProductModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CDC_Product>().HasKey(w => w.Id);
            modelBuilder.Entity<CDC_Product>().Property(o => o.Name).IsRequired();
            modelBuilder.Entity<CDC_Product>().Property(o => o.CategoryId).IsRequired();
            modelBuilder.Entity<CDC_Product>().Property(o => o.Description).HasColumnType("nvarchar(MAX)");

            modelBuilder.Entity<CDC_Product>().Property(o => o.DateAddded).HasColumnType("datetime");
            modelBuilder.Entity<CDC_Product>().Property(o => o.Price).HasColumnType("decimal(18,4)");

            modelBuilder.Entity<CDC_Product>().Property(p => p.DataStatus).HasConversion(dataStatusValueConverter);
        }
    }
}
