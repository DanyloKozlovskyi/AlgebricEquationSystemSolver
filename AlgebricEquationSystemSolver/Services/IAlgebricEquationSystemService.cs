﻿using AlgebricEquationSystemSolver.DataAccess.DTO;
using AlgebricEquationSystemSolver.DataAccess.Models;
using System.Linq.Expressions;

namespace AlgebricEquationSystemSolver.WEBApi.Services
{
	public interface IAlgebricEquationSystemService
	{
		Task<AlgebricEquationSystem> AddSystem(AlgebricEquationSystemCreate? systemCreate);
		Task<ICollection<AlgebricEquationSystem>> GetSystems();
		Task<AlgebricEquationSystem?> GetSystemById(Guid? id);
		Task<ICollection<AlgebricEquationSystem>> GetFilteredSystems(Expression<Func<AlgebricEquationSystem, bool>> predicate);
		Task<AlgebricEquationSystem> UpdateSystem(AlgebricEquationSystem system);
		Task<bool> DeleteSystem(Guid? id);
		Task<bool> SystemExists(Guid id);
	}
}