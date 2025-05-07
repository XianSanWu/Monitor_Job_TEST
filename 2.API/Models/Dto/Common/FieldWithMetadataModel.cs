using Models.Enums;

namespace Models.Dto.Common
{
    public class FieldWithMetadataModel
    {
        /// <summary>數學符號</summary>
        public MathSymbolEnum? MathSymbol { get; set; }
        
        /// <summary> 值 </summary>
        public object? Value { get; set; } = "";
    }

}
