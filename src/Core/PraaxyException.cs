using System;
using System.Net;

namespace Core
{
    public class PraaxyException : Exception
    {
        public string Url { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public PraaxyException()
        {}

        public PraaxyException(string url, string message, HttpStatusCode statusCode) : base(message)
        {
            Url = url;
            StatusCode = statusCode;
        }
    }
}