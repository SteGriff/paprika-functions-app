using System;
using System.Collections.Generic;
using System.Text;

namespace PaprikaFunctionsApp.Common.Models
{
    public class BaseCredentials
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public BaseCredentials(string name, string password)
        {
            Name = name;
            Password = password;
        }
    }
}
