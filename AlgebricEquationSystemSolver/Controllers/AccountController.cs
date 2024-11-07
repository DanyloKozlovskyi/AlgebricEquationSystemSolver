﻿using AlgebricEquationSystemSolver.WEBApi.Util;
using AutoMapper;
using CitiesManager.DataAccess.DTO;
using CitiesManager.DataAccess.Identity;
using CitiesManager.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CitiesManager.WebAPI.Controllers
{
	[Route("api/[controller]")]
	[AllowAnonymous]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly RoleManager<ApplicationRole> roleManager;
		private readonly IJwtService jwtService;
		private readonly IMapper mapper;

		public AccountController(UserManager<ApplicationUser> userMng,
			SignInManager<ApplicationUser> signInMng, RoleManager<ApplicationRole> roleMng, IJwtService jwtSvc)
		{
			userManager = userMng;
			signInManager = signInMng;
			roleManager = roleMng;
			jwtService = jwtSvc;

			var map = new MapperConfiguration
			(
				mc => mc.AddProfile(new MappingProfile())
			);
			mapper = map.CreateMapper();
		}

		[HttpPost("register")]
		public async Task<IActionResult> PostRegister(RegisterDTO registerDTO)
		{
			// Validation 
			if (ModelState.IsValid == false)
			{
				string errorMessages = string.Join(" | ", ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage));
				return Problem(errorMessages);
			}

			// Create user
			ApplicationUser user = mapper.Map<ApplicationUser>(registerDTO);

			IdentityResult result = await userManager.CreateAsync(user, registerDTO.Password);

			if (result.Succeeded == true)
			{
				// sign-in
				// isPersister: false - must be deleted automatically when the browser is closed
				await signInManager.SignInAsync(user, isPersistent: false);

				var authenticationResponse = jwtService.CreateJwtToken(user);
				user.RefreshToken = authenticationResponse.RefreshToken;

				user.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;
				await userManager.UpdateAsync(user);

				return Ok(authenticationResponse);
			}

			string errorMessage = string.Join(" | ", result.Errors.Select(e => e.Description));
			return Problem(errorMessage);
		}

		[HttpGet]
		public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
		{
			ApplicationUser? user = await userManager.FindByEmailAsync(email);

			if (user == null)
			{
				return Ok(true);
			}
			return Ok(false);
		}

		[HttpPost("login")]
		public async Task<IActionResult> PostLogin(LoginDTO loginDTO)
		{
			// Validation 
			if (ModelState.IsValid == false)
			{
				string errorMessages = string.Join(" | ", ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage));
				return Problem(errorMessages);
			}

			var result = await signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: false, lockoutOnFailure: false);

			if (result.Succeeded)
			{
				ApplicationUser? user = await userManager.FindByEmailAsync(loginDTO.Email);

				if (user == null)
					return NoContent();

				await signInManager.SignInAsync(user, isPersistent: false);

				var authenticationResponse = jwtService.CreateJwtToken(user);
				user.RefreshToken = authenticationResponse.RefreshToken;

				user.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;
				await userManager.UpdateAsync(user);


				return Ok(authenticationResponse);
			}
			return Problem("Invalid email or password");
		}

		[HttpGet("logout")]
		public async Task<IActionResult> GetLogout()
		{
			await signInManager.SignOutAsync();

			return NoContent();
		}

		[HttpPost("generate-new-jwt-token")]
		public async Task<IActionResult> GenerateNewAccessToken(TokenModel tokenModel)
		{
			if (tokenModel == null)
			{
				return BadRequest("Invalid client request");
			}

			string? token = tokenModel.Token;
			string? refreshToken = tokenModel.RefreshToken;


			ClaimsPrincipal? principal = jwtService.GetPrincipalFromJwtToken(token);
			if (principal == null)
			{
				return BadRequest("Invalid access token");
			}

			string? email = principal.FindFirstValue(ClaimTypes.Email);

			ApplicationUser? user = await userManager.FindByEmailAsync(email);

			if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpirationDateTime <= DateTime.Now)
			{
				return BadRequest("Invalid refresh token");
			}

			AuthenticationResponse authenticationResponse = jwtService.CreateJwtToken(user);

			user.RefreshToken = authenticationResponse.RefreshToken;
			user.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;

			await userManager.UpdateAsync(user);

			return Ok(authenticationResponse);
		}
	}
}
