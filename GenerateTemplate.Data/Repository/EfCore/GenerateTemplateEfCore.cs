using GenerateTemplate.Domain.Entity;
using GenerateTemplate.Domain.Entity.UserEntity;
using GenerateTemplate.Domain.Interface.Dao;
using GenerateTemplate.Infra.Data.Context;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GenerateTemplate.Infra.Data.Repository.EfCore;

public class GenerateTemplateEfCore : BaseContext<UserModel>, IAuthDao
{
    private readonly IMongoCollection<UserModel> _AuthCollection;

    public GenerateTemplateEfCore(IOptions<DatabaseSettings> options) : base(options, "GenerateTemplateCollection")
    {
        _AuthCollection = Collection;
    }

    public Task<UserModel> FindEmail(string Email)
    {
        throw new NotImplementedException();
    }
}
