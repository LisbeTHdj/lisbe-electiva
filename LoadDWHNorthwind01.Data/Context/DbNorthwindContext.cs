using LoadDWHNorthwind01.Data.Entities.DWHNorthwind;
using LoadDWHNorthwind01.Data.Entities.DWHNothwind;
using Microsoft.EntityFrameworkCore;

public class DbNorthwindContext : DbContext
{
    // Constructor para la configuración de DbContext
    public DbNorthwindContext(DbContextOptions<DbNorthwindContext> options)
        : base(options)
    { }

    #region "Db Sets"
    public DbSet<DimCategories> DimCategories { get; set; }
    public DbSet<DimEmployees> DimEmployees { get; set; }
    public DbSet<DimCustomers> DimCustomers { get; set; }
    public DbSet<DimProducts> DimProducts { get; set; }
    public DbSet<DimShippers> DimShippers { get; set; }
    #endregion

    // Método para mapear las entidades a las tablas correspondientes en la base de datos
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de la tabla DimEmployees
        modelBuilder.Entity<DimEmployees>()
            .ToTable("DimEmployees", "dbo") // Especifica la tabla y el esquema.
            .Property(e => e.EmployeeKey)
            .ValueGeneratedOnAdd(); // Configura EmployeeKey como una columna de identidad.
    }

}
