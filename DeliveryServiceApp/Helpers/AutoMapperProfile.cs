using AutoMapper;
using DeliveryServiceApp.Models.Entities;
using DeliveryServiceApp.Models.Steps;
using DeliveryServiceApp.Models.Users;

namespace DeliveryServiceApp.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region User
            CreateMap<User, UserModel>();
            CreateMap<RegisterModel, User>();
            CreateMap<UpdateModel, User>();
            CreateMap<Models.Users.GetModel, User>();
            #endregion

            #region Steps
            CreateMap<CreateModel, Step>();
            CreateMap<StepModel, Step>();
            #endregion
        }
    }
}
