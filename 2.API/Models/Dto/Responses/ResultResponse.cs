using Models.Enums;
using Utilities.Extensions;

namespace Models.Dto.Responses
{
    /// <summary>
    /// 客製化 Response 格式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultResponse<T>
    {
        #region Properties
        /// <summary>
        /// 狀態結果
        /// </summary>
        public ResultStatus Status { get; set; } = ResultStatus.Success;

        private string? _msg;
        /// <summary>
        /// 訊息描述
        /// </summary>
        public string? Message
        {
            get
            {
                // 如果沒有自定義的結果描述，則可以獲取當前狀態的描述
                return !string.IsNullOrEmpty(_msg) ? _msg : Status.GetDescription();    //EnumHelper.GetDescription(Status);
            }
            set
            {
                _msg = value;
            }
        }

        /// <summary>
        /// 回傳結果
        /// </summary>
        public T? Data { get; set; }

        #endregion

        #region Response樣板

        /// <summary>
        /// 自訂 Response 內容中會包含 成功、失敗、異常的格式。
        /// </summary>
        /// <param name="status"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static ResultResponse<T> Result(ResultStatus status, T data, string? msg = null)
        {
            return new ResultResponse<T> { Status = status, Data = data, Message = msg };
        }

        #endregion

        #region 成功格式

        /// <summary>
        /// [成功狀態]回傳結果
        /// </summary>
        /// <param name="result">傳回的資料</param>
        /// <returns></returns>
        public static ResultResponse<T> SuccessResult(T data)
        {
            return new ResultResponse<T> { Status = ResultStatus.Success, Data = data };
        }

        #endregion

        #region 失敗格式

        /// <summary>
        /// [失敗狀態]回傳結果
        /// </summary>
        /// <param name="code">狀態碼</param>
        /// <param name="msg">失敗訊息</param>
        /// <returns></returns>
        public static ResultResponse<T> FailResult(string? msg = null)
        {
            return new ResultResponse<T> { Status = ResultStatus.Fail, Message = msg };
        }

        #endregion

        #region 異常格式

        /// <summary>
        /// [異常狀態]回傳結果
        /// </summary>
        /// <param name="code">狀態碼</param>
        /// <param name="msg">異常訊息</param>
        /// <returns></returns>
        public static ResultResponse<T> ErrorResult(string? msg = null)
        {
            return new ResultResponse<T> { Status = ResultStatus.Error, Message = msg };
        }

        #endregion

    }

}
