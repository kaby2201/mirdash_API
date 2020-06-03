using AutoMapper;
using backend.Models;
using backend.Models.Users;

namespace backend.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, UserModel>();
            CreateMap<RegisterModel, ApplicationUser>();
            CreateMap<UpdateModel, ApplicationUser>();
        }
    }
}