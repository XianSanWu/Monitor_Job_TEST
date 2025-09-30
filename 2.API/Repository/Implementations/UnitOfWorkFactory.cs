using Microsoft.Extensions.Configuration;
using Models.Enums;
using Repository.Interfaces;

namespace Repository.Implementations
{
    public class UnitOfWorkFactory(IConfiguration config) : IUnitOfWorkFactory
    {
        private readonly IConfiguration _config = config;

        public IUnitOfWork Create(DBConnectionEnum connectionType = DBConnectionEnum.DefaultConnection, bool useTransaction = true)
        {
            var dbHelper = new DbHelper(_config, connectionType);
            var uow = dbHelper.UnitOfWork;

            if (useTransaction)
            {
                uow.Begin();
            }

            return uow;
        }
    }
}
