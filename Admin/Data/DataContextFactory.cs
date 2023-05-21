using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

        return new DataContext(optionsBuilder.Options);
    }
}