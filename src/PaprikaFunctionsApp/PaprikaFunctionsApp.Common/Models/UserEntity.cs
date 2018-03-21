using Microsoft.WindowsAzure.Storage.Table;
using PaprikaFunctionsApp.Common.Extensions;
using System;

namespace PaprikaFunctionsApp.Common.Models
{
    public class UserEntity : TableEntity
    {
        public UserEntity(string username, string passwordPlain, bool isAnon)
        {
            DateTime registrationDate = DateTime.Now;
            string passwordSecured = CryptoKey.DeriveKey(passwordPlain, registrationDate.ToString("O"));

            this.PartitionKey = username;
            this.RowKey = registrationDate.ToIso8601();
            this.RegistrationDate = registrationDate;
            this.EncryptedPassword = passwordSecured;
            this.IsAnon = isAnon;
        }

        public UserEntity() { }

        public DateTime RegistrationDate { get; set; }
        public string EncryptedPassword { get; set; }
        public bool IsAnon { get; set; }

        public string OAuthToken { get; set; }
        public string OAuthTokenSecret { get; set; }
        public string TwitterUsername { get; set; }
        public long TwitterId { get; set; }
    }
}
