using Models.Common;

namespace Models.Dto.Responses
{
    public class BatchIdAppMhResultSuccessCountResponse
    {
        public class BatchIdAppMhResultSuccessCountSearchListResponse : BaseModel
        {
            #region Properties
            /// <summary>  查詢結果[清單] </summary>
            //public List<BatchIdAppMhResultSuccessCountSearchResponse>? SearchItem { get; set; }
            #endregion

            /// <summary> 查詢結果[欄位] </summary>
            public class BatchIdAppMhResultSuccessCountSearchResponse
            {
                public int ProjectId { get; set; }
                public int SuccessCount { get; set; }
            }
        }
       
    }
}
