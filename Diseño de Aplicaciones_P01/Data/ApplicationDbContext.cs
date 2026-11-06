using Diseño_de_Aplicaciones_P01.model;
using Microsoft.EntityFrameworkCore;

namespace Diseño_de_Aplicaciones_P01.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(usuario => usuario.IdUsuario);
            entity.Property(usuario => usuario.IdUsuario)
                .ValueGeneratedOnAdd();
            entity.Property(usuario => usuario.NombreUsuario)
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(usuario => usuario.CorreoElectronicousuario)
                .HasMaxLength(150)
                .IsRequired();
            entity.Property(usuario => usuario.Contrasenausuario)
                .HasMaxLength(255)
                .IsRequired();
            entity.Property(usuario => usuario.Activo)
                .IsRequired();
        });
    }
}
