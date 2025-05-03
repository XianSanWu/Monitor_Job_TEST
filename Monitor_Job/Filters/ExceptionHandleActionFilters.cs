using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Models.Dto.Responses;

namespace WebApi.Filters
{
    public class ExceptionHandleActionFilters
    {
        //並非所有的返回結果都要進行ResultResponse<T> 的包裝，因此定義了NoWrapperAttribute來實現這一效果，只要 Controller 或 Action 有[NoWrapperAttribute] 的修飾則不對返回結果進行任何處理。
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        public class NoWrapperAttribute : Attribute
        {
        }

        /// <summary>
        /// 統一回傳訊息格式 
        /// </summary>
        public class ResultWrapperFilter : ActionFilterAttribute
        {
            public override void OnResultExecuting(ResultExecutingContext context)
            {
                var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                var actionWrapper = controllerActionDescriptor?.MethodInfo.GetCustomAttributes(typeof(NoWrapperAttribute), false).FirstOrDefault();
                var controllerWrapper = controllerActionDescriptor?.ControllerTypeInfo.GetCustomAttributes(typeof(NoWrapperAttribute), false).FirstOrDefault();

                //如果 Controller/Action 包含 [NoWrapperAttribute] 則不需要對返回結果進行包裝，直接返回原始值
                if (actionWrapper != null || controllerWrapper != null)
                {
                    return;
                }

                //根據實際需求進行具體實現
                var rspResult = new ResultResponse<object>();
                if (context.Result is ObjectResult)
                {
                    var objectResult = context.Result as ObjectResult;
                    if (objectResult?.Value == null)
                    {
                        rspResult.Status = ResultStatus.Fail;
                        rspResult.Message = "未找到資源";
                        context.Result = new ObjectResult(rspResult);
                    }
                    else
                    {
                        //如果返回結果已經是ResultResponse<T>類型的則不需要進行再次包裝了
                        if (objectResult.DeclaredType != null && objectResult.DeclaredType.IsGenericType && objectResult.DeclaredType.GetGenericTypeDefinition() == typeof(ResultResponse<>))
                        {
                            return;
                        }

                        rspResult.Data = objectResult.Value;
                        context.Result = new ObjectResult(rspResult);
                    }
                    return;
                }
            }

        }

        /// <summary>
        /// 此處只能補捉到 Action 及 Action Filter 所發出的 Exception。
        /// </summary>
        public class GlobalExceptionFilter : IExceptionFilter
        {
            private readonly ILogger<GlobalExceptionFilter> _logger;
            public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger) => _logger = logger;

            public void OnException(ExceptionContext context)
            {
                //異常返回结果包裝
                var rspResult = ResultResponse<object>.ErrorResult(context.Exception.Message);
                //寫入Log
                _logger.LogError("{exception} {message}", context.Exception, context.Exception.Message);
                context.ExceptionHandled = true;    //Filter中標記已處理，在Middleware中就不會再處理
                context.Result = new InternalServerErrorObjectResult(rspResult);
            }

            public class InternalServerErrorObjectResult : ObjectResult
            {
                public InternalServerErrorObjectResult(object value) : base(value)
                {
                    StatusCode = StatusCodes.Status500InternalServerError;
                }
            }
        }

    }
}
