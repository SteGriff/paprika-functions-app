using System;
using System.Collections.Generic;
using System.Text;

namespace PaprikaFunctionsApp.Common.Models
{
    public class AnonymousCredentials : BaseCredentials
    {
        public AnonymousCredentials()
        {
            var rando = new Random();
            rando.Next();

            string root = rando.Next(0, 2) == 0
                ? "Anon"
                : "Rando";

            //Make a new random GUID and get the first chunk
            var newGuid = Guid.NewGuid();
            var guidPart = newGuid.ToString().Split(new[] { '-' })[0];

            Name = root + rando.Next(100, 1000) + guidPart;
            Password = Name + "password";
        }

    }
}
