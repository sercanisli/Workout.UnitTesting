using AutoMapper;
using Users.Api.DataTransferObjects;
using Users.Api.Models;
using Users.Api.Utilities.FluentValidation;

namespace Users.Api.Utilities.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDtoForInsertion, User>().ReverseMap();
        }
    }
}
