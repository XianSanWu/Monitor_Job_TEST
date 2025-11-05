namespace Models.Common
{
    public class BaseSearchModel
    {
        #region Properties
        /// <summary> 分頁資訊 </summary>
        public PageBase Page { get; set; } = new();
        //public PageBase Page = new PageModel(); //用 new 方式 swagger 中不會出現

        /// <summary> CRUD的動作(含權限) </summary>
        //public string? ActiveType { get; set; }

        /// <summary> 前端表頭欄位篩選 </summary>
        public List<FieldWithMetadataModel>? FilterModel { get; set; }

        /// <summary>  前端表頭排序，後端 SQL order by (asc/desc) </summary>
        public Option? SortModel { get; set; }
        #endregion
    }

}
