using Models.Dto.Common;

namespace Models.Dto.Requests
{
    public class WorkflowStepsRequest
    {
        #region 查詢用
        public class WorkflowStepsSearchListRequest : BaseSearchModel
        {
            public WorkflowStepsSearchListFieldModelRequest? FieldModel { get; set; }
        }

        public class WorkflowStepsSearchListFieldModelRequest
        {
            public string? Channel { get; set; }
        }
        #endregion

        #region 更新用

        #region 工作流程更新
        public class UpdateWorkflowListRequest
        {
            public WorkflowStepsUpdateFieldRequest FieldReq { get; set; } = new();
            public List<WorkflowStepsUpdateConditionRequest> ConditionReq { get; set; } = new();
        }
        #endregion

        #region 工作流程更新[欄位]
        /// <summary> 工作流程更新[欄位] </summary>
        public class WorkflowStepsUpdateFieldRequest
        {
            /// <summary>主鍵</summary>
            public int SN { get; set; }

            /// <summary>工作流程唯一識別碼</summary>
            public string? WorkflowUuid { get; set; }

            /// <summary>發送 UUID</summary>
            public string? SendUuid { get; set; }

            /// <summary>批次 ID</summary>
            public string? BatchId { get; set; }

            /// <summary>旅程 ID</summary>
            public string? ActivityId { get; set; }

            /// <summary>旅程名稱</summary>
            public string? ActivityName { get; set; }

            /// <summary>旅程狀態</summary>
            public string? ActivityStatus { get; set; }

            /// <summary>節點 ID</summary>
            public string? NodeId { get; set; }

            /// <summary>節點名稱</summary>
            public string? NodeName { get; set; }

            /// <summary>通道 (Email / SMS)</summary>
            public string? Channel { get; set; }

            /// <summary>通道類型 (EDM / 簡訊)</summary>
            public string? ChannelType { get; set; }

            /// <summary>上傳檔名</summary>
            public string? UploadFileName { get; set; }

            /// <summary>狀態</summary>
            public string? ProgressStatus { get; set; }

            /// <summary>建立時間</summary>
            public DateTime CreateAt { get; set; }

            /// <summary>更新時間</summary>
            public DateTime UpdateAt { get; set; }

            /// <summary>旅程建立時間</summary>
            public DateTime JourneyCreateAt { get; set; }

            /// <summary>旅程更新時間</summary>
            public DateTime JourneyUpdateAt { get; set; }

            /// <summary>群發建立時間</summary>
            public DateTime GroupSendCreateAt { get; set; }

            /// <summary>群發更新時間</summary>
            public DateTime GroupSendUpdateAt { get; set; }
        }
        #endregion

        #region 工作流程更新[條件]
        /// <summary>工作流程更新[條件]</summary>
        public class WorkflowStepsUpdateConditionRequest : BaseConditionModel
        {
            /// <summary>主鍵</summary>
            public FieldWithMetadataModel SN { get; set; } = new();

            /// <summary>工作流程唯一識別碼</summary>
            public FieldWithMetadataModel WorkflowUuid { get; set; } = new();

            /// <summary>發送 UUID</summary>
            public FieldWithMetadataModel SendUuid { get; set; } = new();

            /// <summary>批次 ID</summary>
            public FieldWithMetadataModel BatchId { get; set; } = new();

            /// <summary>旅程 ID</summary>
            public FieldWithMetadataModel ActivityId { get; set; } = new();

            /// <summary>旅程名稱</summary>
            public FieldWithMetadataModel ActivityName { get; set; } = new();

            /// <summary>旅程狀態</summary>
            public FieldWithMetadataModel ActivityStatus { get; set; } = new();

            /// <summary>節點 ID</summary>
            public FieldWithMetadataModel NodeId { get; set; } = new();

            /// <summary>節點名稱</summary>
            public FieldWithMetadataModel NodeName { get; set; } = new();

            /// <summary>通道 (EDM / SMS / APP_PUSH)</summary>
            public FieldWithMetadataModel Channel { get; set; } = new();

            /// <summary>通道類型 (旅程 / 群發)</summary>
            public FieldWithMetadataModel ChannelType { get; set; } = new();

            /// <summary>上傳檔名</summary>
            public FieldWithMetadataModel UploadFileName { get; set; } = new();

            /// <summary>狀態</summary>
            public FieldWithMetadataModel Status { get; set; } = new();

            /// <summary>建立時間</summary>
            public FieldWithMetadataModel CreateAt { get; set; } = new();

            /// <summary>更新時間</summary>
            public FieldWithMetadataModel UpdateAt { get; set; } = new();

            /// <summary>旅程建立時間</summary>
            public FieldWithMetadataModel JourneyCreateAt { get; set; } = new();

            /// <summary>旅程更新時間</summary>
            public FieldWithMetadataModel JourneyUpdateAt { get; set; } = new();

            /// <summary>群發建立時間</summary>
            public FieldWithMetadataModel GroupSendCreateAt { get; set; } = new();

            /// <summary>群發更新時間</summary>
            public FieldWithMetadataModel GroupSendUpdateAt { get; set; } = new();
        }
        #endregion

        #endregion

    }
}
