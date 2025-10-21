using GenerateTemplate.Domain.Entity;
using GenerateTemplate.Domain.Entity.UserEntity;

namespace GenerateTemplate.Domain.Interface.Services.v1;

/// <summary>
/// Interface for authentication services.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Gets a paginated list of users.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of users.</returns>
    Task<OperationResult<IEnumerable<UserEntity>>> GetAllAsync(int page, int pageSize);

    /// <summary>
    /// Gets a user by ID.
    /// </summary>
    /// <param name="Id">The user ID.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user.</returns>
    Task<OperationResult<UserEntity>> GetIdAsync(string Id);

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="addObject">The user to add.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created user.</returns>
    Task<OperationResult<UserEntity>> CreateAsync(UserEntity addObject);

    /// <summary>
    /// Removes a user by ID.
    /// </summary>
    /// <param name="Id">The user ID.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the removed user.</returns>
    Task<OperationResult<UserEntity>> RemoveAsync(string Id);

    /// <summary>
    /// Updates a user by ID.
    /// </summary>
    /// <param name="Id">The user ID.</param>
    /// <param name="updateObject">The user to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated user.</returns>
    Task<OperationResult<UserEntity>> UpdateAsync(string Id, UserEntity updateObject);

    /// <summary>
    /// Logs in a user.
    /// </summary>
    /// <param name="userLogin">The user login details.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the login token.</returns>
    Task<OperationResult<string>> LoginAsync(UserEntity userLogin);

    /// <summary>
    /// Sends a forget password email.
    /// </summary>
    /// <param name="email">The user's email.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the status of the operation.</returns>
    Task<OperationResult<string>> ForgetPasswordAsync(string email);

    /// <summary>
    /// Resets the password.
    /// </summary>
    /// <param name="passwordReset">The password reset details.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the status of the operation.</returns>
    Task<OperationResult<string>> ResetPasswordAsync(PasswordReset passwordReset);

    /// <summary>
    /// Verifies the password OTP.
    /// </summary>
    /// <param name="token">The OTP token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the status of the operation.</returns>
    OperationResult<string> VerificationPasswordOTP(string token);
}
