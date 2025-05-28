using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EruMobil.Persistence.Context
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // 1. appsettings.json dosyasını oku
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Migration komutunu hangi klasörde çalıştırırsan orası
                .AddJsonFile("appsettings.Development.json") // veya appsettings.Development.json
                .Build();

            // 2. Connection string'i oku
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // 3. DbContextOptions oluştur
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
