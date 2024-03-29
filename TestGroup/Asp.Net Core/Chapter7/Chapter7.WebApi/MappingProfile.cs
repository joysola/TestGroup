﻿using AutoMapper;
using Entities;
using Shared.DataTransferObjects;

namespace Chapter7.WebApi
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>()
                //.ForCtorParam("FullAddress",
                .ForMember(c => c.FullAddress, 
                opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));
            CreateMap<Employee, EmployeeDto>();

        }
    }
}
