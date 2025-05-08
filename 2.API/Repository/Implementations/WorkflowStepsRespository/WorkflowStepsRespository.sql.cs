using Dapper;
using Models.Dto.Common;
using Models.Entities;
using Models.Enums;
using Repository.Interfaces;
using System.Text;
using Utilities.Utilities;
using static Models.Dto.Requests.WorkflowStepsRequest;

namespace Repository.Implementations.WorkflowStepsRespository
{
    public partial class WorkflowStepsRespository : BaseRepository, IWorkflowStepsRespository
    {
        /// <summary>
        /// 工作進度查詢DB
        /// </summary>
        /// <param name="searchReq"></param>
        private void QueryWorkflowSql(WorkflowStepsSearchListRequest searchReq)
        {
            _sqlStr = new StringBuilder();
            _sqlStr?.Append(@" SELECT * FROM Workflow WITH (NOLOCK) WHERE 1=1 ");

            _sqlParams = new DynamicParameters();

            #region  處理 FieldModel 輸入框 (模糊查詢)
            var columnsWithValues = Reflection.GetValidColumnsWithValues(searchReq.FieldModel);

            foreach (var column in columnsWithValues)
            {
                AppendFilterCondition(column.Key, string.Empty, column.Value, null); // 不需要驗證欄位是否有效，因為已從 model 取得
            }
            #endregion

            #region  處理 FilterModel Grid (模糊查詢)
            var validColumns = Reflection.GetValidColumns<WorkflowEntity>();

            if (searchReq.FilterModel != null)
            {
                foreach (var filter in searchReq.FilterModel)
                {
                    AppendFilterCondition(filter.Key, filter.MathSymbol?.ToString() ?? string.Empty, filter.Value, validColumns);
                }
            }
            #endregion

            #region  初版模糊查詢
            //// 獲取模型的有效欄位（使用反射）//輸入框
            //var columnsWithValues = Reflection.GetValidColumnsWithValues(searchReq.FieldModel);

            //foreach (var column in columnsWithValues)
            //{
            //    if (column.Value != null)
            //    {
            //        if (column.Key.EndsWith("At", StringComparison.OrdinalIgnoreCase))
            //        {
            //            // 假設此欄位在資料庫中是 DATETIME 類型，進行模糊查詢
            //            _sqlStr.Append($" AND CONVERT(VARCHAR, {column.Key}, 121) LIKE @{column.Key} ");
            //            _sqlParams.Add($"@{column.Key}", $"%{column.Value}%");
            //        }
            //        else
            //        {
            //            _sqlStr.Append($" AND {column.Key} LIKE @{column.Key} ");
            //            _sqlParams.Add($"@{column.Key}", $"%{column.Value}%");
            //        }
            //    }
            //    //Console.WriteLine($"Key: {column.Key}, Value: {column.Value}");
            //}


            //// 獲取模型的有效欄位（使用反射）//grid
            //// 根據模型類型來查找有效欄位，只有模型中的屬性會被考慮，避免sql注入風險
            //var validColumns = Reflection.GetValidColumns<WorkflowEntity>();

            ////設定grid欄位模糊查詢
            //if (searchReq.FilterModel != null)
            //{
            //    foreach (var filter in searchReq.FilterModel)
            //    {
            //        if (string.IsNullOrWhiteSpace(filter.Key) || string.IsNullOrWhiteSpace(filter.Value) || !validColumns.Contains(filter.Key, StringComparer.OrdinalIgnoreCase))
            //        {
            //            continue;
            //        }

            //        // 如果 filter.Key 以 "At" 結尾，進行 datetime 處理
            //        if (filter.Key.EndsWith("At", StringComparison.OrdinalIgnoreCase))
            //        {
            //            // 假設此欄位在資料庫中是 DATETIME 類型，進行模糊查詢
            //            _sqlStr.Append($" AND CONVERT(VARCHAR, {filter.Key}, 121) LIKE @{filter.Key} ");
            //            _sqlParams.Add($"@{filter.Key}", $"%{filter.Value}%");
            //        }
            //        else
            //        {
            //            // 否則，正常處理字串模糊查詢
            //            _sqlStr.Append($" AND {filter.Key} LIKE @{filter.Key} ");
            //            _sqlParams.Add($"@{filter.Key}", $"%{filter.Value}%");
            //        }
            //    }
            //}
            #endregion

            #region  設定SQL排序
            if (searchReq.SortModel != null &&
                !string.IsNullOrWhiteSpace(searchReq.SortModel.Key) &&
                searchReq.SortModel.Value != null &&
                validColumns.Contains(searchReq.SortModel.Key, StringComparer.OrdinalIgnoreCase)
                )
            {
                _sqlOrderByStr = $" ORDER BY {searchReq.SortModel.Key} {searchReq.SortModel.Value} ";
            }
            else
            {
                _sqlOrderByStr = $" ORDER BY CreateAt DESC ";
            }
            #endregion

        }

        /// <summary>
        /// 工作流程資料更新(多筆)DB
        /// </summary>
        /// <param name="fieldReq"></param>
        /// <param name="conditionReq"></param>
        private void UpdateWorkflowSql(WorkflowStepsUpdateFieldRequest fieldReq, List<WorkflowStepsUpdateConditionRequest> conditionReq)
        {
            _sqlStr = new StringBuilder();
            _sqlStr?.Append("UPDATE Workflow SET ");

            _sqlParams = new DynamicParameters();

            #region 處理欄位更新

            var columnsWithValues = Reflection.GetModelPropertiesWithValues(fieldReq);
            var tempList = new List<string>();

            #region 排除不可被更新欄位（例如主鍵）
            var excludeFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "SN","WorkflowUuid",
                //"CreateAt","UpdateAt","JourneyCreateAt","JourneyUpdateAt","GroupSendCreateAt","GroupSendUpdateAt"
            };
            #endregion

            #region 組合要更新的欄位
            foreach (var column in columnsWithValues)
            {
                var columnName = column.Key;
                var (type, value) = column.Value;


                DateTime? date = null;
                try
                {
                    if (DateTime.TryParse(value?.ToString(), out DateTime parsedDate))
                    {
                        date = parsedDate;
                    }
                }
                catch (Exception)
                {
                    //不理會
                }

                if (excludeFields.Contains(columnName) ||
                    value == null ||
                    string.IsNullOrWhiteSpace(value?.ToString()) ||
                    (date)?.ToString("yyyy/MM/dd") == DateTime.MinValue.ToString("yyyy/MM/dd"))
                    continue;

                var paramName = $"@{columnName}";

                #region 處理特殊型別（可自訂）
                // 處理特殊型別（可自訂）
                object? dbValue = value;

                //if (value == null)
                //{
                //    dbValue = DBNull.Value;
                //}
                //else if (type == typeof(bool))
                //{
                //    dbValue = (bool)value ? 1 : 0; // 儲存成 bit/int
                //}
                //else if (type.IsEnum)
                //{
                //    dbValue = value.ToString(); // 儲存 enum 名稱，也可改成數值
                //}
                //else if (type == typeof(DateTime) || type == typeof(DateTime?))
                //{
                //    // 可加格式化或轉 UTC，看需求
                //    dbValue = value;
                //}
                #endregion

                tempList.Add($"{columnName} = {paramName}");
                _sqlParams.Add(paramName, dbValue);
            }

            _sqlStr?.Append(string.Join(", ", tempList));
            #endregion

            #endregion

            #region 處理欄位條件（多組 OR 群組，每組內 AND 條件）
            if (conditionReq != null && conditionReq.Any())
            {
                var whereGroups = new List<string>();
                int groupIndex = 0;

                foreach (var group in conditionReq)
                {
                    var groupConditions = new List<string>();

                    var props = Reflection.GetModelPropertiesWithValues(group);
                    foreach (var prop in props)
                    {
                        var columnName = prop.Key;
                        var (type, value) = prop.Value;

                        if (value is not FieldWithMetadataModel meta || // 確保 meta 是正確的型別
                            string.IsNullOrWhiteSpace(meta.MathSymbol?.ToString()) ||
                            meta.Value == null ||
                            string.IsNullOrWhiteSpace(meta.Value?.ToString()))
                            continue;

                        switch (meta.MathSymbol.ToUpper())
                        {
                            case "MAX":
                            case "MIN":
                                var mathParamName = $"@cond_{groupIndex}_{columnName} ";
                                groupConditions.Add($"{columnName} {MathSymbolEnum.FromName(meta.MathSymbol)?.Symbol}({mathParamName}) ");
                                break;

                            case "IN":
                                // 特別排除 string 因為 string 也是 IEnumerable
                                if (meta.Value is IEnumerable<object> list && meta.Value is not string)
                                {
                                    var placeholders = new List<string>();
                                    int index = 0;

                                    foreach (var item in list)
                                    {
                                        var paramName = $"@cond_{groupIndex}_{columnName}_{index++}";
                                        placeholders.Add(paramName);
                                        _sqlParams?.Add(paramName, item);
                                    }

                                    if (placeholders.Count > 0)
                                    {
                                        groupConditions.Add($" {columnName} {MathSymbolEnum.FromName(meta.MathSymbol)?.Symbol} ({string.Join(", ", placeholders)}) ");
                                    }
                                }
                                break;

                            case "LIKE":
                                var likeParamName = $"@cond_{groupIndex}_{columnName} ";
                                if (columnName.EndsWith("At", StringComparison.OrdinalIgnoreCase))
                                {
                                    groupConditions.Add($" CONVERT(VARCHAR, {columnName}, 121) {MathSymbolEnum.FromName(meta.MathSymbol)?.Symbol} {likeParamName} ");
                                }
                                else
                                {
                                    groupConditions.Add($"{columnName} {MathSymbolEnum.FromName(meta.MathSymbol)?.Symbol} {likeParamName} ");
                                }

                                _sqlParams?.Add(likeParamName, $"%{value}%");
                                break;

                            default:
                                var paramKey = $"@cond_{groupIndex}_{columnName} ";
                                groupConditions.Add($" {columnName} {MathSymbolEnum.FromName(meta.MathSymbol)?.Symbol} {paramKey} ");
                                _sqlParams?.Add(paramKey, meta.Value); // 使用 meta.Value
                                break;
                        }
                    }

                    if (groupConditions.Count > 0)
                    {
                        LogicOperatorEnum opi = group.InsideLogicOperator;
                        string insideLogicOperator = opi == LogicOperatorEnum.None ? string.Empty : opi.ToString();

                        whereGroups.Add(" (" + string.Join($" {insideLogicOperator} ", groupConditions) + ") ");

                        LogicOperatorEnum opg = group.GroupLogicOperator;
                        string groupLogicOperator = opg == LogicOperatorEnum.None ? string.Empty : opg.ToString();
                        whereGroups.Add($" {groupLogicOperator} ");
                    }

                    groupIndex++;
                }

                if (whereGroups.Count > 0)
                {
                    // 移除最後一個如果是 "AND" 或 "OR"
                    var last = whereGroups[^1].Trim().ToUpper();
                    if (last == LogicOperatorEnum.AND.ToString() || last == LogicOperatorEnum.OR.ToString())
                    {
                        whereGroups.RemoveAt(whereGroups.Count - 1);
                    }

                    _sqlStr?.Append(" WHERE 1=1 AND ");
                    _sqlStr?.Append(string.Join(string.Empty, whereGroups));
                }

            }

            #endregion
        }

    }
}
