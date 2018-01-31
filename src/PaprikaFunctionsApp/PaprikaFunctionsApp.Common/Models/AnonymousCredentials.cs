using System;

namespace PaprikaFunctionsApp.Common.Models
{
    public class AnonymousCredentials : BaseCredentials
    {
        public AnonymousCredentials()
        {
            var rando = new Random();
            rando.Next();
            
            //Make a new random GUID and get the first chunk
            var newGuid = Guid.NewGuid();
            var guidPart = newGuid.ToString().Split(new[] { '-' })[0];

            const string root = "User";
            Name = root + rando.Next(100, 1000) + guidPart;
            Password = Name + "password";
        }

    }
}
