$(function () {

    jQuery.validator.unobtrusive.adapters.add('filesize', ['maxkilobytes'], function (options) {
        // Set up test parameters
        var params = {
            maxkilobytes: options.params["maxkilobytes"]
        };

        // Match parameters to the method to execute
        options.rules['filesize'] = params;
        if (options.message) {
            // If there is a message, set it for the rule
            options.messages['filesize'] = options.message;
        }
    });

    jQuery.validator.addMethod("filesize", function (value, element, param) {

        if (value === "") {
            // no file supplied
            return true;
        }

        var maxKilobytes = parseInt(param.maxkilobytes);

        // use HTML5 File API to check selected file size
        // https://developer.mozilla.org/en-US/docs/Using_files_from_web_applications
        // http://caniuse.com/#feat=fileapi
        if (element.files != undefined && element.files[0] != undefined && element.files[0].size != undefined) {
            var filesize = parseInt(element.files[0].size);
            return (filesize / 1024) <= maxKilobytes;
        }

        // if the browser doesn't support the HTML5 file API, just return true
        // since returning false would prevent submitting the form 
        return true;
    });
}(jQuery));