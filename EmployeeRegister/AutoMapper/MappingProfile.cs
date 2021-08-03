using AutoMapper;
using Entities.DataTransferObjects.ReadOnly;
using Entities.DataTransferObjects.Writable;
using Entities.DataTransferObjects.Writable.Updatable;
using Entities.Models;

namespace EmployeeRegister.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDTO>()
                .ForMember(c => c.FullAddress, opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));

            CreateMap<Employee, EmployeeDTO>();  // source|Destination  CreateMap<TSource,Tdestination>
            CreateMap<CompanyDTOW, Company>();
            CreateMap<EmployeeDTOW, Employee>();
            CreateMap<EmployeeUDTOW, Employee>().ReverseMap(); //The ReverseMap() allows for two way mapping
            CreateMap<CompanyUDTOW, Company>();

        }
    }
}
