function Paprika(urlRoot) {

    this.baseUrl = urlRoot;

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
        key: '',
    }
    this.newAnonEndpoint = {
        url: this.baseUrl + '/api/Anon/New/',
        key: '',
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
            var cssClass = success ? "bg-green" : "bg-red"
            return "<li><span class='pa1 br2 " + cssClass + "'>" + status + "</span> <code>" + response + "</code></li>"
        }

        $('.js-results').append(newLine(true, status, response));
    }

    //I would do anything for scope
    me = this;

    this.getOptions = function (endpoint, successCallback, errorCallback, doReport = true) {

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
                if (doReport) {
                    me.report(true, xhr.statusText, xhr.responseText);
                }
                if (typeof successCallback === 'function') {
                    successCallback(response);
                }
            },
            error: function onError(xhr, statusWord, response) {
                if (doReport) {
                    me.report(false, xhr.statusText, xhr.responseText);
                }
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

    this.query = function () {
        var query = $('.js-query').val();
        query = encodeURI(query);

        var options = this.getOptions(this.resolveEndpoint);
        options.url = this.resolveEndpoint.url + query;
        options.method = 'GET';

        this.loading(true);
        $.ajax(options);
    }

    this.getGrammar = function () {

        var populateGrammarOnSuccess = function (response) {
            $('.js-grammar-text').val(response);
        }

        var retryOnError = function (response) {
            console.log("Set timeout to retry...");
            setTimeout(me.getGrammar, 2000);
        }

        var options = this.getOptions(this.getGrammarEndpoint, populateGrammarOnSuccess, retryOnError, false);
        options.method = 'GET';

        this.loading(true);
        $.ajax(options);
    }

    this.createAnon = function () {
        console.log("Create Anon");

        me.report(true, "Just a sec...", "I'm generating an anonymous test user for you to use");

        var useAnonData = function (response) {
            console.log("Use anon data", response);
            me.setUsername(response.Name);
            me.setPassword(response.Password);
            console.log("Get grammar...");
            me.getGrammar();
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
