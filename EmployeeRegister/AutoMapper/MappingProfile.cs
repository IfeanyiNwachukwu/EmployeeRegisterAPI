using AutoMapper;
using Entities.DataTransferObjects.ReadOnly;
using Entities.DataTransferObjects.Writable;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeRegister.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDTO>()
                .ForMember(c => c.FullAddress, opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));

            CreateMap<Employee, EmployeeDTO>();
            CreateMap<CompanyDTOW, Company>();
            CreateMap<EmployeeDTOW, Employee>();
        }
    }
}
