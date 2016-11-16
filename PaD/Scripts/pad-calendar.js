//****************************
//
//Functions common to pad-calendar-month.js and pad-calendar-day.js
//
//****************************

var setDate = function (date) {

    if (date.isBefore(moment(projectFirstDate))) {
        date = projectFirstDate;
    }
    else if (date.isAfter(moment(projectLastDate))) {
        date = projectLastDate;
    }

    $('#datepicker').data("DateTimePicker").date(date);
};

var getDate = function () {

    return $('#datepicker').data("DateTimePicker").date();
};

var setNavButtonsEnabled = function (date) {

    if (enablePrevButtons(date, projectFirstDate)) {
        $("#buttonFirst").removeClass('disabled');
        $("#buttonPrevious").removeClass('disabled');
    }
    else {
        $("#buttonFirst").addClass('disabled');
        $("#buttonPrevious").addClass('disabled');
    }
    if (enableNextButtons(date, projectLastDate)) {
        $("#buttonNext").removeClass('disabled');
        $("#buttonLast").removeClass('disabled');
    }
    else {
        $("#buttonNext").addClass('disabled');
        $("#buttonLast").addClass('disabled');
    }
};

$('#datepicker').on('dp.change', function (e) {

    e.preventDefault();

    // allowUpdate is a flag set to false when page is first created. Don't need to
    // call updateView this first time.
    if (allowUpdate) {

        // Update view with the newly-selected date
        updateView(e.date);
    }

    setNavButtonsEnabled(e.date);
});

// Called when user clicks First button.
$("#buttonFirst").on('click', function (e) {

    e.preventDefault();
    this.blur();

    // Do nothing if the clicked button is disabled.
    if ($("#buttonFirst").hasClass("disabled")) {
        return;
    }

    setDate(projectFirstDate);
});

// Called when user clicks Previous button.
$("#buttonPrevious").on('click', function (e) {

    e.preventDefault();
    this.blur();

    // Do nothing if the clicked button is disabled.
    if ($("#buttonPrevious").hasClass("disabled")) {
        return;
    }

    var date = getDate();
    var prevDate = calculatePrevDate(date, projectFirstDate);

    setDate(prevDate);
});

// Called when user clicks Next button.
$("#buttonNext").on('click', function (e) {

    e.preventDefault();
    this.blur();

    // Do nothing if the clicked button is disabled.
    if ($("#buttonNext").hasClass("disabled")) {
        return;
    }

    var date = getDate();
    var nextDate = calculateNextDate(date, projectLastDate);

    setDate(nextDate);
});

// Called when user clicks Last button.
$("#buttonLast").on('click', function (e) {

    e.preventDefault();
    this.blur();

    // Do nothing if the clicked button is disabled.
    if ($("#buttonLast").hasClass("disabled")) {
        return;
    }

    setDate(projectLastDate);
});

// Okay to push state unless the user uses the browser back/forward.
// In that case, we don;t want to re-push the state. Use a flag to
// keep track of when it is okay to push state.
var okayToPushState = true;

// Called when the user clicks the browser's back or forward buttons
$(window).on('popstate', function (e) {

    var date = getDateFromUrl();

    // Since we were called when the user clicked browser forward/back,
    // set to false so we don't re-push this state.
    okayToPushState = false;
    setDate(date);
});

var updateView = function (date) {

    var year = date.year();
    var month = date.month();
    var day = date.date();
    // urlFormat = '/{0}/{1}/{2}' OR '/{0}/{1}/{2}/{3}'
    // url becomes ie '/eric/2016/12' OR '/eric/2016/12/31'
    var url = getUrl(urlFormat, username, year, month, day);

    // Detect if browser supports history.pushState()
    if (history.pushState) {

        var fadeTime = 'fast'; //200

        // Use ajax to update the page
        $.ajax({
            type: 'GET',
            cache: false,
            url: url,
            beforeSend: function () {
                // Fade out old content
                $('#' + dataDivId).fadeTo(fadeTime, 0);
            },
            //complete: function () {
            //    //$('#' + dataDivId).fadeTo(fadeTime, 1);
            //},
            error: function (xhr, textStatus, errorThrown) {
                // send error message
                var err = 'pad-caledar.js updateView() failed';
                logError(textStatus + ': ' + errorThrown + ': ' + err);
            }
        })
        .done(function (data) {

            changeHtml('#' + dataDivId, data);
            setNavButtonsEnabled(date);

            // Use pushstate so browser does what's expected when user steps through history,
            // can bookmark, etc.
            // Only push state if we didn't get here from a browser back/forward so we don't
            // add this state again.
            if (okayToPushState) {
                history.pushState(null, '', url);
            }
            // Set to true for future calls. The popstate callback will set to false as needed.
            okayToPushState = true;

            // Set the title after pushing the state, otherwise the browser history will show the wrong
            // title.
            if (urlFormat == '/{0}/{1}/{2}') {
                // month view
                document.title = "PaD: " + date.format("MMMM YYYY");
            }
            else {
                // day view
                document.title = "PaD: " + date.format("M/D/YYYY");
            }

            // Fade content back in
            $('#' + dataDivId).fadeTo(fadeTime, 1);

            // Because this is an AJAX call, send this url to google analytics
            ga('set', 'page', url);
            ga('send', 'pageview');

        });

    }
    else {
        // No history.pushState. Refresh the whole page.
        window.location.href = url;
    }
};