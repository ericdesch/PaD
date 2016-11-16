var logErrorUrl = '/log/error';

function logError(message) {

    // send error message
    jQuery.ajax({
        type: 'POST',
        url: logErrorUrl,
        data: { message: message },
        success: function (data) {
        },
        error: function () {
            alert("Fatal Error");
        }

    });
}

function logError(ex, stack) {

    if (ex == null) return;
    if (logErrorUrl == null) {
        alert('logErrorUrl must be defined.');
        return;
    }

    var url = ex.fileName != null ? ex.fileName : document.location;
    if (stack == null && ex.stack != null) stack = ex.stack;

    // format output
    var out = ex.message != null ? ex.name + ": " + ex.message : ex;
    out += ": at document path '" + url + "'.";
    if (stack != null) out += "\n  at " + stack.join("\n  at ");

    // send error message
    jQuery.ajax({
        type: 'POST',
        url: logErrorUrl,
        data: { message: out }
    });
}

AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};

Function.prototype.trace = function () {
    var trace = [];
    var current = this;
    while (current) {
        trace.push(current.signature());
        current = current.caller;
    }
    return trace;
}

Function.prototype.signature = function () {
    var signature = {
        name: this.getName(),
        params: [],
        toString: function () {
            var params = this.params.length > 0 ?
                "'" + this.params.join("', '") + "'" : "";
            return this.name + "(" + params + ")"
        }
    };
    if (this.arguments) {
        for (var x = 0; x < this.arguments.length; x++)
            signature.params.push(this.arguments[x]);
    }
    return signature;
}

Function.prototype.getName = function () {
    if (this.name)
        return this.name;
    var definition = this.toString().split("\n")[0];
    var exp = /^function ([^\s(]+).+/;
    if (exp.test(definition))
        return definition.split("\n")[0].replace(exp, "$1") || "anonymous";
    return "anonymous";
}

var getUsernameFromUrl = function () {

    var username = '';
    var path = window.location.pathname;

    // trim leading '/'
    if (path.indexOf('/') == 0) {
        path = path.substr(1, path.length - 1);
    }

    if (path.indexOf('/') != -1) {
        // username/year/month so get first element after splitting
        username = path.split('/')[0];
    }
    else {
        // username so just return path
        username = path;
    }

    return username;
};

var getDateFromUrl = function () {

    var year, month, day = 1;
    var path = window.location.pathname;

    // trim leading '/'
    if (path.indexOf('/') == 0) {
        path = path.substr(1, path.length - 1);
    }

    if (path.indexOf('/') != -1) {
        // username/year/month/day
        parts = path.split('/');
        year = parts[1];
        month = parts[2];
        // day is optional, so set to 1 if not in url
        day = parts.length == 4 ? parts[3] : 1;
    }
    else {
        // user only, so use currDate as set in containing page
        return currDate;
    }

    var date = moment(year + '-' + month + '-' + day, 'YYYY-M-D');

    return date;
};

// Changes the contents of a div to the passed html.
// Fires htmlChanging and htmlChanged so other scripts can respond to the changes.
var changeHtml = function (selector, html) {

    var elem = $(selector);

    jQuery.event.trigger('htmlChanging', { elements: elem, content: { current: elem.html(), pending: html } });
    elem.html(html);
    jQuery.event.trigger('htmlChanged', { elements: elem, content: html });
};

var getUrl = function (format, username, year, month, day) {

    var url = format.format(username, year, month + 1, day);

    return url;
};

// Implement string.format like other languages.
if (!String.prototype.format) {
    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
              ? args[number]
              : match
            ;
        });
    };
}