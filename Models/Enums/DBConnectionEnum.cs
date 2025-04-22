namespace Models.Enums
{
    /// <summary>
    /// 對應 appsettings.json 中 ConnectionStrings 區塊
    /// "ConnectionStrings": {
    ///   "DefaultConnection": "",    
    /// },
    /// </summary>
    public enum DBConnectionEnum
    {
        /// <summary> 預設 </summary>
        DefaultConnection,
        /// <summary> CDP </summary>
        Cdp,
        /// <summary> Mail_hunter </summary>
        Mail_hunter,
    }
}
