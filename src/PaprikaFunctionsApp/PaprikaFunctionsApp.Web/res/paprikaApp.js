var paprikaApp = angular.module('paprikaApp',[]);

paprikaApp.controller('MainController', ['$scope', '$http', function($scope, $http) {

/*
	$scope.message = "";
	$scope.newButtonText = "";
	$scope.buttons = ["Thanks", "OK", "Bye", "ðŸ˜€"];
	
	$scope.addChat = function(segment){
		$scope.message += " " + segment;
	}
	
	$scope.addButton = function(){
		
		if ($scope.newButtonText)
		{
			$scope.buttons.push($scope.newButtonText);
		}
	}
    */

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

    //$scope.username;
    //$scope.password;

    $scope.showModal = false;

    $scope.reports = [];
    $scope.report = function (success, status, response) {
        $scope.reports.push({ "success": success, "status": status, "response": response });
    }
    
    $scope.getOptions = function (endpoint, successCallback, errorCallback) {

        return {
            beforeSend: function (request) {
                request.setRequestHeader('username', $scope.username);
                request.setRequestHeader('password', $scope.password);
                request.setRequestHeader('x-functions-key', endpoint.key);
            },
            url: endpoint.url,
            cache: false,
            contentType: false,
            processData: false,
            method: 'POST',
            success: function onSuccess(response, statusWord, xhr) {
                if (typeof successCallback === 'function') {
                    successCallback(response);
                }
            },
            error: function onError(xhr, statusWord, response) {
                $scope.report(false, xhr.statusText, xhr.responseText);
                if (typeof errorCallback === 'function') {
                    errorCallback(response);
                }
            },
            complete: function onResponse(data) {
                $scope.loading(false);
            }
        }
    }

    $scope.uploadFile = function () {
        var data = new FormData();
        $.each($('.js-file')[0].files, function (i, file) {
            data.append('file-' + i, file);
        });

        var options = $scope.getOptions($scope.uploadFileEndpoint);
        options.data = data;
        options.method = 'POST';

        $scope.loading(true);
        $.ajax(options);
    }

    $scope.grammarText;
    $scope.uploadText = function () {
        var options = $scope.getOptions($scope.uploadTextEndpoint);
        options.data = $scope.grammarText;;
        options.method = 'POST';

        $scope.loading(true);
        $.ajax(options);
    }

    $scope.query = function (event) {

        var showQueryResultOnSuccess = function (response) {
            newLine = function (response) {
                return "<li class='b'>" + response + "</li>"
            }

            $('.js-results').prepend(newLine(response));
        }

        var query = $('.js-query').val();

        var options = $scope.getOptions($scope.resolveEndpoint, showQueryResultOnSuccess);
        options.url = $scope.resolveEndpoint.url
        options.data = JSON.stringify({ "query": query });
        options.contentType = "application/json";
        options.method = 'POST';

        $scope.loading(true);
        $.ajax(options);

        event.preventDefault();
        return false;
    }

    $scope.getGrammar = function () {

        var populateGrammarOnSuccess = function (response) {
            $('.js-grammar-text').val(response);
        }

        var options = $scope.getOptions($scope.getGrammarEndpoint, populateGrammarOnSuccess);
        options.method = 'GET';

        $scope.loading(true);
        $.ajax(options);
    }

    $scope.createAnon = function () {
        console.log("Create Anon");

        $scope.report(true, "Just a sec...", "I'm generating an anonymous test user for you to use");

        var useAnonData = function (response) {
            $scope.username = response.Name;
            $scope.password = response.Password;
            $scope.getGrammar();
            $scope.report(true, "Done", "Go ahead, you're now " + response.Name);
        }

        var options = $scope.getOptions($scope.newAnonEndpoint, useAnonData);
        options.method = 'POST';

        $scope.loading(true);
        $.ajax(options);
    }

    $scope.upgradeAnon = function (event) {
        console.log("Upgrade Anon");

        $scope.report(true, "Transformulating...", "I'm saving your new user account");

        var usePermanentData = function (response) {
            $scope.username = $scope.newUsername;
            $scope.password = $scope.newPassword;
            $scope.getGrammar();
            $scope.report(true, "Done", "Go ahead, you're now " + $scope.newUsername);
            page.closeDialog();
        }

        var options = $scope.getOptions($scope.upgradeAnonEndpoint, usePermanentData);
        options.data = JSON.stringify({ "newUsername": $scope.newUsername, "newPassword": $scope.newPassword });
        options.contentType = "application/json";
        options.method = 'POST';

        $scope.loading(true);
        $.ajax(options);

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