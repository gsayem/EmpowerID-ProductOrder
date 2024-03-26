using EmpowerID.Models;
using Microsoft.EntityFrameworkCore;

namespace EmpowerID.Repository
{
    public partial class EmpowerIDDBContext
    {
        public DbSet<Order> Orders { set; get; }
        private void OrderModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasKey(w => w.Id);

            modelBuilder.Entity<Order>().Property(o => o.OrderDate).HasColumnType("datetime");


            modelBuilder.Entity<Order>().HasIndex(w => w.OrderDate).HasDatabaseName("OrderDate_NonClustered_Index");

            modelBuilder.Entity<Order>().HasMany(p => p.ProductOrders).WithOne(p => p.Order);
        }
    }
}
