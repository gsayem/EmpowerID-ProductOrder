using EmpowerID.Models.CDC;
using Microsoft.EntityFrameworkCore;

namespace EmpowerID.Repository
{
    public partial class EmpowerIDDBContext
    {
        public DbSet<CDC_Order> CDC_Orders { set; get; }
        private void CDC_OrderModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CDC_Order>().HasKey(w => w.Id);
            modelBuilder.Entity<CDC_Order>().Property(p => p.DataStatus).HasConversion(dataStatusValueConverter);

        }
    }
}
