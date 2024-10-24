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
		}
	}
}
