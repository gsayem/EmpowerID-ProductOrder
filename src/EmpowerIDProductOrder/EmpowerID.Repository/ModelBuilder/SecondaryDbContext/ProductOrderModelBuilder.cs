//using EmpowerID.Models;
//using Microsoft.EntityFrameworkCore;

//namespace EmpowerID.Repository
//{
//    public partial class SecondaryEmpowerIDDBContext
//    {
//        public DbSet<ProductOrder> ProductOrders { set; get; }
//        private void ProductOrderModelBuilder(ModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<ProductOrder>().HasKey(w => w.Id);
//            modelBuilder.Entity<ProductOrder>().Property(p => p.ProductId).IsRequired();
//            modelBuilder.Entity<ProductOrder>().Property(p => p.OrderId).IsRequired();

//            modelBuilder.Entity<ProductOrder>().HasOne(p => p.Product).WithMany(p => p.ProductOrders);
//            modelBuilder.Entity<ProductOrder>().HasOne(p => p.Order).WithMany(p => p.ProductOrders);
//        }
//    }
//}
