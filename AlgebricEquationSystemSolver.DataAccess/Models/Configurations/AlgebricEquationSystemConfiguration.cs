using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgebricEquationSystemSolver.DataAccess.Models.Configurations
{
	public class AlgebricEquationSystemConfiguration : IEntityTypeConfiguration<AlgebricEquationSystem>
	{
		public void Configure(EntityTypeBuilder<AlgebricEquationSystem> builder)
		{
			builder.HasKey(x => x.Id);

			/*builder.HasData(
				new AlgebricEquationSystem()
				{
					Id = new Guid("EC1A1B75-11D5-4EC6-9D52-3DDAE2EAC040"),
					Parameters = [-90, 1, -10, -100, 9, 1],
					Roots = [-15.43, 6.55, -0.98]
				},
				new AlgebricEquationSystem()
				{
					Id = new Guid("239F25AD-6F48-4C01-AFB9-66E39313C534"),
					Parameters = [15, 1, 2, 3, -1],
					Roots = [-1.59, 3.85]
				});*/
		}
	}
}
