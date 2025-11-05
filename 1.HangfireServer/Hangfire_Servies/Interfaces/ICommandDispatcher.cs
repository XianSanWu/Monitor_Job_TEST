using Hangfire_Models.Dto.Requests;

namespace Hangfire_Services.Interfaces
{
    public interface ICommandDispatcher
    {
        Task DispatchAsync(JobExecutionContext context);
    }
}
