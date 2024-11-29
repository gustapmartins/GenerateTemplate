using GenerateTemplate.Domain.Entity;
using GenerateTemplate.Domain.Entity.UserEntity;
using GenerateTemplate.Domain.Interface.Dao;
using GenerateTemplate.Infra.Data.Context;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace VarzeaLeague.Infra.Data.Repository.EfCore;

public class AuthDaoEfCore : BaseContext<UserModel>, IAuthDao
{
    private readonly IMongoCollection<UserModel> _AuthCollection;

    public AuthDaoEfCore(IOptions<DatabaseSettings> options) : base(options, "AuthCollection")
    {
        _AuthCollection = Collection;
    }

    public async Task<UserModel> FindEmail(string Email)
    {
        return await _AuthCollection.Find(x => x.Email == Email).FirstOrDefaultAsync();
    }
}
