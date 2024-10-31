using AlgebricEquationSystemSolver.DataAccess;
using AlgebricEquationSystemSolver.DataAccess.DTO;
using AlgebricEquationSystemSolver.DataAccess.Models;
using AlgebricEquationSystemSolver.WEBApi.Services;
using AlgebricEquationSystemSolver.WEBApi.Util;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlgebricEquationSystemSolver.WEBApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[AllowAnonymous]
	public class AlgebricEquationSystemController : ControllerBase
	{
		private readonly IAlgebricEquationSystemService systemService;
		private readonly IMapper mapper;
		public AlgebricEquationSystemController(IAlgebricEquationSystemService service)
		{
			systemService = service;

			var map = new MapperConfiguration
			(
				mc => mc.AddProfile(new MappingProfile())
			);
			mapper = map.CreateMapper();
		}

		// GET: api/AlgebricEquationSystem
		[HttpGet]
		public async Task<IActionResult> GetSystems()
		{
			var systems = await systemService.GetSystems().ConfigureAwait(false);
			return Ok(systems);
		}

		// GET: api/AlgebricEquationSystem/5
		[HttpGet("{id}")]
		public async Task<IActionResult> GetAlgebricEquationSystem(Guid id)
		{
			var algebricEquationSystem = await systemService.GetSystemById(id).ConfigureAwait(false);

			if (algebricEquationSystem == null)
			{
				return NotFound();
			}

			return Ok(algebricEquationSystem);
		}

		// PUT: api/AlgebricEquationSystem/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutAlgebricEquationSystem(Guid id, AlgebricEquationSystem algebricEquationSystem)
		{
			if (id != algebricEquationSystem.Id)
			{
				return BadRequest();
			}

			try
			{
				await systemService.UpdateSystem(algebricEquationSystem).ConfigureAwait(false);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await AlgebricEquationSystemExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		// POST: api/AlgebricEquationSystem
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<IActionResult> PostAlgebricEquationSystem(AlgebricEquationSystemCreate algebricEquationSystemDto)
		{
			var algebricEquationSystem = await systemService.AddSystem(algebricEquationSystemDto).ConfigureAwait(false);

			return CreatedAtAction("GetAlgebricEquationSystem", new { id = algebricEquationSystem.Id }, algebricEquationSystem);
		}

		// DELETE: api/AlgebricEquationSystem/5
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

		private async Task<bool> AlgebricEquationSystemExists(Guid id)
		{
			return await systemService.SystemExists(id).ConfigureAwait(false);
		}
	}
}
