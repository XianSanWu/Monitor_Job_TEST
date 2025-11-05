using Models.Common;

namespace Models.Entities.Responses
{
    public class BatchIdAppMhResultSuccessCountEntityResponse
    {
        #region 查詢List回傳
        public class BatchIdAppMhResultSuccessCountEntitySearchListResponse : BaseModel
        {
            #region Properties
            /// <summary>  查詢結果[清單] </summary>
            //public List<BatchIdAppMhResultSuccessCountEntity>? SearchItem { get; set; }
            #endregion

        }
        #endregion
        public class BatchIdAppMhResultSuccessCountEntity
        {
            public int ProjectId { get; set; }
            public int SuccessCount { get; set; }
        }
    }
}
