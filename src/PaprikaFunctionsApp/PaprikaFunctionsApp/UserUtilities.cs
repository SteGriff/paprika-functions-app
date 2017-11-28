using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaprikaFunctionsApp
{
    public static class UserUtilities
    {
        public static string GetFilenameForUser(string username)
        {
            return username + ".txt";
        }
    }
}
