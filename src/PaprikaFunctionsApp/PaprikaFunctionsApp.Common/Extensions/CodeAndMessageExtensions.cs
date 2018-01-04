using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace PaprikaFunctionsApp.Common.Extensions
{
    public static class CodeAndMessageExtensions
    {
        public static HttpStatusCode GetHttpStatusCode(this CodeAndMessage codeAndMessage)
        {
            return (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), codeAndMessage.Code.ToString());
        }
    }
}
