using AutoMapper;
using Users.Api.DataTransferObjects;

namespace Users.Api.Utilities.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDtoForInsertion, UserDtoForInsertion>().ReverseMap();
        }
    }
}
