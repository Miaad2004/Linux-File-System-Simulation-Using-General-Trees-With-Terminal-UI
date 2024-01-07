using FileSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace FileSystem.Data
{
    public class LinuxDbContext(IConfiguration configuration) : DbContext
    {
        public DbSet<User> Users { get; set; }

        private readonly IConfiguration _configuration = configuration;

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection")
                ?? throw new ConfigurationErrorsException("Invalid Configuration");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(GetConnectionString());
        }
    }
}
