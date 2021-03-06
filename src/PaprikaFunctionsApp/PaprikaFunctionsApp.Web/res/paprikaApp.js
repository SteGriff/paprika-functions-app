﻿var paprikaApp = angular
    .module('paprikaApp', ['LocalStorageModule'])
    .config(function (localStorageServiceProvider) {
        localStorageServiceProvider.setPrefix('paprikaMeUk');
    });

paprikaApp.controller('MainController', ['$scope', '$http', 'localStorageService', function ($scope, $http, localStorageService) {

    $scope.baseUrl = "";

    const USERNAME = 'username';
    const PASSWORD = 'password';
    $scope.username = localStorageService.get(USERNAME);
    $scope.password = localStorageService.get(PASSWORD);

    $scope.$watch(USERNAME, function (value) {
        $scope.identifier = $scope.getIdentifier();
        localStorageService.set(USERNAME, value);
    });
    $scope.$watch(PASSWORD, function (value) {
        $scope.identifier = $scope.getIdentifier();
        localStorageService.set(PASSWORD, value);
    });
    
    $scope.$watch(
        function () { return localStorageService.get(USERNAME); },
        function (value) { $scope.username = value; }
    );
    $scope.$watch(
        function () { return localStorageService.get(PASSWORD); },
        function (value) { $scope.password = value; }
    );

    $scope.uploadFileEndpoint = {
        url: $scope.baseUrl + '/api/Grammar/UploadFile/',
        key: 'q7HVSreWULwdAbSFbuG517bakta77cyDG5ZGv5ZPyWdPTLVze3fHbA=='
    };
    $scope.resolveEndpoint = {
        url: $scope.baseUrl + '/api/Grammar/Resolve/',
        key: '84DlNVzNcAQf2s6P6PddI8lEcfRhPWZFhy3UPOz/4zWWgbnB3mqzMA=='
    };
    $scope.uploadTextEndpoint = {
        url: $scope.baseUrl + '/api/Grammar/UploadText/',
        key: 'aUJaV591ZybgjVBvX7X1a/0SUJPwdE6NpUKnjRAuzr4AS12mf8vUow=='
    };
    $scope.getGrammarEndpoint = {
        url: $scope.baseUrl + '/api/Grammar/GetGrammar/',
        key: 'RMMCAxuECnryL29QwzIiqB8JZ9LnrQF8JWb5whPpadcdCgayzveYIQ=='
    };
    $scope.newAnonEndpoint = {
        url: $scope.baseUrl + '/api/Anon/New/',
        key: 'Z3KhN4CAzjiilXrgohxb7s24cRkgWslIYZmeS9oqliafaSlOgMQEpw=='
    };
    $scope.upgradeAnonEndpoint = {
        url: $scope.baseUrl + '/api/Anon/Upgrade/',
        key: 'hCRr9w3QhRbUqbQguaXzpZl1buJkMm2srnTmuaEbf1C4RsxpCSUHQA=='
    };
    $scope.twitterGetEndpoint = {
        url: $scope.baseUrl + '/api/Twitter/Get',
        key: '9GZSWLecqH1Fp7Tpsu9iLWAWqZDZpJ/qxLVblovK8hwvtkmSeNHvmw=='
    };
    $scope.twitterSetEndpoint = {
        url: $scope.baseUrl + '/api/Twitter/Set',
        key: 'oEa3pp2frOMvPlzxaCQKNsUdGfgEfIoYr2DZAyS6m2jdPnBxbHwx9Q=='
    };
    $scope.twitterDisconnectEndpoint = {
        url: $scope.baseUrl + '/api/Twitter/Disconnect',
        key: 'owTY277yezU4bkvaiacdPT74iXxqbWfafPYKip9ssaXfMVe7rapgCA=='
    };

    //Set by DOM:
    //$scope.isAnon;

    $scope.tutorials = [
        {
            "title": "Basic lookup of simple tags",
            "produces": "what a neat red dog",
            "input": "what a [cool] [colour] [animal]"
        },
        {
            "title": "Repeated tags get the same value every time:",
            "produces": "wolf!! wolf in the library!!",
            "input": "[animal]!! [animal] in [place]!!"
        },
        {
            "title": "You can re-roll for a different word using hashtags at the end of a tag but they're not guaranteed to be different results",
            "produces": "yellow bear, grey lion",
            "input": "[colour#1] [animal#1], [colour#2] [animal#2]"
        },
        {
            "title": "The hashtags can be anything you want to use to distinguish the tags",
            "produces": "from the park to school",
            "input": "from [place#from] to [place#to]"
        },
        {
            "title": "You can nest tags as long as the inside-bit has already been resolved",
            "produces": "my lion controls the weather",
            "input": "my [animal] [does] [[does] thing]",
            "notes": "After round 1: 'my lion controls [controls thing]' then finally: 'my lion controls the weather'"
        },
        {
            "title": "To fix that, you can do an 'early' lookup and make it invisible using the ! command:",
            "produces": "pig is an animal",
            "input": "[!thing][[thing]] is [a] [thing]",
            "notes": "Note that putting [a] or [an] in brackets is magic; it will be replaced with the correct article (a/an) when resolved"
        },
        {
            "title": "Another nested example",
            "produces": "Choose an animal. Choose mouse",
            "input": "Choose [a] [thing]. Choose [[thing]]"
        },
        {
            "title": "If you put a slash in a tag, it won't look it up, but will pick from the options",
            "produces": "this day can't get any better",
            "input": "[my/this] day [could/can't] get any [worse/better]"
        },
        {
            "title": "You can make a word fully optional using the slash",
            "produces": "how about them apples, eh?",
            "input": "[/so, ]how about [those/them] [apples/oranges][, huh/, eh/]?"
        }
    ];

    $scope.tutorialStep = 0;
    $scope.tutorialOpen = false;
    $scope.progressIcon = function (index)
    {
        return index <= $scope.tutorialStep ? "✅" : "◼";
    }
    $scope.nextStep = function () { if ($scope.tutorialStep < $scope.tutorials.length - 1) { $scope.tutorialStep++; } }
    $scope.prevStep = function () { if ($scope.tutorialStep > 0) { $scope.tutorialStep--; } }
    $scope.useExample = function () {
        $scope.query = $scope.tutorials[$scope.tutorialStep].input;
        $scope.doQuery();
    };
    $scope.startTutorial = function () {
        $scope.tutorialStep = 0;
        $scope.tutorialOpen = true;
    };

    $scope.modal = '';
    $scope.openDialog = function () { $scope.modal = 'upgrade'; }
    $scope.openTwitterDialog = function () { $scope.modal = 'twitter'; $scope.getTwitter(); }
    $scope.closeDialog = function () { $scope.modal = ''; }

    $scope.reports = [];
    $scope.report = function (success, status, response) {
        $scope.reports.push({ "success": success, "status": status, "response": response });
    }
    $scope.answer = function (text) {
        $scope.reports.push({ "answer": text });
    }
    $scope.clearResults = function () {
        $scope.reports = [];
    }

    $scope.getOptions = function (endpoint, successCallback, errorCallback) {
        return {
            headers: {
                'username': $scope.username,
                'password': $scope.password,
                'x-functions-key': endpoint.key
            },
            url: endpoint.url,
            cache: false,
            method: 'POST'
        }
    }

    $scope.webRequest = function (options, onSuccess, onError) {
        $scope.loading(true);
        $http(options)
            .then(onSuccess, onError)
            .finally(function () { $scope.loading(false) });
    }

    $scope.grammarText;
    $scope.uploadText = function () {

        var saved = function () { $scope.report(true, "Saved", "Grammar saved"); }
        var failed = function () { $scope.report(true, "Error", "Failed to save grammar - try again or back it up to a local text file!"); }

        var options = $scope.getOptions($scope.uploadTextEndpoint);
        options.data = $scope.grammarText;

        $scope.webRequest(options, saved, failed);
    }

    $scope.doQuery = function () {

        var showQueryResultOnSuccess = function (response) {
            $scope.answer(response.data);
        }

        var complain = function (response) {
            $scope.report(false, "Paprika Error", response.data);
        }

        var options = $scope.getOptions($scope.resolveEndpoint);
        options.data = { "query": $scope.query };

        $scope.webRequest(options, showQueryResultOnSuccess, complain);

        return false;
    }

    $scope.getGrammar = function () {

        var populateGrammarOnSuccess = function (response) {
            $scope.grammarText = response.data;
            //Hack to check if user is Anon
            $scope.isAnon = $scope.username.lastIndexOf("User", 0) === 0;
            $scope.report(true, "Loaded", "Got grammar for " + $scope.username);
        }

        var complain = function () {
            $scope.report(false, "Error", "Couldn't get your grammar, try again");
        }

        var options = $scope.getOptions($scope.getGrammarEndpoint);
        options.method = 'GET';

        $scope.webRequest(options, populateGrammarOnSuccess, complain);
    }

    $scope.createAnon = function () {
        console.log("Create Anon");

        $scope.report(true, "Just a sec...", "I'm generating an anonymous test user for you to use");

        var useAnonData = function (response) {
            console.log(response);
            $scope.username = response.data.Name;
            $scope.password = response.data.Password;
            $scope.isAnon = true;
            $scope.getGrammar();
            $scope.report(true, "Done", "Go ahead, you're now " + response.data.Name);
        }

        var complain = function () {
            $scope.report(false, "Oops", "I failed to generate an anon user for you... try refreshing the page");
        }

        var options = $scope.getOptions($scope.newAnonEndpoint);

        $scope.webRequest(options, useAnonData, complain);
    }

    $scope.upgradeAnon = function (event) {
        console.log("Upgrade Anon");

        $scope.report(true, "Transformulating...", "I'm saving your new user account");

        var usePermanentData = function (response) {
            $scope.username = $scope.newUsername;
            $scope.password = $scope.newPassword;
            $scope.isAnon = false;
            $scope.getGrammar();
            $scope.report(true, "Done", "Go ahead, you're now " + $scope.newUsername);
            $scope.closeDialog();
        }

        var options = $scope.getOptions($scope.upgradeAnonEndpoint);
        options.data = { "newUsername": $scope.newUsername, "newPassword": $scope.newPassword };

        $scope.webRequest(options, usePermanentData);

        event.preventDefault();
        return false;
    }

    $scope.getIdentifier = function () {
        function utoa(str) {
            return window.btoa(unescape(encodeURIComponent(str)));
        }

        var separator = "----IDENTIFIER----";
        var concat = $scope.username + separator + $scope.password;
        var identifier = utoa(concat);
        return identifier;
    }

    $scope.twitter = null;
    $scope.getTwitter = function ()
    {
        console.log("Get Twitter Data...");
        
        var setTwitterData = function (response) {
            console.log(response);
            $scope.twitter = response.data;
            $scope.report(true, "Twitter", "You're linked up to @" + $scope.twitter.TwitterUsername);
        }

        var complain = function () {
            $scope.twitterError = "Uh-oh. Failed to fetch your Twitter link details...";
        }

        var options = $scope.getOptions($scope.twitterGetEndpoint);

        $scope.webRequest(options, setTwitterData, complain);
    }

    $scope.saveSchedule = function (event) {
        console.log("Save Schedule");

        $scope.report(true, "Saving...", "I'm saving your Tweet schedule...");

        var scheduleSaved = function (response) {
            console.log("scheduleSaved", response);
            $scope.report(true, "Saved!", "Your tweet schedule was saved");
            $scope.twitterError = "";
            $scope.closeDialog();
        }

        var complain = function (response) {
            console.log("complain", response);
            $scope.report(false, "Oops", "Failed to save the tweet schedule...");
            $scope.twitterError = "Nope, that failed.";
        }

        var options = $scope.getOptions($scope.twitterSetEndpoint);
        options.data = {
            "ScheduleEnable": $scope.twitter.ScheduleEnable,
            "ScheduleQuery": $scope.twitter.ScheduleQuery,
            "ScheduleHourInterval": $scope.twitter.ScheduleHourInterval
        };

        console.log(options.data);

        $scope.webRequest(options, scheduleSaved, complain);

        event.preventDefault();
        return false;
    }

    $scope.disconnectTwitter = function () {
        console.log("Disconnect Twitter...");

        var setTwitterData = function (response) {
            $scope.twitter = null;
            $scope.report(true, "Twitter", "Account disconnected");
        }

        var complain = function () {
            $scope.twitterError = "Hmmm... Failed to disconnect from your twitter account (you could remove the PaprikaLanguage app from your Twitter settings instead).";
        }

        var options = $scope.getOptions($scope.twitterDisconnectEndpoint);
        $scope.webRequest(options, setTwitterData, complain);
    }

    $scope.isLoading = false;
    $scope.loading = function (isOn, text) {
        $scope.isLoading = isOn;
        $scope.loadingText = text || "Loading...";
    }

    $scope.setupInitialView = function () {
        if ($scope.username) {
            $scope.getGrammar();
        }
        else {
            $scope.createAnon();
        }
    }

    //We're in a function body
    //Do setup
    $scope.setupInitialView();
}]);