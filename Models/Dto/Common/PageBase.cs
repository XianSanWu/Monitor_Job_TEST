namespace Models.Dto.Common
{
    public class PageBase
    {
        #region Proterties
        /// <summary> 一頁顯示幾筆資料(單頁可容納之資料筆數) </summary>
        private int _pageSize;

        /// <summary> 目前第幾頁 </summary>
        private int _pageIndex;

        /// <summary> 總筆數 </summary>
        private int _totalCount;

        /// <summary> 一頁顯示幾筆資料(單頁可容納之資料筆數) </summary>
        public int PageSize { get { return _pageSize; } set { _pageSize = value; } }

        /// <summary> 目前第幾頁 </summary>
        public int PageIndex { get { return _pageIndex; } set { _pageIndex = value; } }
       
        /// <summary> 總筆數 </summary>
        public int TotalCount { get { return _totalCount; } set { _totalCount = value; } }

        /// <summary> 總頁數 </summary>
        public int GetTotalPageCount
        {
            get
            {
                //_pageSize 為0時要改為1，避免發生除0錯誤
                if (_pageSize == 0)
                    _pageSize = 1;
                //無條件進位(double精確度不夠)
                //return (int)Math.Ceiling((double)(_totalCount / _pageSize));
                return (int)Math.Ceiling(_totalCount / (decimal)_pageSize);
            }
        }
        #endregion

        #region Public Methods
        public int GetStartIndex()
        {
            //不可小於0
            if (_pageIndex < 1)
            {
                _pageIndex = 1;
            }
            return (_pageIndex - 1) * _pageSize;
        }
        #endregion

    }
}
