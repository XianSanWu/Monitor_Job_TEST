namespace Models.Dto.Common
{
    public class BaseModel
    {
        #region Properties
        /// <summary> 分頁資訊 </summary>
        public PageBase Page { get; set; } = new();
        //public PageBase Page = new PageModel(); //用 new 方式 swagger 中不會出現

        /// <summary> CRUD的動作(含權限) </summary>
        //public string? ActiveType { get; set; }
        #endregion
    }

    public class Option
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
    }
}
