namespace Models.Dto.Common
{
    public class FieldWithMetadataModel
    {
        /// <summary> 欄位名稱 </summary>
        public string? Key { get; set; } = "";

        /// <summary> 數學符號 </summary>
        public string? MathSymbol { get; set; } = ""; //MathSymbolEnum

        /// <summary> 值 </summary>
        public object? Value { get; set; } = "";
    }

}
