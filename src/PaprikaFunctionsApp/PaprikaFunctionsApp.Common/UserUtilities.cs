using Microsoft.WindowsAzure.Storage.Table;
using PaprikaFunctionsApp.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaprikaFunctionsApp.Common
{
    public class UserUtilities
    {
        private AzureStorageProvider _storageProvider;

        public UserUtilities(AzureStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public string GetFilenameForUser(string username)
        {
            return username + ".txt";
        }

        public UserEntity GetUser(string username)
        {
            var tableAccess = new TableUtilities(_storageProvider);
            var userTable = tableAccess.GetTable("users");
            var query = new TableQuery<UserEntity>() { FilterString = TableQuery.GenerateFilterCondition("PartitionKey", "eq", username) };
            var results = userTable.ExecuteQuerySegmentedAsync(query, new TableContinuationToken()).Result;
            var user = results.FirstOrDefault();
            return user;
        }

    }
}
