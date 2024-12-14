namespace fsCore.Persistence.Migration
{
    public interface IMigrator
    {
        public Task Migrate();
    }
}