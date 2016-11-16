
// Function to calculate next date.
var calculatePrevDate = function (thisDate, firstDate) {

    // prevDate will be the last day of the previous month.
    prevDate = moment(thisDate).clone().subtract(1, 'days');
    if (prevDate.isBefore(moment(firstDate))) {
        prevDate = firstDate;
    }

    return prevDate;
};

// Function to calculate next date.
var calculateNextDate = function (thisDate, lastDate) {

    // nextDate will be the first of the next month.
    nextDate = moment(thisDate).clone().add(1, 'days');
    if (nextDate.isAfter(moment(lastDate))) {
        nextDate = lastDate;
    }

    return nextDate;
};

var enablePrevButtons = function (thisDate, firstDate) {

    // prevDate will be the last date of the previous month.
    prevDate = moment(thisDate).clone().subtract(1, 'days');
    if (prevDate.isBefore(moment(firstDate))) {
        return false;
    }

    return true;
};

var enableNextButtons = function (thisDate, lastDate) {

    // nextDate will be the first of the next month.
    nextDate = moment(thisDate).clone().add(1, 'days');
    if (nextDate.isAfter(moment(lastDate))) {
        return false;
    }

    return true;
};

// Called when user clicks Calendar button.
$("#buttonCalendar").on('click', function (e) {

    e.preventDefault();

    // Do nothing if the clicked button is disabled.
    if ($("#buttonCalendar").hasClass("disabled")) {
        return;
    }

    var thisDate = getDate();
    var url = '/' + username + '/' + thisDate.year() + '/' + (thisDate.month() + 1);

    window.location.href = url;
});
