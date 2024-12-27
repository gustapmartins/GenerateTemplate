using AutoMapper;
using GenerateTemplate.Application.Dto.v1.User;
using GenerateTemplate.Domain.Entity.UserEntity;
using GenerateTemplate.Domain.Entity;
using System.Diagnostics.CodeAnalysis;

namespace GenerateTemplate.Application.Mapper.v1;

[ExcludeFromCodeCoverage]
public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserModel, CreateUserDto>();
        CreateMap<UserModel, CreateUserDto>().ReverseMap();
        CreateMap<LoginDto, UserModel>();
        CreateMap<ViewUserDto, UserModel>();
        CreateMap<UserModel, ViewUserDto>();
        CreateMap<PasswordResetDto, PasswordReset>().ReverseMap();
        CreateMap<UpdateUserDto, UserModel>().ReverseMap();

        CreateMap(typeof(OperationResult<>), typeof(OperationResult<>));
    }
}
