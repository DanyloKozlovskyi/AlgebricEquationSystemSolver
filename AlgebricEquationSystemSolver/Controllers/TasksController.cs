using AlgebricEquationSystemSolver.DataAccess;
using AlgebricEquationSystemSolver.DataAccess.DTO;
using AlgebricEquationSystemSolver.DataAccess.Models;
using AlgebricEquationSystemSolver.WEBApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LoadBalancer.WebAPI.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/tasks")]
	public class TasksController : ControllerBase
	{
		private readonly AlgebricEquationSystemDbContext dbContext;
		private readonly AlgebricEquationSystemService systemService;
		//private readonly IHubContext<TaskHub> _hubContext;
		private const int MAX_UNKNOWN_LIMIT = 10;

		public TasksController(AlgebricEquationSystemDbContext context, AlgebricEquationSystemService equationsService/*, IHubContext<TaskHub> hubContext*/)
		{
			dbContext = context;
			systemService = equationsService;
			//_hubContext = hubContext;

			//systemService.OnProgressUpdate += (progress) => SendProgressUpdate(progress);
		}

		[HttpPost("start")]
		public async Task<ActionResult<TaskCalculation>> StartTask([FromBody] AlgebricEquationSystem request)
		{
			var taskCalculation = new TaskCalculation { SystemId = request.Id, System = request, IsCompleted = false };
			dbContext.TaskCalculations.Add(taskCalculation);
			await dbContext.SaveChangesAsync();

			AlgebricEquationSystem result;
			try
			{
				result = await systemService.MapWithAsync(request);
				//taskCalculation.State = "Completed";
				//taskCalculation.Result = string.Join(", ", result);
				taskCalculation.System = result;
			}
			catch (OperationCanceledException ex)
			{
				//taskCalculation.State = "Error: " + ex.Message;
				return StatusCode(500, taskCalculation);
			}

			//taskCalculation.Progress = 100;
			await dbContext.SaveChangesAsync();

			return Ok(taskCalculation);
		}

		/*private async void SendProgressUpdate(int progress)
		{
			var taskId = dbContext.Tasks.OrderByDescending(ts => ts.Id).FirstOrDefault()?.Id;
			if (taskId.HasValue)
			{
				await _hubContext.Clients.All.SendAsync("ReceiveTaskProgress", taskId.Value, progress);
			}
		}*/

		[HttpPost("cancel/{id}")]
		public async Task<IActionResult> CancelTask(Guid id)
		{
			var task = await dbContext.TaskCalculations.FindAsync(id);
			if (task == null)
				return NotFound();

			//task.State = "Canceled";
			CancellationTokenCalculation? token = await dbContext.CancellationTokenCalculations.FirstOrDefaultAsync(x => x.Id == id);
			task.IsCompleted = false;
			token.IsCanceled = true;
			await dbContext.SaveChangesAsync();
			return Ok("Task canceled.");
		}

		[HttpGet("history")]
		public async Task<ActionResult<List<TaskCalculation>>> GetTaskHistory()
		{
			var history = await dbContext.TaskCalculations.ToListAsync();
			return Ok(history);
		}
	}
}
