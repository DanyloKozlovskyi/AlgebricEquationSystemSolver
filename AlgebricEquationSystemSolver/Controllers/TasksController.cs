using AlgebricEquationSystemSolver.DataAccess;
using AlgebricEquationSystemSolver.DataAccess.DTO;
using AlgebricEquationSystemSolver.DataAccess.Models;
using AlgebricEquationSystemSolver.WEBApi.Services;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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

		public TasksController(AlgebricEquationSystemDbContext context, IAlgebricEquationSystemService equationsService, IServiceProvider provider)
		{
			dbContext = context;
			systemService = equationsService;
			serviceProvider = provider;
			//_hubContext = hubContext;

			//systemService.OnProgressUpdate += (progress) => SendProgressUpdate(progress);
		}

		[HttpPost]
		public async Task<ActionResult<AlgebricEquationSystem>> StartTask([FromBody] AlgebricEquationSystem request)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized("User not authorized.");
			}

			var taskCalculation = new TaskCalculation
			{
				Id = request.Id,
				SystemId = request.Id,
				System = request,
				IsCompleted = false,
				UserId = Guid.Parse(userId)
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

			AlgebricEquationSystem result;
			using (var scope = serviceProvider.CreateScope())
			{
				var scopedContext = scope.ServiceProvider.GetRequiredService<AlgebricEquationSystemDbContext>();

				try
				{
					result = await systemService.MapWithAsync(request, scopedContext);
					//taskCalculation.State = "Completed";
					//taskCalculation.Result = string.Join(", ", result);
					taskCalculation.System = result;
				}
				catch (OperationCanceledException ex)
				{
					//taskCalculation.State = "Error: " + ex.Message;
					dbContext.TaskCalculations.Remove(taskCalculation);
					await dbContext.SaveChangesAsync();
					return Ok(null);
				}
			}
			//taskCalculation.Progress = 100;
			await dbContext.SaveChangesAsync();

			return Ok(result);
		}

		/*private async void SendProgressUpdate(int progress)
		{
			var taskId = dbContext.Tasks.OrderByDescending(ts => ts.Id).FirstOrDefault()?.Id;
			if (taskId.HasValue)
			{
				await _hubContext.Clients.All.SendAsync("ReceiveTaskProgress", taskId.Value, progress);
			}
		}*/

		[HttpDelete("terminate/{id}")]
		public async Task<IActionResult> TerminateTask(Guid id)
		{
			var task = await dbContext.TaskCalculations.FirstOrDefaultAsync(x => x.Id == id);
			if (task == null)
				return NotFound();

			//task.State = "Canceled";
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
