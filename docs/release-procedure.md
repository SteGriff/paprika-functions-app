# Release Procedure

In lieu of automated DevOps, we can at least have a written-down release plan, and here it is:-

 0. Open the Solution in Visual Studio
 0. Open `PaprikaFunctionsApp.Web/index.htm`
 0. Remove the rootUrl passed to the Paprika object at the bottom of the file
 0. Right-click `PaprikaFunctionsApp.Web` and **Publish** it
 0. Right-click `PaprikaFunctionsApp` and **Publish** it
 
## About connection strings

When the Functions app is run locally, it uses the connection strings from `local.settings.json`, but when it is published to Azure, it uses the connection string configured in the Azure portal (local settings file is not uploaded)