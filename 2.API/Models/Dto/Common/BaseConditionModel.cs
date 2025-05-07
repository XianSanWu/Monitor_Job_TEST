using Models.Enums;

namespace Models.Dto.Common
{
    public class BaseConditionModel
    {
        /// <summary> 內部條件(AND/OR/None) </summary>
        public LogicOperatorEnum InsideLogicOperator { get; set; }

        /// <summary> 外部條件(AND/OR/None) </summary>
        public LogicOperatorEnum GroupLogicOperator { get; set; }
    }
}
