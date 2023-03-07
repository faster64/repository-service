function tsFnFactory(_l) {
  return {
    toHHMM: function (minutes) {
      if (minutes == '0') return '00:00';
      if (!minutes) return '';
      var min_num = parseInt(minutes, 10);
      var hours = Math.floor(min_num / 60);
      var minutes = Math.floor(min_num % 60);

      return [hours, minutes]
        .map((v) => (v < 10 ? '0' + v : v))
        //.filter((v, i) => v !== '00' || i > 0)
        .join(':');
    },
    toWorkTime: function (minutes) {
      if (!minutes) return '';
      var min_num = parseInt(minutes, 10);
      var hours = Math.floor(min_num / 60);
      var minutes = Math.floor(min_num % 60);

      return hours > 0 ? hours + 'h' + minutes + 'p' : minutes + 'p';
    },
    getDayName: function (kendo, date, mode) {
      if (!kendo || !date) return '';
      var dayInfo = kendo.culture().calendar.days;
      var dn = moment(date).format('d');
      if (mode === 0) return dayInfo.namesAbbr[dn];
      return dayInfo.names[dn];
    },
    backToApp: function () {
      if (typeof Android != 'undefined') {
        Android.backToMainApp();
      } else {
        if (
          window.webkit &&
          window.webkit.messageHandlers &&
          window.webkit.messageHandlers.sendToNative
        ) {
          window.webkit.messageHandlers.sendToNative.postMessage(
            'backToMainApp'
          );
        }
      }
    },
  };
}

tsFnFactory.$inject = ['tsLabelFactory'];

export default tsFnFactory;
