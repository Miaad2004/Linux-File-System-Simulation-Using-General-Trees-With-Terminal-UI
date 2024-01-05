using FileSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem.Data
{
    public class LinuxDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }

        private readonly IConfiguration _configuration;

        public LinuxDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

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
