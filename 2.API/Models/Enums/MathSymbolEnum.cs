using System.ComponentModel;

namespace Models.Enums
{
    /// <summary>
    /// 數學符號
    /// </summary>
    public class MathSymbolEnum
    {
        [Description("空")]
        public static readonly MathSymbolEnum None = new("", "空");

        [Description("加")]
        public static readonly MathSymbolEnum Plus = new("+", "加");

        [Description("減")]
        public static readonly MathSymbolEnum Minus = new("-", "減");

        [Description("乘")]
        public static readonly MathSymbolEnum Times = new("*", "乘");

        [Description("除")]
        public static readonly MathSymbolEnum Divided = new("/", "除");

        [Description("不等於")]
        public static readonly MathSymbolEnum NotEqual = new("!=", "不等於");

        [Description("等於")]
        public static readonly MathSymbolEnum Equal = new("=", "等於");

        [Description("大於")]
        public static readonly MathSymbolEnum GreaterThan = new(">", "大於");

        [Description("大於等於")]
        public static readonly MathSymbolEnum GreaterThanOrEqual = new(">=", "大於等於");

        [Description("小於")]
        public static readonly MathSymbolEnum LessThan = new("<", "小於");

        [Description("小於等於")]
        public static readonly MathSymbolEnum LessThanOrEqual = new("<=", "小於等於");

        [Description("合集")]
        public static readonly MathSymbolEnum In = new("IN", "合集");

        [Description("模糊查詢")]
        public static readonly MathSymbolEnum Like = new("LIKE", "模糊查詢");

        public string Symbol { get; }
        public string Description { get; }

        private MathSymbolEnum(string symbol, string description)
        {
            Symbol = symbol;
            Description = description;
        }

        public override string ToString() => Symbol;

        public static MathSymbolEnum? FromName(string name)
        {
            var field = typeof(MathSymbolEnum).GetField(name,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            return field?.GetValue(null) as MathSymbolEnum;
        }
    }
}
