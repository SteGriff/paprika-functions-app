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

    //I would do anything for scope
    me = this;
    
    this.getOptions = function (endpoint, callback) {

        newLine = function (success, status, response) {
            var cssClass = success ? "success" : "error"
            return "<li><span class='label " + cssClass + "'>" + status + "</span><code>" + response + "</code></li>"
        }

        return {
            beforeSend: function (request) {
                request.setRequestHeader('username', $('.js-username').val());
                request.setRequestHeader('password', $('.js-password').val());
                request.setRequestHeader('x-functions-key', endpoint.key);
            },
            url: endpoint.url,
            cache: false,
            contentType: false,
            processData: false,
            method: 'POST',
            success: function onSuccess(response, statusWord, xhr) {
                $('.js-results').append(newLine(true, xhr.statusText, xhr.responseText));
                if (typeof callback === 'function') {
                    callback(response);
                }
            },
            error: function onError(xhr, statusWord, response) {
                $('.js-results').append(newLine(false, xhr.statusText, xhr.responseText));
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

        var populateGrammar = function (response) {
            $('.js-grammar-text').val(response);
        }

        var options = this.getOptions(this.getGrammarEndpoint, populateGrammar);
        options.method = 'GET';

        this.loading(true);
        $.ajax(options);
    }

    this.loading = function (isOn) {
        if (isOn) {
            $('.js-loading').show();
        }
        else {
            $('.js-loading').hide();
        }
    }
}
