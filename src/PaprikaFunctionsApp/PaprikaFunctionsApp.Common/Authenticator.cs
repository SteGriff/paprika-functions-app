using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaprikaFunctionsApp.Common
{
    public class Authenticator
    {
        public bool IsAuthorised(string username, string password)
        {
            return username == "stegriff";
        }
    }
}
