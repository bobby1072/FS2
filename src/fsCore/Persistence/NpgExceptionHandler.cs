using System.Net;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Abstract.ErrorHandler;

namespace Persistence
{
    public class NpgExceptionHandler : INpgExceptionHandler
    {
        public NpgExceptionHandler() { }
        public async Task<(int, string)?> HandleException<T>(T exception) where T : Exception
        {
            if (exception is DbUpdateException npgException)
            {
                if (npgException.InnerException is Npgsql.PostgresException postgresException)
                {
                    if (postgresException.SqlState == DbConstants.ErrorCodesAndMessages.UniqueViolation) return ((int)HttpStatusCode.UnprocessableEntity, DbConstants.ErrorCodesAndMessages.UniqueViolationMessage);
                }
            }
            return null;
        }
    }
}