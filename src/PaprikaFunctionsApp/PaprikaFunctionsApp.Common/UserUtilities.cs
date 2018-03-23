using Microsoft.WindowsAzure.Storage.Table;
using PaprikaFunctionsApp.Common.Extensions;
using PaprikaFunctionsApp.Common.Interfaces;
using PaprikaFunctionsApp.Common.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PaprikaFunctionsApp.Common
{
    public class UserUtilities
    {
        private AzureStorageProvider _storageProvider;
        const string USERS = "users";

        public ITraceWriter Logger { get; set; }

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
            var userTable = tableAccess.GetTable(USERS);
            var query = new TableQuery<UserEntity>() { FilterString = TableQuery.GenerateFilterCondition("PartitionKey", "eq", username) };
            var results = userTable.ExecuteQuerySegmentedAsync(query, new TableContinuationToken()).Result;
            var user = results.FirstOrDefault();
            return user;
        }

        public Status<UserEntity> UserExists(string username)
        {
            var theUser = GetUser(username);
            if (theUser == null)
            {
                return new Status<UserEntity>(false);
            }
            else
            {
                return new Status<UserEntity>(true, theUser);
            }
        }

        public async Task<Status<string>> CreateUserAsync(string username, string passwordPlain, bool isAnon)
        {
            try
            {
                var tableAccess = new TableUtilities(_storageProvider);
                var userTable = tableAccess.GetTable(USERS);

                var newUserEntity = new UserEntity(username, passwordPlain, isAnon);

                var insert = TableOperation.Insert(newUserEntity, true);
                await userTable.ExecuteAsync(insert);
            }
            catch (Exception ex)
            {
                return new Status<string>(false, ex.Message);
            }
            return new Status<string>(true);
        }
        
        public Status<string> ValidatePassword(string newPassword)
        {
            if (newPassword.Contains(HttpRequestMessageExtensions.IdentifierSeparator))
            {
                return new Status<string>(false, "Your password cannot include " + HttpRequestMessageExtensions.IdentifierSeparator);
            }
            else if (newPassword.Length < 7)
            {
                return new Status<string>(false, "Password must be 7 characters or longer.");
            }
            return new Status<string>(true);
        }

        public Status<string> ValidateUsername(string newUsername)
        {
            if (newUsername.Contains(HttpRequestMessageExtensions.IdentifierSeparator))
            {
                return new Status<string>(false, "Your username cannot include " + HttpRequestMessageExtensions.IdentifierSeparator);
            }
            else if (newUsername.Length < 2)
            {
                return new Status<string>(false, "Username must be 2 characters or longer.");
            }
            else if (newUsername.StartsWith("User"))
            {
                return new Status<string>(false, "Sorry, a custom username cannot start with 'User'");
            }
            return new Status<string>(true);
        }

        public async Task<Status<string>> RenameUserAsync(string oldUsername, string newUsername, string newPassword)
        {
            try
            {
                var usernameResult = ValidateUsername(newUsername);
                if (!usernameResult.Success)
                {
                    return usernameResult;
                }

                var passwordResult = ValidatePassword(newPassword);
                if (!passwordResult.Success)
                {
                    return passwordResult;
                }

                //Create a new user record
                // and associate the existing grammar cache and blob with the new record
                var userCreationResult = await CreateUserAsync(newUsername, newPassword, false);
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

        public async Task<Status<string>> UpdateUserAsync(UserEntity user)
        {
            try
            {
                var tableAccess = new TableUtilities(_storageProvider);
                var userTable = tableAccess.GetTable(USERS);
                user.Sanitise();
                var update = TableOperation.Merge(user);
                await userTable.ExecuteAsync(update);
            }
            catch (Exception ex)
            {
                if (Logger != null)
                {
                    Logger.Error("UpdateUserAsync Exception", ex);
                }
                return new Status<string>(false, ex.Message);
            }
            return new Status<string>(true);
        }
    }
}
