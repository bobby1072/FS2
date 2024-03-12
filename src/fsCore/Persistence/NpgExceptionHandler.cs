using Common.Dbinterfaces.ErrorHandlers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Persistence
{
    public class NpgExceptionHandler : INpgExceptionHandler
    {
        public NpgExceptionHandler() { }
        public async Task<(int, string)?> HandleException<T>(T exception) where T : Exception
        {
            if (exception is DbUpdateException npgException)
            {
                return (400, "hdrhgfdr");
            }
            return null;
        }
    }
}