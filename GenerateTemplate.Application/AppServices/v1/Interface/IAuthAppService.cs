using GenerateTemplate.Application.Dto.v1.User;
using GenerateTemplate.Domain.Entity;

namespace GenerateTemplate.Application.AppServices.v1.Interfaces;

public interface IAuthAppService
{
    /// <summary>
    /// Gets a paginated list of users.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of users.</returns>
    Task<OperationResult<IEnumerable<ViewUserDto>>> GetAsync(int page, int pageSize);

    /// <summary>
    /// Gets a user by ID.
    /// </summary>
    /// <param name="Id">The user ID.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user.</returns>
    Task<OperationResult<ViewUserDto>> GetIdAsync(string Id);

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="addObject">The user to add.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created user.</returns>
    Task<OperationResult<CreateUserDto>> CreateAsync(CreateUserDto addObject);

    /// <summary>
    /// Removes a user by ID.
    /// </summary>
    /// <param name="Id">The user ID.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the removed user.</returns>
    Task<OperationResult<ViewUserDto>> RemoveAsync(string Id);

    /// <summary>
    /// Updates a user by ID.
    /// </summary>
    /// <param name="Id">The user ID.</param>
    /// <param name="updateObject">The user to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated user.</returns>
    Task<OperationResult<ViewUserDto>> UpdateAsync(string Id, UpdateUserDto updateObject);

    /// <summary>
    /// Logs in a user.
    /// </summary>
    /// <param name="userLogin">The user login details.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the login token.</returns>
    Task<OperationResult<string>> LoginAsync(LoginDto userLogin);

    /// <summary>
    /// Sends a forget password email.
    /// </summary>
    /// <param name="email">The user's email.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the status of the operation.</returns>
    Task<OperationResult<string>> ForgetPasswordAsync(string email);

    /// <summary>
    /// Resets the password.
    /// </summary>
    /// <param name="passwordResetDto">The password reset details.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the status of the operation.</returns>
    Task<OperationResult<string>> ResetPasswordAsync(PasswordResetDto passwordResetDto);

    /// <summary>
    /// Verifies the password OTP.
    /// </summary>
    /// <param name="token">The OTP token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the status of the operation.</returns>
    OperationResult<string> VerificationPasswordOTP(string token);
}
