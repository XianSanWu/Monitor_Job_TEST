namespace Services.Job.Interfaces
{
    public interface IRecurringJob
    {
        string JobId { get; }
        string CronExpression { get; }
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }

}
