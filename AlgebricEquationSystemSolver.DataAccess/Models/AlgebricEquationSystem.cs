using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgebricEquationSystemSolver.DataAccess.Models
{
	// need to add validation in DTO Remote(action: controller: message: ) - to check whether system has one solution
	public class AlgebricEquationSystem
	{
		[Key]
		public Guid Id { get; set; }
		public List<double>? Parameters { get; set; }
		public List<double>? Roots { get; set; }
	}
}
