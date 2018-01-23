function Paprika(urlRoot) {

    this.baseUrl = urlRoot || "";

    this.uploadFileEndpoint = {
        url: this.baseUrl + '/api/Grammar/UploadFile/',
        key: 'q7HVSreWULwdAbSFbuG517bakta77cyDG5ZGv5ZPyWdPTLVze3fHbA=='
    };
    this.resolveEndpoint = {
        url: this.baseUrl + '/api/Grammar/Resolve/',
        key: '84DlNVzNcAQf2s6P6PddI8lEcfRhPWZFhy3UPOz/4zWWgbnB3mqzMA=='
    };
    this.uploadTextEndpoint = {
        url: this.baseUrl + '/api/Grammar/UploadText/',
        key: 'aUJaV591ZybgjVBvX7X1a/0SUJPwdE6NpUKnjRAuzr4AS12mf8vUow=='
    };
    this.getGrammarEndpoint = {
        url: this.baseUrl + '/api/Grammar/GetGrammar/',
        key: 'RMMCAxuECnryL29QwzIiqB8JZ9LnrQF8JWb5whPpadcdCgayzveYIQ==',
    }
    this.newAnonEndpoint = {
        url: this.baseUrl + '/api/Anon/New/',
        key: 'Z3KhN4CAzjiilXrgohxb7s24cRkgWslIYZmeS9oqliafaSlOgMQEpw==',
    }
    this.newUserEndpoint = {
        url: this.baseUrl + '/api/User/New/',
        key: '',
    }

    this.getUsername = function () {
        return $('.js-username').val();
    }
    this.getPassword = function () {
        return $('.js-password').val();
    }

    this.setUsername = function (value) {
        $('.js-username').val(value);
    }
    this.setPassword = function (value) {
        $('.js-password').val(value);
    }

    this.report = function (success, status, response) {
        newLine = function (success, status, response) {
            var cssClass = success ? "bg-green" : "bg-red";
            return "<li><span class='pa1 br1 " + cssClass + "'>" + status + "</span> <code>" + response + "</code></li>"
        }

        $('.js-results').prepend(newLine(true, status, response));
    }

    //I would do anything for scope
    me = this;

    this.getOptions = function (endpoint, successCallback, errorCallback) {

        return {
            beforeSend: function (request) {
                request.setRequestHeader('username', me.getUsername());
                request.setRequestHeader('password', me.getPassword());
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
                me.report(false, xhr.statusText, xhr.responseText);
                if (typeof errorCallback === 'function') {
                    errorCallback(response);
                }
            },
            complete: function onResponse(data) {
                me.loading(false);
            }
        }
    }

    this.uploadFile = function () {
        var data = new FormData();
        $.each($('.js-file')[0].files, function (i, file) {
            data.append('file-' + i, file);
        });

        var options = this.getOptions(this.uploadFileEndpoint);
        options.data = data;
        options.method = 'POST';

        this.loading(true);
        $.ajax(options);
    }

    this.uploadText = function () {
        var options = this.getOptions(this.uploadTextEndpoint);
        var text = $('.js-grammar-text').val();
        options.data = text;
        options.method = 'POST';

        this.loading(true);
        $.ajax(options);
    }

    this.urlEncode = function (request)
    {
        var result = encodeURIComponent(request);
        result = result.replace("#", "%23");
        return result;
    }

    this.query = function (event) {

        var showQueryResultOnSuccess = function (response) {
            newLine = function (response) {
                return "<li class='b'>" + response + "</li>"
            }

            $('.js-results').prepend(newLine(response));
        }

        var query = $('.js-query').val();

        var options = this.getOptions(this.resolveEndpoint, showQueryResultOnSuccess);
        options.url = this.resolveEndpoint.url
        options.data = JSON.stringify({ "query": query });
        options.contentType = "application/json";
        options.method = 'POST';

        this.loading(true);
        $.ajax(options);

        event.preventDefault();
        return false;
    }

    this.getGrammar = function () {

        var populateGrammarOnSuccess = function (response) {
            $('.js-grammar-text').val(response);
        }
        
        var options = this.getOptions(this.getGrammarEndpoint, populateGrammarOnSuccess);
        options.method = 'GET';

        this.loading(true);
        $.ajax(options);
    }

    this.createAnon = function () {
        console.log("Create Anon");

        me.report(true, "Just a sec...", "I'm generating an anonymous test user for you to use");

        var useAnonData = function (response) {
            //console.log("Use anon data", response);
            me.setUsername(response.Name);
            me.setPassword(response.Password);
            //console.log("Get grammar...");
            me.getGrammar();
            me.report(true, "Done", "Go ahead, you're now " + response.Name);
        }

        var options = this.getOptions(this.newAnonEndpoint, useAnonData);
        options.method = 'POST';

        this.loading(true);
        $.ajax(options);
    }

    this.loading = function (isOn, text) {
        var $loader = $('.js-loading');
        if (isOn) {
            if (text) {
                $loader.text(text);
            }
            else {
                $loader.text('Loading...');
            }
            $loader.show();
        }
        else {
            $loader.hide();
        }
    }

    this.setupInitialView = function () {
        var username = this.getUsername();
        if (username) {
            this.getGrammar();
        }
        else {
            this.createAnon();
        }
    }

    //We're in a function body
    //Do setup
    this.setupInitialView();
}
