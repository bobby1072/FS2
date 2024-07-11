namespace Persistence.EntityFramework.Abstract.ErrorHandler
{
    public interface INpgExceptionHandler
    {
        Task<(int, string)?> HandleException<T>(T exception) where T : Exception;
    }
}