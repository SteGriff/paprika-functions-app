using Microsoft.WindowsAzure.Storage.Table;
using PaprikaFunctionsApp.Common.Extensions;
using System;

namespace PaprikaFunctionsApp.Common.Models
{
    public class UserEntity : TableEntity
    {
        public UserEntity(string username, string passwordEnc, DateTime registrationDate)
        {
            this.PartitionKey = username;
            this.RowKey = registrationDate.ToIso8601();
            this.RegistrationDate = registrationDate;
            this.EncryptedPassword = passwordEnc;
        }

        public UserEntity() { }

        public DateTime RegistrationDate { get; set; }
        public string Salt { get { return RowKey; } }
        public string EncryptedPassword { get; set; }
    }
}
