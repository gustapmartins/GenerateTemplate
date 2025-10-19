using GenerateTemplate.Domain.Entity;
using GenerateTemplate.Domain.Interface.Dao;
using GenerateTemplate.Infra.Data.Context;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GenerateTemplate.Infra.Data.Repository.EfCore;

public class GenerateTemplateEfCore : BaseContext<GenerateTemplateEntity>, IGenerateTemplateDao
{
    private readonly IMongoCollection<GenerateTemplateEntity> _AuthCollection;

    public GenerateTemplateEfCore(IOptions<DatabaseSettings> options) : base(options, "GenerateTemplateCollection")
    {
        _AuthCollection = Collection;
    }
}
