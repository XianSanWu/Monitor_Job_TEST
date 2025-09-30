using Repository.Interfaces;
using System.Threading;

namespace Repository.Repositories
{
    public class UnitOfWorkScopeAccessor : IUnitOfWorkScopeAccessor
    {
        // 每個 async context(request/thread) 都有自己的 Current
        private static readonly AsyncLocal<IUnitOfWork?> _current = new();

        public IUnitOfWork? Current
        {
            get => _current.Value;
            set => _current.Value = value;
        }
    }
}
