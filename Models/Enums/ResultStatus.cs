using System.ComponentModel;

namespace Models.Enums
{
    /// <summary>
    /// Status欄位狀態說明：
    /// 請求成功(CRUD) = 1，
    /// 請求失敗(檢核未通過) = 0，
    /// 請求異常(Exception) = -1。
    /// </summary>
    public enum ResultStatus
    {
        [Description("請求成功")]
        Success = 1,
        [Description("請求失敗")]
        Fail = 0,
        [Description("請求異常")]
        Error = -1
    }
}
