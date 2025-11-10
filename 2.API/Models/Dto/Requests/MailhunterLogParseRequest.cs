using Models.Common;

namespace Models.Dto.Requests
{

    public class MailhunterLogParseRequest
    {
        public class MailhunterLogParseSearchListRequest : BaseSearchModel
        {
            public MailhunterLogParseSearchListFieldModelRequest? FieldModel { get; set; }
        }

        public class MailhunterLogParseSearchListFieldModelRequest
        {
            public string? LogContent { get; set; }
            public List<string>? LogContentList { get; set; }

        }

    }
}
