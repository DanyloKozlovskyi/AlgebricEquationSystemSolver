using AlgebricEquationSystemSolver.DataAccess.Models;
using AlgebricEquationSystemSolver.DataAccess.Models.Configurations;
using CitiesManager.DataAccess.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AlgebricEquationSystemSolver.DataAccess
{
	public class AlgebricEquationSystemDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
	{
		public DbSet<AlgebricEquationSystem> Systems { get; set; }
		public DbSet<TaskCalculation> TaskCalculations { get; set; }
		public DbSet<CancellationTokenCalculation> CancellationTokenCalculations { get; set; }


		public AlgebricEquationSystemDbContext(DbContextOptions options) : base(options)
		{

		}
		public AlgebricEquationSystemDbContext()
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfiguration(new AlgebricEquationSystemConfiguration());
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			// set connection string
			//optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AlgebricEquationSystemDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=system_;Username=postgres;Password=postgres;", builder =>
			{
				builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);

			});
		}
	}
	public static class MigrationExtensions
	{
		public static void ApplyMigrations(this IApplicationBuilder app)
		{
			using IServiceScope scope = app.ApplicationServices.CreateScope();

			using AlgebricEquationSystemDbContext dbContext = scope.ServiceProvider.GetRequiredService<AlgebricEquationSystemDbContext>();

			dbContext.Database.Migrate();
		}
	}
}
