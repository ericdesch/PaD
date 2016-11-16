
// Function to calculate previous date.
var calculatePrevDate = function (date, firstDate) {

    // prevDate will be the first of the previous month.
    var prevDate = moment(date).clone().subtract(1, 'months');
    // Set to first in case to was firstDate or lastDate previously
    prevDate.date(1);
    if (prevDate.isBefore(moment(firstDate))) {
        prevDate = firstDate;
    }

    return prevDate;
};

// Function to calculate next date.
var calculateNextDate = function (date, lastDate) {

    // nextDate will be the first of the next month.
    var nextDate = moment(date).clone().add(1, 'months');
    // Set to first in case to was firstDate or lastDate previously
    nextDate.date(1);
    if (nextDate.isAfter(moment(lastDate))) {
        nextDate = lastDate;
    }

    return nextDate;
};

var enablePrevButtons = function (date, firstDate) {

    // prevDate will be the last date of the previous month.
    var prevDate = moment(date).clone().subtract(1, 'days');
    if (prevDate.isBefore(moment(firstDate))) {
        return false;
    }

    return true;
};

var enableNextButtons = function (date, lastDate) {

    // nextDate will be the first of the next month.
    var nextDate = moment(date).clone().add(1, 'months');
    if (nextDate.isAfter(moment(lastDate))) {
        return false;
    }

    return true;
};
