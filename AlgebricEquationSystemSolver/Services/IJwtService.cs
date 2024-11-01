using CitiesManager.DataAccess.DTO;
using CitiesManager.DataAccess.Identity;
using System.Security.Claims;

namespace CitiesManager.WebAPI.Services
{
	public interface IJwtService
	{
		AuthenticationResponse CreateJwtToken(ApplicationUser user);
		ClaimsPrincipal? GetPrincipalFromJwtToken(string? token);
	}
}
