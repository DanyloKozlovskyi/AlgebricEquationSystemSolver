using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgebricEquationSystemSolver.DataAccess.Models
{
	public class CancellationTokenCalculation
	{
		[Key]
		public Guid Id { get; set; }

		[ForeignKey(nameof(TaskCalculation))]
		public Guid TaskId { get; set; }

		public virtual TaskCalculation? TaskCalculation { get; set; } = null;

		[ForeignKey(nameof(AlgebricEquationSystem))]
		public Guid SystemId { get; set; }

		public virtual AlgebricEquationSystem? System { get; set; } = null;

		[Required]
		public bool IsCanceled { get; set; }
	}
}
