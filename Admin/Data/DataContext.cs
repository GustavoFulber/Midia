using Microsoft.EntityFrameworkCore;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    { }

    public DbSet<Arquivo> Arquivo { get; set;}

    public DbSet<Usuario> Usuario { get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Arquivo>().ToTable("arquivo", schema: "arquivos");

        modelBuilder.Entity<Usuario>().ToTable("usuario", schema: "arquivos");
    }
}