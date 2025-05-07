using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Models.Dto.Responses;
using Models.Enums;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController(
        IConfiguration configuration,
        IMapper mapper
            ) : ControllerBase
    {
        public readonly IConfiguration _config = configuration;
        public readonly IMapper _mapper = mapper;

        /// <summary>
        /// 成功狀態返回結果
        /// </summary>
        /// <param name="result">返回的數據</param>
        /// <returns></returns>
        protected ResultResponse<T> SuccessResult<T>(T result)
        {
            return ResultResponse<T>.SuccessResult(result);
        }

        /// <summary>
        /// 失敗狀態返回結果
        /// </summary>
        /// <param name="code">狀態碼</param>
        /// <param name="msg">失敗訊息</param>
        /// <param name="requestValidateData">檢核錯誤訊息</param>
        /// <returns></returns>
        protected ResultResponse<T> FailResult<T>(string? msg = null)
        {
            return ResultResponse<T>.FailResult(msg);
        }

        /// <summary>
        /// 異常狀態返回結果
        /// </summary>
        /// <param name="code">狀態碼</param>
        /// <param name="msg">異常訊息</param>
        /// <returns></returns>
        protected ResultResponse<T> ErrorResult<T>(string? msg = null)
        {
            return ResultResponse<T>.ErrorResult(msg);
        }

        /// <summary>
        /// 自定義狀態返回結果
        /// </summary>
        /// <param name="status"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected ResultResponse<T> Result<T>(ResultStatus status, T result, string? msg = null)
        {
            return ResultResponse<T>.Result(status, result, msg);
        }
    }
}
