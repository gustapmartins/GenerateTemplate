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
        CreateMap<UserEntity, CreateUserDto>();
        CreateMap<UserEntity, CreateUserDto>().ReverseMap();
        CreateMap<LoginDto, UserEntity>();
        CreateMap<ViewUserDto, UserEntity>();
        CreateMap<UserEntity, ViewUserDto>();
        CreateMap<PasswordResetDto, PasswordReset>().ReverseMap();
        CreateMap<UpdateUserDto, UserEntity>().ReverseMap();

        CreateMap(typeof(OperationResult<>), typeof(OperationResult<>));
    }
}
