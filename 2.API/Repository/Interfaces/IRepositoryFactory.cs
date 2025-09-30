
namespace Repository.Interfaces
{
    public interface IRepositoryFactory
    {
        T Create<T>(IUnitOfWorkScopeAccessor accessor) where T : class, IRepository;
        //T Create<T>(IUnitOfWorkScopeAccessor accessor) where T : class;
    }
}
