using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgebricEquationSystemSolver.DataAccess.Models
{
	public class AlgebricEquationSystem
	{
		public Guid Id { get; set; }
		public List<int>? Parameters { get; set; }
	}
}
