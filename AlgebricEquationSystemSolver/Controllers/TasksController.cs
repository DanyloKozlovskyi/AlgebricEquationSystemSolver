using AlgebricEquationSystemSolver.DataAccess;
using AlgebricEquationSystemSolver.DataAccess.DTO;
using AlgebricEquationSystemSolver.DataAccess.Models;
using AlgebricEquationSystemSolver.WEBApi.Services;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AlgebricEquationSystemSolver.WebAPI.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]
	public class TasksController : ControllerBase
	{
		private readonly AlgebricEquationSystemDbContext dbContext;
		private readonly IAlgebricEquationSystemService systemService;
		private readonly IServiceProvider serviceProvider;
		private const int MaxTasks = 2;

		public TasksController(AlgebricEquationSystemDbContext context, IAlgebricEquationSystemService equationsService, IServiceProvider provider)
		{
			dbContext = context;
			systemService = equationsService;
			serviceProvider = provider;
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult<AlgebricEquationSystem>> StartTask([FromBody] AlgebricEquationSystem request)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized("User not authorized.");
			}

			Guid userGuid = Guid.Parse(userId);

			//validate max number of tasks
			int countTasksOfUser = dbContext.TaskCalculations.Where(x => x.UserId == userGuid)
				.Count(x => !x.IsCompleted);
			if (countTasksOfUser >= MaxTasks)
				throw new OverflowException("max number of tasks is calculating");


			var taskCalculation = new TaskCalculation
			{
				Id = request.Id,
				SystemId = request.Id,
				System = request,
				IsCompleted = false,
				UserId = userGuid
			};

			var cancellationTokenCalculation = new CancellationTokenCalculation()
			{
				Id = request.Id,
				TaskId = request.Id,
				SystemId = request.Id,
				System = request,
				IsCanceled = false
			};


			dbContext.TaskCalculations.Add(taskCalculation);
			dbContext.CancellationTokenCalculations.Add(cancellationTokenCalculation);
			await dbContext.SaveChangesAsync();

			_ = Task.Run(async () =>
			{
				Console.WriteLine("\n\n\nWe entered Task\n\n\n");
				using (var scope = serviceProvider.CreateScope())
				{
					Console.WriteLine("\n\n\nWe entered scope\n\n\n");
					var scopedContext = scope.ServiceProvider.GetRequiredService<AlgebricEquationSystemDbContext>();
					try
					{
						await AlgebricEquationSystemService.MapWithAsync(request, scopedContext).ConfigureAwait(false);
						//await systemService.MapWithAsync(
					}
					catch (OperationCanceledException ex)
					{
						scopedContext.TaskCalculations.Remove(taskCalculation);
						scopedContext.Systems.Remove(request);
						var cancellationToken = await scopedContext.CancellationTokenCalculations.FirstOrDefaultAsync(x => x.Id == taskCalculation.Id).ConfigureAwait(false);
						scopedContext.CancellationTokenCalculations.Remove(cancellationToken);
						await scopedContext.SaveChangesAsync().ConfigureAwait(false);
					}
					catch (Exception ex)
					{
						Console.WriteLine($"\n\n\nError processing task: {ex.Message}\n\n\n");
					}
				}
			});
			Thread.Sleep(13);

			Console.WriteLine("\n\n\nFunction exited with Ok\n\n\n");
			return Ok();
		}


		[HttpGet("{taskId}/status")]
		public async Task<IActionResult> GetTaskStatus(Guid taskId)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized(false);
			}
			TaskCalculation task = null;
			try
			{
				task = await dbContext.TaskCalculations.Include(x => x.System).FirstOrDefaultAsync(t => t.Id == taskId);
			}
			catch (Exception exc)
			{
				Console.WriteLine($"exception was caught {exc.Message}");
			}
			if (task == null || task.UserId != Guid.Parse(userId))
			{
				return NotFound(false);
			}

			bool isCompleted = task.IsCompleted;
			return Ok(isCompleted);
		}

		[HttpDelete("terminate/{id}")]
		public async Task<IActionResult> TerminateTask(Guid id)
		{
			var task = await dbContext.TaskCalculations.FirstOrDefaultAsync(x => x.Id == id);
			if (task == null)
				return NotFound();

			CancellationTokenCalculation? token = await dbContext.CancellationTokenCalculations.FirstOrDefaultAsync(x => x.Id == id);
			task.IsCompleted = false;
			token.IsCanceled = true;
			await dbContext.SaveChangesAsync();
			return Ok();
		}

		[HttpGet("history")]
		public async Task<ActionResult<List<TaskCalculation>>> GetTaskHistory()
		{
			var history = await dbContext.TaskCalculations.ToListAsync();
			return Ok(history);
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<TaskCalculation>>> Get()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized("User not authorized.");
			}

			Guid userGuid = Guid.Parse(userId);
			var tasks = await dbContext.TaskCalculations.Include(x => x.User).Include(x => x.System).Where(x => x.UserId == userGuid).ToListAsync();

			var systems = await Task.Run(() =>
			{
				return tasks.Select(x => x.System);
			});

			return Ok(systems);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAlgebricEquationSystem(Guid id)
		{
			var systemDeleted = await systemService.DeleteSystem(id).ConfigureAwait(false);
			if (systemDeleted == false)
			{
				return NotFound();
			}

			return NoContent();
		}
	}
}
