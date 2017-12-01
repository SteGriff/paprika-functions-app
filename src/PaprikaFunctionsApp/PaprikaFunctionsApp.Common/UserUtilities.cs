using Microsoft.WindowsAzure.Storage.Table;
using PaprikaFunctionsApp.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaprikaFunctionsApp.Common
{
    public static class UserUtilities
    {
        public static string GetFilenameForUser(string username)
        {
            return username + ".txt";
        }

        public static UserEntity GetUser(string username)
        {
            var userTable = TableUtilities.GetTable("users");
            var query = new TableQuery<UserEntity>() { FilterString = TableQuery.GenerateFilterCondition("username", "eq", username) };
            var results = userTable.ExecuteQuerySegmentedAsync(query, new TableContinuationToken()).Result;
            var user = results.FirstOrDefault();
            return user;
        }

    }
}
