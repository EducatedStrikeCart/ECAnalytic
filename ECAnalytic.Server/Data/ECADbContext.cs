using ECAnalytic.Server.Controllers;
using ECAnalytic.Server.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql;

namespace ECAnalytic.Server.Data
{
	public class ECADbContext : DbContext
	{
		private readonly IConfiguration _config;
		public ECADbContext(IConfiguration config)
		{
			_config = config;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var serverVersion = new MySqlServerVersion(new Version(8, 0, 42));
			var connectionString = _config["ConnectionString"] ?? throw new Exception("Connection string not set up in User Secrets");
			optionsBuilder.UseMySql(
				connectionString,
				serverVersion,
				options => options.EnableRetryOnFailure());
		}

		// virtual needed for testing
		public virtual DbSet<Order> Orders { get; set; }
	}
}
