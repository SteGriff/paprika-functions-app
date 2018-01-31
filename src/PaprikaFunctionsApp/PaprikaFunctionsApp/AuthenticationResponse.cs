using Microsoft.Azure.WebJobs;
using PaprikaFunctionsApp.Common;
using PaprikaFunctionsApp.Common.Models;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace PaprikaFunctionsApp
{
    public class AuthenticationResponse
    {
        public string Username { get; set; }
        
        public Status<HttpResponseMessage> Get(AzureStorageProvider storageProvider, [HttpTrigger]HttpRequestMessage req)
        {
            Username = "";
            if (req.Headers.Contains("username"))
            {
                Username = req.Headers.GetValues("username").FirstOrDefault();
                if (Username.ToLower() == "default")
                {
                    return new Status<HttpResponseMessage>(false, req.CreateResponse(HttpStatusCode.Forbidden, "This user is not enabled"));
                }
            }
            else
            {
                return new Status<HttpResponseMessage>(false, req.CreateResponse(HttpStatusCode.Forbidden, "No username received"));
            }

            string plainPasswordString;
            if (req.Headers.Contains("password"))
            {
                plainPasswordString = req.Headers.GetValues("password").FirstOrDefault();
            }
            else
            {
                return new Status<HttpResponseMessage>(false, req.CreateResponse(HttpStatusCode.Forbidden, "No password received"));
            }

            //Get the user and check their auth
            var userUtils = new UserUtilities(storageProvider);
            var user = userUtils.GetUser(Username);
            bool isAuthed = false;
            if (user != null)
            {
                var passwordSecured = CryptoKey.DeriveKey(plainPasswordString, user.RowKey);
                isAuthed = passwordSecured == user.EncryptedPassword;
            }

            //If either the user is null or the password was wrong, display the same message
            // (this is an Infosec measure to avoid revealing existence of usernames)
            if (!isAuthed)
            {
                return new Status<HttpResponseMessage>(false, req.CreateResponse(HttpStatusCode.Forbidden, "Incorrect username/password combination"));
            }

            return new Status<HttpResponseMessage>(true);
        }
    }
}
