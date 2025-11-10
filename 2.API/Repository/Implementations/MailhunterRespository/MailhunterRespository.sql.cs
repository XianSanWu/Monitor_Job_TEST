using Models.Entities.Requests;
using Repository.Interfaces;
using System.Text;
using static Models.Entities.Requests.BatchIdAppMhResultSuccessCountEntityRequest;

namespace Repository.Implementations.MailhunterRespository
{
    public partial class MailhunterRespository : BaseRepository, IMailhunterRespository
    {
        private void QueryTodayAppMhProject()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            _sqlStr = new StringBuilder();
            _sqlStr.AppendLine("SELECT * FROM app_mh_project WHERE create_date >= @Today AND create_date < @Tomorrow");

            _sqlParams?.Add("Today", today);
            _sqlParams?.Add("Tomorrow", tomorrow);
        }

        private void QueryBatchIdAppMhResultSuccessCount(BatchIdAppMhResultSuccessCountEntitySearchListFieldModelRequest req)
        {
            _sqlStr = new StringBuilder();
            _sqlStr.AppendLine($"SELECT project_id, COUNT(1) AS SuccessCount FROM app_mh_result_{req.ProjectSplitTableId} WITH(NOLOCK) WHERE 1=1");

            _sqlStr.AppendLine("project_id = @ProjectId AND result_status = 0 GROUP BY batch_id");
            _sqlParams?.Add("ProjectId", req.ProjectId);
        }
    }
}
