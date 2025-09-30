using Models.Enums;
using Repository.Interfaces;

namespace Repository.UnitOfWorkExtension
{
    public static class UnitOfWorkExtension
    {
        public static IUnitOfWork UseUnitOfWork(this IUnitOfWorkFactory factory, IUnitOfWorkScopeAccessor accessor, DBConnectionEnum dbType, bool useTransaction = false)
        {
            var uow = factory.Create(dbType, useTransaction);
            accessor.Current = uow;
            return uow;
        }
    }

}
