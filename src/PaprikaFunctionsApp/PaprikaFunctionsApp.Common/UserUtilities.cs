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

        public async Task<Status<string>> CreateUserAsync(string username, string passwordPlain)
        {
            try
            {
                var tableAccess = new TableUtilities(_storageProvider);
                var userTable = tableAccess.GetTable("users");

                var newUserEntity = new UserEntity(username, passwordPlain);

                var insert = TableOperation.Insert(newUserEntity, true);
                await userTable.ExecuteAsync(insert);
            }
            catch (Exception ex)
            {
                return new Status<string>(false, ex.Message);
            }
            return new Status<string>(true);
        }
        
        public async Task<Status<string>> RenameUserAsync(string oldUsername, string newUsername, string newPassword)
        {
            try
            {
                //Create a new user record
                // and associate the existing grammar cache and blob with the new record
                var userCreationResult = await CreateUserAsync(newUsername, newPassword);
                if (!userCreationResult.Success)
                {
                    return userCreationResult;
                }

                //Reassociate cache
                var gc = new GrammarCache(_storageProvider);
                gc.CopyCache(oldUsername, newUsername);

                //Reassociate blob
                var gb = new GrammarBlob(_storageProvider);
                await gb.CopyGrammar(oldUsername, newUsername);
            }
            catch (Exception ex)
            {
                return new Status<string>(false, ex.Message);
            }
            return new Status<string>(true);
        }
    }
}
