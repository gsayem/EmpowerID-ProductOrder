using EmpowerID.Models.CDC;
using Microsoft.EntityFrameworkCore;

namespace EmpowerID.Repository
{
    public partial class EmpowerIDDBContext
    {
        public DbSet<CDC_Category> CDC_Categories { set; get; }
        private void CDC_CategoryModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CDC_Category>().HasKey(w => w.Id);
            modelBuilder.Entity<CDC_Category>().Property(w => w.Name).IsRequired();
            modelBuilder.Entity<CDC_Category>().Property(p => p.DataStatus).HasConversion(dataStatusValueConverter);

        }
    }
}
