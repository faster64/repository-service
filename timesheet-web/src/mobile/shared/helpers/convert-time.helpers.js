export const convertTimeToLong = (time) => {
    return moment(time).hours() * 60 + moment(time).minutes();
};

export const convertLongToTime = (num) => {
    return moment(parseInt(num / 60) + ':' + (num % 60), 'HH:mm').toDate();
};

export const isTimeInBeforeAndAfter = (from, to) => {
    if (from === 0 || to === 0 || !from || !to) return false;
    if ((from.getTime() - to.getTime()) / 60000 >= 0) {
        return false;
    } else {
        return true;
    }
};

export const getTimeOfShift = (shift) => {
    const to = (shift || {}).to ? moment().startOf('day').minutes(shift.to).format('HH:mm') : '00:00';
    const from = (shift || {}).from ? moment().startOf('day').minutes(shift.from).format('HH:mm') : '00:00';
    const timeOfShift = from + ' - ' + to;
    return timeOfShift;
};

export const renderTimeSheetSchedule = (timeSheet, constants) => {
    if (!timeSheet.isRepeat) {
        return '<span> </span>';
    }

    if (timeSheet.repeatType === constants.timeSheetRepeatType.values.Daily) {
        return (
            '<span>' +
            'Lặp lại mỗi ' +
            (timeSheet.repeatEachDay > 1 ? timeSheet.repeatEachDay + ' ' : '') +
            'ngày' +
            '</span>'
        );
    }

    if (timeSheet.repeatType === constants.timeSheetRepeatType.values.Weekly) {
        const repeateDays = constants.timeSheetDayOfWeek.filter(function (day) {
            return timeSheet.repeatDaysOfWeek.split(',').some(function (obj) {
                return parseInt(obj) === day.value;
            });
        });

        const text = '';
        if (repeateDays && repeateDays.length > 0) {
            text +=
                repeateDays
                    .map(function (obj) {
                        return obj.text;
                    })
                    .join(',') + ' ';
        }

        return (
            '<span>' +
            'Lặp lại mỗi ' +
            (timeSheet.repeatEachDay > 1 ? timeSheet.repeatEachDay + ' ' : '') +
            'tuần, <br> vào ' +
            text +
            '</span>'
        );
    }
    return '';
};

export const setTimeByShift = (startTime, shift) => {
    endTime = moment(startTime).startOf('day').minutes(shift.to);
    if (shift.from > shift.to) {
        endTime.add('days', 1);
    }
    return {
        startTime: moment(startTime).startOf('day').minutes(shift.from).toDate(),
        endTime: endTime.toDate(),
    };
};

export const diffHours = (dt2, dt1) => {
    const diff = moment(dt2).diff(moment(dt1), 'minutes') / 60;
    return Math.abs(diff);
};

export const diffHoursToMinute = (dt2, dt1) => {
    const diffDate = moment(dt2).diff(moment(dt1));
    let minutes = Math.abs(moment.duration(diffDate).asMinutes());
    if (diffDate < 0) {
        minutes = 24 * 60 - minutes;
    }
    return minutes;
};

export const minutestToHourMin = (mins, kvLabel) => {
    const h = Math.floor(mins / 60);
    const m = mins % 60;
    const hours =
        m > 0
            ? h > 0
                ? h + ' ' + kvLabel.timesheet_hour.toLowerCase()
                : ''
            : h > 0
            ? h + ' ' + kvLabel.timesheet_hour.toLowerCase()
            : '';
    const minutes = m > 0 ? m + ' ' + kvLabel.timesheet_minute.toLowerCase() : '';

    return hours + ' ' + minutes;
};

export const toCurrencyFormat = (number, decimalDigits) => {
    let value = number.toFixed(decimalDigits).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    if (decimalDigits > 0) {
        // Kiểm tra nếu giá trị decimal = int sẽ chỉ lấy phần nguyên
        if (value.match(/\./)) {
            value = value.replace(/\.?0+$/, '');
        }
    }
    return value;
};

export const validateExpiredTimeSheet = (tsSession, logger, constants) => {
    var timeSheetPosParameter = tsSession.posParameter
        ? tsSession.posParameter.find(function (param) {
              return param.key === 'TimeSheet';
          })
        : {};
    if (timeSheetPosParameter) {
        if (!timeSheetPosParameter.isActive) {
            if (logger) {
                logger.error(constants.timeSheetExpiredMessage.InActive);
            }
            return false;
        }
        if (timeSheetPosParameter.isExpired) {
            if (logger) {
                if (timeSheetPosParameter.posParameterType === 0) {
                    logger.error(constants.timeSheetExpiredMessage.Trial);
                } else {
                    logger.error(constants.timeSheetExpiredMessage.Paid);
                }
            }
            return false;
        }
        return true;
    }
    return true;
};

export const createTimeSchedule = (num) => {
    if (num < 2) return 1;
    return createTimeSchedule(num - 2) + createTimeSchedule(num - 1);
};

export const toHourMin = (mins) => {
    const h = Math.floor(mins / 60);
    const m = mins % 60;
    const hours = h > 0 ? h + 'h' : '';
    const minutes = m > 0 ? m + 'p' : '';

    return hours + minutes;
};

export const timeToDecimal = (t) => {
    const arr = t.split(':');
    const dec = parseInt((arr[1] / 6) * 10, 10);

    return parseFloat(parseInt(arr[0], 10) + '.' + (dec < 10 ? '0' : '') + dec);
};

Date.prototype.addDays = function (days) {
    var date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date;
};
