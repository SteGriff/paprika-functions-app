using Microsoft.Azure.WebJobs;
using PaprikaFunctionsApp.Common;
using PaprikaFunctionsApp.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PaprikaFunctionsApp
{
    public class Authenticator
    {
        public string Username { get; set; }

        public Status<HttpResponseMessage> IsAuthorised([HttpTrigger]HttpRequestMessage req)
        {
            Username = "";
            if (req.Headers.Contains("username"))
            {
                Username = req.Headers.GetValues("username").FirstOrDefault();
            }
            else
            {
                return new Status<HttpResponseMessage>(req.CreateResponse(HttpStatusCode.Unauthorized, "No username received"), false);
            }

            string plainPasswordString;
            if (req.Headers.Contains("password"))
            {
                plainPasswordString = req.Headers.GetValues("password").FirstOrDefault();
            }
            else
            {
                return new Status<HttpResponseMessage>(req.CreateResponse(HttpStatusCode.Unauthorized, "No password received"), false);
            }

            //Get the user and check their auth
            var user = UserUtilities.GetUser(Username);
            bool isAuthed = false;
            if (user != null)
            {
                var encryptedPassword = Encryptor.Encrypt(plainPasswordString, user.RowKey);
                isAuthed = encryptedPassword == user.EncryptedPassword;
            }

            //If either the user is null or the password was wrong, display the same message
            // (this is an Infosec measure to avoid revealing existence of usernames)
            if (!isAuthed)
            {
                return new Status<HttpResponseMessage>(req.CreateResponse(HttpStatusCode.Unauthorized, "Incorrect username/password combination"), false);
            }

            return new Status<HttpResponseMessage>(true);
        }
    }
}
