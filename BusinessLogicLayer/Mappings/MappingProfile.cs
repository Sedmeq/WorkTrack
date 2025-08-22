using AutoMapper;
using EmployeeAdminPortal.Models.Dto;
using EmployeeAdminPortal.Models.Entities;
using Models.Models.Dto;

namespace BusinessLogicLayer.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity -> DTO
            CreateMap<Employee, EmployeeResponseDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : null))
                .ForMember(dest => dest.WorkScheduleName, opt => opt.MapFrom(src => src.WorkSchedule != null ? src.WorkSchedule.Name : null))
                .ForMember(dest => dest.WorkStartTime, opt => opt.MapFrom(src => src.WorkSchedule != null ? src.WorkSchedule.StartTime.ToString(@"hh\:mm") : null))
                .ForMember(dest => dest.WorkEndTime, opt => opt.MapFrom(src => src.WorkSchedule != null ? src.WorkSchedule.EndTime.ToString(@"hh\:mm") : null));

            // DTO -> Entity
            CreateMap<EmployeeDto, Employee>();
        }
    }
}
