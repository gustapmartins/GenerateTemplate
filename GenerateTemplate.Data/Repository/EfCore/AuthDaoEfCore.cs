using GenerateTemplate.Domain.Entity;
using GenerateTemplate.Domain.Entity.UserEntity;
using GenerateTemplate.Domain.Interface.Dao;
using GenerateTemplate.Infra.Data.Context;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GenerateTemplate.Infra.Data.Repository.EfCore;

public class AuthDaoEfCore : BaseContext<UserEntity>, IAuthDao
{
    private readonly IMongoCollection<UserEntity> _AuthCollection;

    public AuthDaoEfCore(IOptions<DatabaseSettings> options) : base(options, "AuthCollection")
    {
        _AuthCollection = Collection;
    }

    public async Task<UserEntity> FindEmailAsync(string Email)
    {
        return await _AuthCollection.Find(x => x.Email == Email).FirstOrDefaultAsync();
    }
}
