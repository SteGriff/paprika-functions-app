using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Table;
using PaprikaFunctionsApp.Common;
using PaprikaFunctionsApp.Common.Behaviour;
using PaprikaFunctionsApp.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace PaprikaFunctionsApp.Migrations
{
    class Program
    {
        public static IConfigurationRoot Configuration;
        private static AzureStorageProvider _storageProvider;

        private const string USERS = "users";
        private const string GRAMMAR = "grammar";
        private const string DEFAULT_USER = "default";
        private const string STORAGE_EMULATOR = @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe";

        static void Main(string[] args)
        {
            string command = "";

            Output("Starting...");
            string connectionString = GetConnectionString();
            if (connectionString.Contains("UseDevelopmentStorage=true"))
            {
                Output("Development storage detected... starting emulator");
                var startEmulator = new ProcessStartInfo(STORAGE_EMULATOR, "start")
                {
                    UseShellExecute = true
                };
                Process.Start(startEmulator);
            }

            Output("Connection string is: {0}", connectionString);
            _storageProvider = new AzureStorageProvider(connectionString);
            var tableAccess = new TableUtilities(_storageProvider);

            //Ready
            PrintHelpText();

            while (command.ToLower() != "exit")
            {
                command = Prompt();
                switch (command.ToLower())
                {
                    case "drop users":
                    case "du":
                        Output("Dropping users table...");
                        var dropUsers = DropTable(USERS, tableAccess);
                        if (!dropUsers.Success) { Output(dropUsers.Attachment); }
                        break;
                    case "drop grammar":
                    case "dg":
                        Output("Dropping grammar table...");
                        var dropGrammar = DropTable(GRAMMAR, tableAccess);
                        if (!dropGrammar.Success) { Output(dropGrammar.Attachment); }
                        break;
                    case "gen users":
                    case "gu":
                        Output("Regenerating basic users data...");
                        CreateFakeUsers(tableAccess);
                        SelectAndPrint(tableAccess, USERS);
                        break;
                    case "gen grammar":
                    case "gg":
                        Output("Generating default grammar...");
                        var genGrammar = GenerateGrammar(tableAccess);
                        if (!genGrammar.Success) { Output(genGrammar.Attachment); }
                        break;
                    case "select users":
                    case "su":
                        SelectAndPrint(tableAccess, USERS);
                        break;
                    case "select grammar":
                    case "sg":
                        SelectAndPrint(tableAccess, GRAMMAR);
                        break;
                }
                Output("Finished operation");
            }

        }

        private static Status<string> DropTable(string tableName, TableUtilities tableAccess)
        {
            try
            {
                tableAccess.DropTableAsync(tableName).Wait();
            }
            catch (Exception ex)
            {
                return new Status<string>(false, ex.Message);
            }
            return new Status<string>(true);
        }

        private static void PrintHelpText()
        {
            Output();
            Output("Enter a command and press return:");
            Output("select users (su) - View a list of existing users");
            Output("drop users (du) - Drop the users table");
            Output("drop grammar (dg) - Drop the grammar table");
            Output("gen users (gu) - Generate sample users (drop it first)");
            Output("gen grammar (gg) - Generate grammar for 'default' user");
            Output("exit - Quit");
        }

        private static void Output()
        {
            Console.WriteLine();
        }
        private static void Output(string aString)
        {
            Console.WriteLine(aString);
        }
        private static void Output(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }

        private static string Prompt()
        {
            Console.Write("> ");
            return Console.ReadLine();
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

        private static void CreateFakeUsers(TableUtilities tableAccess)
        {
            CloudTable table = tableAccess.GetTable(USERS);

            var fakeNames = new List<string> { "adam", "beatrice", "charlie", "dave", "emily", DEFAULT_USER };

            foreach (var name in fakeNames)
            {
                string password = name + "password";
                if (name == DEFAULT_USER)
                {
                    password = "ALongAndSecureSecretIsBetterThanAnOreo" + DateTime.Now.ToString("HHmmssff");
                    Output("Default user password: '{0}' (save it now)", password);
                }

                var newUser = new UserEntity(name, password);
                var insert = TableOperation.Insert(newUser);
                table.ExecuteAsync(insert).Wait();
            }
        }

        private static Status<string> GenerateGrammar(TableUtilities tableAccess)
        {
            CloudTable table = tableAccess.GetTable(GRAMMAR);

            Output("Getting file...");
            string defaultGrammarContent = "";
            try
            {
                defaultGrammarContent = File.ReadAllText("default-grammar.txt");
                if (string.IsNullOrWhiteSpace(defaultGrammarContent))
                {
                    return new Status<string>(false, "default-grammar.txt is empty");
                }
            }
            catch (Exception ex)
            {
                return new Status<string>(false, "default-grammar.txt not found - " + ex.Message);
            }

            Output("Parse using Paprika and write to cache...");
            var parseAndCacheAction = new ParseAndCache();
            var result = parseAndCacheAction.Run(defaultGrammarContent, DEFAULT_USER, _storageProvider);

            if (!result.Success)
            {
                return new Status<string>(false, result.Attachment.Message);
            }

            Output("Write to blob...");
            try
            {
                var grammarBlob = new GrammarBlob(_storageProvider);
                grammarBlob.WriteGrammar(DEFAULT_USER, defaultGrammarContent);
            }
            catch (Exception ex)
            {
                return new Status<string>(false, "Failed to write grammar to blob: " + ex.Message);
            }

            return new Status<string>(true);
        }

        private static void SelectAndPrint(TableUtilities tableAccess, string tableName)
        {
            Output("Loading {0} table for display:...", tableName);
            CloudTable table = tableAccess.GetTable(tableName);

            var token = new TableContinuationToken();
            var lookup = new TableQuery<UserEntity>();
            var resultsAsync = table.ExecuteQuerySegmentedAsync(lookup, token);

            var result = resultsAsync.Result;
            foreach (var res in result)
            {
                Output("Key: {0}; Row: {1}", res.PartitionKey, res.RowKey);
            }
        }
    }
}
