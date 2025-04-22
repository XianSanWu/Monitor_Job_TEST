using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Services.Job.Interfaces;

namespace Services.Job
{
    public static class HangfireJobRegistrar
    {
        public static void RegisterAllRecurringJobs(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var jobs = scope.ServiceProvider.GetServices<IRecurringJob>();

            foreach (var job in jobs)
            {
                RecurringJob.AddOrUpdate(
                    job.JobId,
                    () => job.ExecuteAsync(default),
                    job.CronExpression
                );
            }
        }
    }
}
