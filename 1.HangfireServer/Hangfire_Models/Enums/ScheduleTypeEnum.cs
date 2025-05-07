
using System.ComponentModel;

namespace Hangfire_Models.Enums
{

    /// <summary>
    /// 自訂義執行排程
    /// </summary>
    public enum ScheduleTypeEnum
    {
        [Description("更新流程狀態任務")]
        UpdateWorkflowStatusJob
    }

}
