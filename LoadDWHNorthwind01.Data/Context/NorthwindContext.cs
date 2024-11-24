using LoadDWHNorthwind01.Data.Entities.Northwind;
using Microsoft.EntityFrameworkCore;

namespace LoadDWHNorthwind01.Data.Context
{
    public partial class NorthwindContext : DbContext
    {
        public NorthwindContext(DbContextOptions<NorthwindContext> options) : base(options)
        {
        }
        #region"Db Sets"

        public DbSet<Categories> Categories { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Shippers> Shippers { get; set; }
        #endregion

    }
}
