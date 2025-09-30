using Models.Enums;

namespace Repository.Interfaces
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create(DBConnectionEnum connectionType = DBConnectionEnum.DefaultConnection, bool useTransaction = true);
    
    }
}
