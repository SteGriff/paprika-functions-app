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

            PartitionKey = username;
            RowKey = registrationDate.ToIso8601();
            RegistrationDate = registrationDate;
            EncryptedPassword = passwordSecured;
            IsAnon = isAnon;
        }

        public UserEntity() { }

        public DateTime RegistrationDate { get; set; }
        public string EncryptedPassword { get; set; }
        public bool IsAnon { get; set; }

        public string OAuthToken { get; set; }
        public string OAuthTokenSecret { get; set; }
        public string TwitterUsername { get; set; }
        public long TwitterId { get; set; }

        public int ScheduleMinuteInterval { get; set; }
        public string ScheduleQuery { get; set; }
        public bool ScheduleEnable { get; set; }
        public DateTime ScheduleLastPosted { get; set; }

        public void MergeFromTwitterModel(UserTwitterViewModel twitterModel)
        {
            ScheduleEnable = twitterModel.ScheduleEnable;
            ScheduleMinuteInterval = twitterModel.ScheduleHourInterval * 60;
            ScheduleQuery = twitterModel.ScheduleQuery;
        }

        public void Sanitise()
        {
            if (ScheduleLastPosted.Year < 2000)
            {
                ScheduleLastPosted = DateTime.Now.AddDays(-1);
            }
        }

        public void DisconnectTwitter()
        {
            OAuthToken = "";
            OAuthTokenSecret = "";
            TwitterUsername = "";
            TwitterId = 0;
        }

        public override string ToString()
        {
            return string.Format("UserEntity PKey:{4}, RowKey:{5}, @{0}, {1}, '{2}' every {3} mins",
                TwitterUsername,
                ScheduleEnable ? "Enabled" : "Disabled",
                ScheduleQuery,
                ScheduleMinuteInterval,
                PartitionKey,
                RowKey);
        }
    }
}
