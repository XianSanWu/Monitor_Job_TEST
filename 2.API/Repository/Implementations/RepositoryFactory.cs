using System.Reflection;
using AutoMapper;
using Repository.Interfaces;

public class RepositoryFactory : IRepositoryFactory
{
    private readonly IMapper _mapper;
    private readonly Assembly _assembly; // 存放 Repository 類別的 assembly

    public RepositoryFactory(IMapper mapper, Assembly? assembly = null)
    {
        _mapper = mapper;
        _assembly = assembly ?? Assembly.GetExecutingAssembly();
    }

    public T Create<T>(IUnitOfWorkScopeAccessor accessor) where T : class, IRepository
    {
        // T 是介面
        if (!typeof(T).IsInterface)
            throw new InvalidOperationException($"{typeof(T).Name} 必須是介面");

        // 嘗試找同名實作類別
        var implType = _assembly.GetTypes()
            .FirstOrDefault(t => typeof(T).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

        if (implType == null)
            throw new InvalidOperationException($"無法建立 {typeof(T).Name}，找不到實作類別");

        // 找到符合建構子的 ctor
        var ctor = implType.GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length) // 優先使用最多參數
            .FirstOrDefault();

        if (ctor == null)
            throw new InvalidOperationException($"{implType.Name} 沒有公開建構子");

        // 生成建構子參數
        var ctorParams = ctor.GetParameters();
        var parameters = ctorParams
            .Select<ParameterInfo, object>(p =>
            {
                if (p.ParameterType == typeof(IUnitOfWork)) return accessor.Current!;
                if (p.ParameterType == typeof(IMapper)) return _mapper;
                if (p.ParameterType == typeof(IUnitOfWorkScopeAccessor)) return accessor;
                throw new InvalidOperationException($"無法解析參數 {p.Name} ({p.ParameterType.Name})");
            })
            .ToArray();


        return (T)Activator.CreateInstance(implType, parameters)!;
    }
}
