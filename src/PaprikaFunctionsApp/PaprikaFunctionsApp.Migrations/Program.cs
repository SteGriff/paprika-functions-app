using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Table;
using PaprikaFunctionsApp.Common;
using PaprikaFunctionsApp.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PaprikaFunctionsApp.Migrations
{
    class Program
    {
        public static IConfigurationRoot Configuration;
        private static AzureStorageProvider _storageProvider;

        static void Main(string[] args)
        {
            _storageProvider = new AzureStorageProvider(GetConnectionString());
            var tableAccess = new TableUtilities(_storageProvider);
            var usersTable = tableAccess.GetTable("users");

            tableAccess.DropTableAsync("users").Wait();

            //Table has been dropped; recreate it
            usersTable = tableAccess.GetTable("users");
            MakeAndCheckUsers(usersTable);
        }

        private static void SetupConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        private static string GetConnectionString()
        {
            if (Configuration == null)
            {
                SetupConfiguration();
            }
            string conString = Configuration.GetConnectionString("PrimaryStorage");
            return conString;
        }

        private static void MakeAndCheckUsers(CloudTable usersTable)
        {
            CreateFakeUsers(usersTable);

            CheckFakeUsers(usersTable);

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static void CreateFakeUsers(CloudTable table)
        {
            var fakeNames = new List<string> { "adam", "beatrice", "charlie", "dave", "emily" };
            var regDates = new List<DateTime> {
                new DateTime(2017, 01, 01),
                new DateTime(2017, 01, 02),
                new DateTime(2017, 01, 03),
                new DateTime(2017, 01, 04),
                new DateTime(2017, 01, 05)
            };

            int i = 0;
            foreach (var name in fakeNames)
            {
                var regDate = regDates[i];
                string password = name + "password";
                string passwordEnc = Encryptor.Encrypt(password, regDate.ToString("O"));
                var newUser = new UserEntity(name, passwordEnc, regDate);
                var insert = TableOperation.Insert(newUser);
                table.ExecuteAsync(insert).Wait();
                i += 1;
            }
        }

        private static void CheckFakeUsers(CloudTable table)
        {
            var token = new TableContinuationToken();
            var lookup = new TableQuery<UserEntity>();
            var resultsAsync = table.ExecuteQuerySegmentedAsync(lookup, token);

            var result = resultsAsync.Result;
            foreach (var res in result)
            {
                Console.WriteLine("Key: {0}; Row: {1}", res.PartitionKey, res.RowKey);
            }
        }
    }
}
