using AutoMapper;
using GenerateTemplate.Application.AppServices.v1.Interfaces;
using GenerateTemplate.Application.Dto.v1.User;
using GenerateTemplate.Domain.Entity;
using GenerateTemplate.Domain.Entity.UserEntity;
using GenerateTemplate.Domain.Interface.Services.v1;

namespace GenerateTemplate.Application.AppServices.v1;

public class AuthAppService : IAuthAppService
{
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;

    public AuthAppService(IMapper mapper, IAuthService authService)
    {
        _mapper = mapper;
        _authService = authService;
    }
    
    public async Task<OperationResult<IEnumerable<ViewUserDto>>> GetAsync(int page, int pageSize)
    {
        OperationResult<IEnumerable<UserModel>> result = await _authService.GetAllAsync(page, pageSize);

        return _mapper.Map<OperationResult<IEnumerable<ViewUserDto>>>(result);
    }

    public async Task<OperationResult<ViewUserDto>> GetIdAsync(string Id)
    {
        OperationResult<UserModel> result = await _authService.GetIdAsync(Id);

        return _mapper.Map<OperationResult<ViewUserDto>>(result);
    }

    public async Task<OperationResult<string>> LoginAsync(LoginDto userLogin)
    {
        UserModel LoginDto = _mapper.Map<UserModel>(userLogin);

        return await _authService.Login(LoginDto);
    }
    public async Task<OperationResult<CreateUserDto>> CreateAsync(CreateUserDto createUserDto)
    {
        UserModel UserCreated = _mapper.Map<UserModel>(createUserDto);

        OperationResult<UserModel> result = await _authService.CreateAsync(UserCreated);

        return _mapper.Map<OperationResult<CreateUserDto>>(result);
    }
  

    public async Task<OperationResult<ViewUserDto>> RemoveAsync(string Id)
    {
        OperationResult<UserModel> result = await _authService.RemoveAsync(Id);

        return _mapper.Map<OperationResult<ViewUserDto>>(result);
    }

    public async Task<OperationResult<ViewUserDto>> UpdateAsync(string Id, UpdateUserDto updateUserDto)
    {
        UserModel updateUserModel = _mapper.Map<UserModel>(updateUserDto);

        OperationResult<UserModel> result = await _authService.UpdateAsync(Id, updateUserModel);

        return _mapper.Map<OperationResult<ViewUserDto>>(result);
    }

    public async Task<OperationResult<string>> ForgetPasswordAsync(string email)
    {
        return await _authService.ForgetPassword(email);
    }

    public OperationResult<string> VerificationPasswordOTP(string token)
    {
        return _authService.VerificationPasswordOTP(token);
    }

    public async Task<OperationResult<string>> ResetPasswordAsync(PasswordResetDto passwordResetDto)
    {
        PasswordReset passwordResetModel = _mapper.Map<PasswordReset>(passwordResetDto);

        return await _authService.ResetPassword(passwordResetModel);
    }
}
