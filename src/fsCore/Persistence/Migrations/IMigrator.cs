namespace Persistence.Migrations
{
    public interface IMigrator
    {
        public Task Migrate();
    }
}