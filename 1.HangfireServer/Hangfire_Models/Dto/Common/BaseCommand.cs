namespace Hangfire_Models.Dto.Common
{
    public abstract class BaseCommand
    {
        public string Type => GetType().Name;
    }
}
