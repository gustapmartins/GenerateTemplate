using GenerateTemplate.Domain.Entity.UserEntity;
using GenerateTemplate.Infra.Data.Repository.Utils;

namespace GenerateTemplate.Domain.Interface.Dao;

public interface IGenerateTemplateDao : BaseDao<UserModel>
{
}
