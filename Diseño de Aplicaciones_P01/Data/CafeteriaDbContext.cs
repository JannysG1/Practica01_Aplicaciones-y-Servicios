using Diseño_de_Aplicaciones_P01.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Diseño_de_Aplicaciones_P01.Data;

public class CafeteriaDbContext : DbContext
{
    public CafeteriaDbContext(DbContextOptions<CafeteriaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Producto> Productos => Set<Producto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Productos");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id)
                .ValueGeneratedOnAdd();
            entity.Property(p => p.Nombre)
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(p => p.Precio)
                .HasPrecision(18, 2)
                .IsRequired();
            entity.Property(p => p.Activo)
                .IsRequired();
        });
    }
}

public class CafeteriaDbContextFactory : IDesignTimeDbContextFactory<CafeteriaDbContext>
{
    public CafeteriaDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CafeteriaDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=CafeteriaDb;Trusted_Connection=True;TrustServerCertificate=True");
        return new CafeteriaDbContext(optionsBuilder.Options);
    }
}
