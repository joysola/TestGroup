using AutoMapper;
using Entities;
using Shared.DataTransferObjects;

namespace Chapter4.WebAPi
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>().
                ForCtorParam("FullAddress",
                opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));
        }
    }
}
