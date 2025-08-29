using AutoMapper;
using EmployeeAdminPortal.Models.Dto;
using EmployeeAdminPortal.Models.Entities;
using Models.Model.Dto;
using Models.Model.Entities;
using Models.Models.Dto;

namespace BusinessLogicLayer.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity -> DTO (Response)
            CreateMap<Employee, EmployeeResponseDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : null))
                .ForMember(dest => dest.WorkScheduleName, opt => opt.MapFrom(src => src.WorkSchedule != null ? src.WorkSchedule.Name : null))
                .ForMember(dest => dest.WorkStartTime, opt => opt.MapFrom(src => src.WorkSchedule != null ? src.WorkSchedule.StartTime.ToString(@"hh\:mm") : null))
                .ForMember(dest => dest.WorkEndTime, opt => opt.MapFrom(src => src.WorkSchedule != null ? src.WorkSchedule.EndTime.ToString(@"hh\:mm") : null))
                .ForMember(dest => dest.BossId, opt => opt.MapFrom(src => src.BossId))
                .ForMember(dest => dest.BossName, opt => opt.MapFrom(src => src.Boss != null ? src.Boss.Username : null));

            // DTO -> Entity (Create/Update)
            CreateMap<EmployeeDto, Employee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id auto-generate olur
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Ayrıca hash edilir
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Manually set edilir
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // Manually set edilir
                .ForMember(dest => dest.Role, opt => opt.Ignore()) // Navigation property
                .ForMember(dest => dest.WorkSchedule, opt => opt.Ignore()) // Navigation property
                .ForMember(dest => dest.TimeLogs, opt => opt.Ignore()) // Navigation property
                //.ForMember(dest => dest.ManagedRoles, opt => opt.Ignore()) // Navigation property
                .ForMember(dest => dest.Boss, opt => opt.Ignore()) // Navigation property
                .ForMember(dest => dest.Subordinates, opt => opt.Ignore()); // Navigation property

            // Yeni məzuniyyət müraciəti map-i
            CreateMap<Permission, PermissionDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.Username));

            CreateMap<Vacation, VacationDto>()
              .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.Username : string.Empty));
    }
    }
}