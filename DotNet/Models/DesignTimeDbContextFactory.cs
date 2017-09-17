using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Models
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<WorldContext>
    {
        public WorldContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();

            var builder = new DbContextOptionsBuilder<WorldContext>();

            var connectionString = configuration.GetConnectionString("WorldContextConnection");

            builder.UseSqlServer(connectionString);

            return new WorldContext(configuration, builder.Options);
        }
    }
}
