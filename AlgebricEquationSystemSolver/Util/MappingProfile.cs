using AlgebricEquationSystemSolver.DataAccess.DTO;
using AlgebricEquationSystemSolver.DataAccess.Models;
using AutoMapper;
using CitiesManager.DataAccess.DTO;
using CitiesManager.DataAccess.Identity;

namespace AlgebricEquationSystemSolver.WEBApi.Util
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<AlgebricEquationSystem, AlgebricEquationSystemCreate>()
				.ForMember(dest => dest.Parameters, opt => opt.MapFrom(src => src.Parameters));

			CreateMap<AlgebricEquationSystemCreate, AlgebricEquationSystem>()
				.ForMember(dest => dest.Id, opt => Guid.NewGuid())
				.ForMember(dest => dest.Parameters, opt => opt.MapFrom(src => src.Parameters))
				.ForMember(dest => dest.Roots, opt => opt.MapFrom(src => AlgebricEquationSystemCreate.FindRoots(src.Parameters)));

			CreateMap<RegisterDTO, ApplicationUser>()
				.ForMember(dest => dest.PersonName, opt => opt.MapFrom(src => src.PersonName))
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
		}
	}
}
