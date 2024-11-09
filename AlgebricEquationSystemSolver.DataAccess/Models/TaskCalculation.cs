using CitiesManager.DataAccess.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgebricEquationSystemSolver.DataAccess.Models
{
	public class TaskCalculation
	{
		[Key]
		public Guid Id { get; set; }

		[ForeignKey(nameof(ApplicationUser))]
		public Guid UserId { get; set; }

		public virtual ApplicationUser User { get; set; } = null!;

		[ForeignKey(nameof(AlgebricEquationSystem))]
		public Guid SystemId { get; set; }

		public virtual AlgebricEquationSystem? System { get; set; } = null;

		[Required]
		public bool IsCompleted { get; set; }
	}
}
