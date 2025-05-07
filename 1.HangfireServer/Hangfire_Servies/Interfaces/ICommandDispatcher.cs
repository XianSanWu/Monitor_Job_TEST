using Hangfire_Models.Dto.Requests;

namespace Hangfire_Servies.Interfaces
{
    public interface ICommandDispatcher
    {
        Task DispatchAsync(JobExecutionContext context);
    }
}
