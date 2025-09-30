namespace Repository.Interfaces
{
    public interface IUnitOfWorkScopeAccessor
    {
        IUnitOfWork? Current { get; set; }
    }
}