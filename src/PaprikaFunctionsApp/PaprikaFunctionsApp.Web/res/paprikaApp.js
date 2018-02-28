var paprikaApp = angular.module('paprikaApp',[]);

paprikaApp.controller('MainController', ['$scope', '$http', function($scope, $http) {

    $scope.baseUrl = "";
    $scope.baseUrl = "http://localhost:7071/";

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
        key: 'RMMCAxuECnryL29QwzIiqB8JZ9LnrQF8JWb5whPpadcdCgayzveYIQ==',
    }
    $scope.newAnonEndpoint = {
        url: $scope.baseUrl + '/api/Anon/New/',
        key: 'Z3KhN4CAzjiilXrgohxb7s24cRkgWslIYZmeS9oqliafaSlOgMQEpw==',
    }
    $scope.upgradeAnonEndpoint = {
        url: $scope.baseUrl + '/api/Anon/Upgrade/',
        key: '',
    }
    $scope.newUserEndpoint = {
        url: $scope.baseUrl + '/api/User/New/',
        key: '',
    }

    //Set by DOM:
    //$scope.username;
    //$scope.password;
    //$scope.isAnon;

    $scope.showModal = false;
    $scope.openDialog = function () { $scope.showModal = true; }
    $scope.closeDialog = function () { $scope.showModal = false; }

    $scope.reports = [];
    $scope.report = function (success, status, response) {
        $scope.reports.push({ "success": success, "status": status, "response": response });
    }
    $scope.answer = function (text)
    {
        $scope.reports.push({ "answer": text });
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

    $scope.uploadFile = function () {
        var data = new FormData();
        $.each($('.js-file')[0].files, function (i, file) {
            data.append('file-' + i, file);
        });

        var options = $scope.getOptions($scope.uploadFileEndpoint);
        options.data = data;
        options.method = 'POST';

        $scope.webRequest(options);
    }

    $scope.grammarText;
    $scope.uploadText = function () {
        var options = $scope.getOptions($scope.uploadTextEndpoint);
        options.data = $scope.grammarText;

        $scope.webRequest(options);
    }

    $scope.doQuery = function (event) {

        var showQueryResultOnSuccess = function (response) {
            $scope.answer(response.data);
        }

        var options = $scope.getOptions($scope.resolveEndpoint);
        options.data = { "query": $scope.query };
        //options.contentType = "application/json";

        $scope.webRequest(options, showQueryResultOnSuccess);

        event.preventDefault();
        return false;
    }

    $scope.getGrammar = function () {

        var populateGrammarOnSuccess = function (response) {
            $scope.grammarText = response.data;
        }

        var complain = function ()
        {
            $scope.report(false, "Error", "Couldn't get your grammar, try again")
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

        var complain = function ()
        {
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
        options.data = JSON.stringify({ "newUsername": $scope.newUsername, "newPassword": $scope.newPassword });
        //options.contentType = "application/json";

        $scope.webRequest(options, usePermanentData);

        event.preventDefault();
        return false;
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