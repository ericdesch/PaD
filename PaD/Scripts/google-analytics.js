﻿// Google Analytics
$(function () {
    var parsePathname = function () {

        var pathname = location.pathname;

        // Convert /home/index to /
        if (pathname == '/home/index')
            return '/';

        // Convert /username to /username/year/month
        if (typeof currDate !== 'undefined') {
            if (pathname == '/' + getUsernameFromUrl()) {
                var date = getDateFromUrl();
                return pathname + '/' + date.year() + '/' + (date.month() + 1);
            }
        }

        return pathname;
    };

    (function (i, s, o, g, r, a, m) {
        i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
            (i[r].q = i[r].q || []).push(arguments)
        }, i[r].l = 1 * new Date(); a = s.createElement(o),
        m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
    })(window, document, 'script', 'https://www.google-analytics.com/analytics.js', 'ga');

    ga('create', 'UA-12267344-1', 'auto');
    ga('send', 'pageview', location.pathname);
});
// End Google Analytics
