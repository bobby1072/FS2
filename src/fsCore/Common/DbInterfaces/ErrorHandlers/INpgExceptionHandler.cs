namespace Common.DbInterfaces.ErrorHandlers
{
    public interface INpgExceptionHandler
    {
        Task<(int, string)?> HandleException<T>(T exception) where T : Exception;
    }
}