# Release Procedure

In lieu of automated DevOps, we can at least have a written-down release plan, and here it is:-

 0. Open the Solution in Visual Studio
 0. Open `PaprikaFunctionsApp.Web/res/paprikaApp.js`
 0. Make sure the `rootUrl` is `""` (sample below)
 0. Right-click `PaprikaFunctionsApp.Web` and **Publish Web App**
 0. Right-click `PaprikaFunctionsApp` and **Publish** it
 
paprikaApp.js baseUrl:

    $scope.baseUrl = ""; //Live
    //$scope.baseUrl = "http://localhost:7071/"; //Dev
	
## About connection strings

When the Functions app is run locally, it uses the connection strings from `local.settings.json`, but when it is published to Azure, it uses the connection string configured in the Azure portal (local settings file is not uploaded)

## Migrations

**The following process is a first-time setup which will destroy any existing data**

To prepare a new live environment:

 0. Open PaprikaFunctionsApp.Migrations/appSettings.json
 0. Make sure that the `PrimaryStorage` ConnectionString points to live
 0. Start the Migrations program
 
Run the following commands:

	> dg
	> du
	
Restart the program. To verify the tables were dropped:

	> sg
	Loading grammar table for display:...
	Finished operation
	> su
	Loading users table for display:...
	Finished operation
	>
	
Now regenerate:

	> gu
	> gg
 
 
