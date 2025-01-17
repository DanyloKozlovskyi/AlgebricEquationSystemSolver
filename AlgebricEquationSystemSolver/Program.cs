using AlgebricEquationSystemSolver.DataAccess;
using AlgebricEquationSystemSolver.WEBApi.Services;
using CitiesManager.DataAccess.Identity;
using CitiesManager.WebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddControllers(options =>
{
	var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
	options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddDbContext<AlgebricEquationSystemDbContext>(options =>
{
	//options.UseNpgsql("Host=system.database;Port=5432;Database=system;Username=postgres;Password=postgres;");
});

builder.Services.AddScoped<IAlgebricEquationSystemService, AlgebricEquationSystemService>();
builder.Services.AddTransient<IJwtService, JwtService>();

// Enable identity 
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
	options.Password.RequireDigit = false;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<AlgebricEquationSystemDbContext>()
.AddDefaultTokenProviders()
.AddUserStore<UserStore<ApplicationUser, ApplicationRole, AlgebricEquationSystemDbContext, Guid>>()
.AddRoleStore<RoleStore<ApplicationRole, AlgebricEquationSystemDbContext, Guid>>();


//to use cors do that + app.UseCors()
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policyBuilder =>
	{
		policyBuilder
	 .WithOrigins("https://localhost:4200")
	 .AllowAnyHeader()
	 .AllowAnyMethod();
	});
});

//JWT server-side authentication
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
	{
		ValidateAudience = true,
		ValidAudience = builder.Configuration["Jwt:Audience"],
		ValidateIssuer = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
	};

});

builder.Services.AddAuthorization();

var app = builder.Build();

app.ApplyMigrations();
// enable https
app.UseHsts();

// getting error with authorization cause UseRouting after UseAuthorization
app.UseRouting();

app.UseCors();
app.UseHttpsRedirection();
//UseAuthorization should be imported after UseCors() and UseHttpsRedirection() in order to avoid XMLHttpRequest has been blocked by Cors policy
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
