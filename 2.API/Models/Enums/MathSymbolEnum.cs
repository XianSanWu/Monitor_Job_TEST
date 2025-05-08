using System.ComponentModel;

namespace Models.Enums
{
    /// <summary>
    /// 數學符號
    /// </summary>
    public class MathSymbolEnum
    {
        [Description("空")]
        public static readonly MathSymbolEnum None = new("None", "", "空");

        [Description("加")]
        public static readonly MathSymbolEnum Plus = new("Plus","+", "加");

        [Description("減")]
        public static readonly MathSymbolEnum Minus = new("Minus","-", "減");

        [Description("乘")]
        public static readonly MathSymbolEnum Times = new("Times", "*", "乘");

        [Description("除")]
        public static readonly MathSymbolEnum Divided = new("Divided", "/", "除");

        [Description("不等於")]
        public static readonly MathSymbolEnum NotEqual = new("NotEqual", "!=", "不等於");

        [Description("等於")]
        public static readonly MathSymbolEnum Equal = new("Equal", "=", "等於");

        [Description("大於")]
        public static readonly MathSymbolEnum GreaterThan = new("GreaterThan", ">", "大於");

        [Description("大於等於")]
        public static readonly MathSymbolEnum GreaterThanOrEqual = new("GreaterThanOrEqual", ">=", "大於等於");

        [Description("小於")]
        public static readonly MathSymbolEnum LessThan = new("LessThan", "<", "小於");

        [Description("小於等於")]
        public static readonly MathSymbolEnum LessThanOrEqual = new("LessThanOrEqual", "<=", "小於等於");

        [Description("合集")]
        public static readonly MathSymbolEnum In = new("In", "IN", "合集");

        [Description("模糊查詢")]
        public static readonly MathSymbolEnum Like = new("Like", "LIKE", "模糊查詢");

        [Description("最大值")]
        public static readonly MathSymbolEnum Max = new("Max", "Max", "最大值");

        [Description("最小值")]
        public static readonly MathSymbolEnum Min = new("Min", "Min", "最小值");

        public string Key { get; }
        public string Symbol { get; }
        public string Description { get; }

        private MathSymbolEnum(string key, string symbol, string description)
        {
            Key = key;
            Symbol = symbol;
            Description = description;
        }
        public override string ToString() => Key;

        public static MathSymbolEnum? FromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var field = typeof(MathSymbolEnum)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .FirstOrDefault(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));

            return field?.GetValue(null) as MathSymbolEnum;
        }

    }
}
