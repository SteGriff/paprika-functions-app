using System;
using System.Collections.Generic;
using System.Text;

namespace PaprikaFunctionsApp.Common.Extensions
{
    public static class DateExtensions
    {
        public static string ToIso8601(this DateTime theDate)
        {
            return theDate.ToString("O");
        }
    }
}
