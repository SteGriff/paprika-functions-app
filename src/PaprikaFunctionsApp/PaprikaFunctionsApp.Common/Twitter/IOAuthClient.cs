using System;
using System.Collections.Generic;
using System.Text;

namespace PaprikaFunctionsApp.Common.Twitter
{
    public interface IOAuthClient
    {
        Uri GetAuthorizeUri(string consumerKey, string consumerSecret, Uri callback);
        TwitterUser GetUser(string consumerKey, string consumerSecret, string oauthToken, string oauthVerifier);
    }
}
