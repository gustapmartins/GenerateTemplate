using GenerateTemplate.Domain.Entity.UserEntity;
using GenerateTemplate.Domain.Utils;


namespace GenerateTemplate.Domain.Interface.Dao;

/// <summary>
/// Interface for authentication data access object.
/// </summary>
public interface IAuthDao : BaseDao<UserModel>
{
    /// <summary>
    /// Finds a user by their email.
    /// </summary>
    /// <param name="Email">The email of the user to find.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user model.</returns>
    Task<UserModel> FindEmail(string Email);
}
