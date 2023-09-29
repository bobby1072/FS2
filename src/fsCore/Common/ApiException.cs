using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ApiException: Exception
    {
        public HttpStatusCode? StatusCode { get; set; }
        public ApiException(string? message, HttpStatusCode? status): base(message)
        {
            StatusCode = status;
        }
        public ApiException(HttpStatusCode? statusCode)
        {
            StatusCode = statusCode;
        }
    }
}
