using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AlgebricEquationSystemSolver.DataAccess.DTO
{
	public class AlgebricEquationSystemCreate
	{
		private static double EPS = 0.000001;
		public List<double>? Parameters { get; set; }
		public static List<double>? FindRoots(List<double> parameters)
		{
			var polynomial = new MathNet.Numerics.Polynomial(parameters);

			// Find the roots of the polynomial
			var complexRoots = polynomial.Roots();

			var roots = complexRoots.Where(root => root.Imaginary == 0).Select(root => root.Real).ToList();
			return roots.Select(x => Math.Round(x, 2)).ToList();
		}
		/*public static async Task<List<double>?> FindRootsAsync(List<double> coefficients)
		{
			return await Task.Run(() => FindRoots(coefficients));
		}*/
	}
}
