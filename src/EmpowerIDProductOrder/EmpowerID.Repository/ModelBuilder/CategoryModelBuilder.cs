using EmpowerID.Models;
using Microsoft.EntityFrameworkCore;

namespace EmpowerID.Repository
{
    public partial class EmpowerIDDBContext
    {
        public DbSet<Category> Categories { set; get; }
        private void CategoryModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasKey(w => w.Id);
            modelBuilder.Entity<Category>().Property(w => w.Name).IsRequired();
            modelBuilder.Entity<Category>().HasIndex(w => w.Name).IsUnique().HasDatabaseName("CategoryName_Unique_NonClustered_Index");

            modelBuilder.Entity<Category>().HasMany(p => p.Products).WithOne(p => p.Category);
        }
    }
}
