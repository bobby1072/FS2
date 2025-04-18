﻿using System.Net;
namespace fsCore.Common.Misc
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public ApiException(string message = ErrorConstants.InternalServerError, HttpStatusCode status = HttpStatusCode.InternalServerError) : base(message)
        {
            StatusCode = status;
        }
        public ApiException(Exception innerException, string message = ErrorConstants.InternalServerError, HttpStatusCode status = HttpStatusCode.InternalServerError) : base(message, innerException)
        {
            StatusCode = status;
        }
    }
}