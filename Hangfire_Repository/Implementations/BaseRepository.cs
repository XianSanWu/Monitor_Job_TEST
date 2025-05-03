using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Hangfire_Repository.Interfaces;
using System.Text;

namespace Hangfire_Repository.Implementations
{
    public class BaseRepository(IUnitOfWork unitOfWork, IMapper mapper)
    {

        #region 屬性
        protected StringBuilder? _sqlStr;
        protected string? _sqlOrderByStr;
        protected DynamicParameters? _sqlParams;
        protected List<DynamicParameters>? _sqlParamsList;
        protected readonly IUnitOfWork _unitOfWork = unitOfWork;
        protected readonly IMapper _mapper = mapper;
        #endregion

    }
}
