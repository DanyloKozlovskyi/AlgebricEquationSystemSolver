using AlgebricEquationSystemSolver.DataAccess;
using AlgebricEquationSystemSolver.DataAccess.DTO;
using AlgebricEquationSystemSolver.DataAccess.Models;
using AlgebricEquationSystemSolver.WEBApi.Util;
using AutoMapper;
using Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AlgebricEquationSystemSolver.WEBApi.Services
{
	public class AlgebricEquationSystemService : IAlgebricEquationSystemService
	{
		private readonly AlgebricEquationSystemDbContext dbContext;
		private readonly IMapper mapper;
		public AlgebricEquationSystemService(AlgebricEquationSystemDbContext db)
		{
			dbContext = db;

			var map = new MapperConfiguration
			(
				mc => mc.AddProfile(new MappingProfile())
			);
			mapper = map.CreateMapper();
		}

		public async Task AddSystem(AlgebricEquationSystem? systemCreate)
		{
			if (systemCreate == null)
				throw new ArgumentNullException(nameof(systemCreate));

			await MapWithAsync(systemCreate, dbContext);
		}

		public static async Task MapWithAsync(AlgebricEquationSystem source, AlgebricEquationSystemDbContext context)
		{
			if (source == null) throw new ArgumentNullException(nameof(source.Parameters));
			Console.WriteLine("\n\n\nWe entered MapWithAsync\n\n\n");
			await Task.Run(async () =>
			{
				for (int i = 0; i < 20; i++)
				{
					Thread.Sleep(500);

					var cancelationToken = await context.CancellationTokenCalculations.FirstOrDefaultAsync(ct => ct.TaskId == source.Id).ConfigureAwait(false);
					await context.Entry(cancelationToken).ReloadAsync().ConfigureAwait(false);
					// Check for cancellation at the start of each job
					if (cancelationToken != null && cancelationToken.IsCanceled)
					{
						throw new OperationCanceledException();
					}
				}
			});
			source.Roots = AlgebricEquationSystemCreate.FindRoots(source.Parameters);
			source.IsCompleted = true;
			//here was addsystems(source)
			//context.Systems.Update(source);
			context.Systems.Update(source);

			var task = await context.TaskCalculations.FirstOrDefaultAsync(x => x.SystemId == source.Id).ConfigureAwait(false);
			task.IsCompleted = true;
			await context.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task<bool> DeleteSystem(Guid? id)
		{
			if (id == null)
				throw new ArgumentNullException(nameof(id));

			AlgebricEquationSystem? system = await dbContext.Systems.FirstOrDefaultAsync(x => x.Id == id);
			if (system == null)
				return false;

			dbContext.Systems.Remove(await dbContext.Systems.
				FirstOrDefaultAsync(c => c.Id == id));
			await dbContext.SaveChangesAsync();

			return true;
		}

		public async Task<ICollection<AlgebricEquationSystem>> GetFilteredSystems(Expression<Func<AlgebricEquationSystem, bool>> predicate)
		{
			return await dbContext.Systems.Where(predicate).ToListAsync();
		}

		public async Task<ICollection<AlgebricEquationSystem>> GetSystems()
		{
			return dbContext.Systems.AsEnumerable().TakeLast(5).ToList();
		}

		public async Task<AlgebricEquationSystem?> GetSystemById(Guid? id)
		{
			if (id == null)
				throw new ArgumentNullException(nameof(id));

			return await dbContext.Systems.FirstOrDefaultAsync(c => c.Id == id);
		}

		public async Task<bool> SystemExists(Guid id)
		{
			return await dbContext.Systems.AnyAsync(x => x.Id == id);
		}

		public async Task<AlgebricEquationSystem> UpdateSystem(AlgebricEquationSystem system)
		{
			AlgebricEquationSystem? matchingSystem = await dbContext.Systems.FirstOrDefaultAsync(x => x.Id == system.Id);

			if (matchingSystem == null)
				throw new ArgumentNullException(nameof(system));

			matchingSystem.Id = system.Id;
			matchingSystem.Parameters = system.Parameters;

			await dbContext.SaveChangesAsync();

			return system;
		}
	}
}
