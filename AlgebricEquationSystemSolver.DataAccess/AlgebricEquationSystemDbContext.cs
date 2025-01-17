﻿using AlgebricEquationSystemSolver.DataAccess.Models;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
			modelBuilder.Entity<TaskCalculation>()
				.HasOne(c => c.System);
			modelBuilder.Entity<TaskCalculation>()
				.HasOne(c => c.User);

			modelBuilder.Entity<CancellationTokenCalculation>()
				.HasOne(tc => tc.TaskCalculation);
			modelBuilder.Entity<CancellationTokenCalculation>()
				.HasOne(tc => tc.System);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			// set connection string
			//optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AlgebricEquationSystemDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
			base.OnConfiguring(optionsBuilder);

			try
			{
				optionsBuilder.UseNpgsql("Host=system.database;Port=5432;Database=system;Username=postgres;Password=postgres;", builder =>
				{
					builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
				});
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc.Message);
			}
		}
	}
	public static class MigrationExtensions
	{
		public static void ApplyMigrations(this IApplicationBuilder app)
		{
			using IServiceScope scope = app.ApplicationServices.CreateScope();

			using AlgebricEquationSystemDbContext dbContext = scope.ServiceProvider.GetRequiredService<AlgebricEquationSystemDbContext>();
			try
			{
				dbContext.Database.Migrate();
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc.Message);
			}

		}
	}
}
