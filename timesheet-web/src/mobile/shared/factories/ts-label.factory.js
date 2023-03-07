function tsLabelFactory(tslabelEnglish, tsLabelViet) {
  var lang = localStorage.getItem('lang') || 'vi-VN';
  if (lang === 'en-US') return tslabelEnglish;
  return tsLabelViet;
}

tsLabelFactory.$inject = ['tsLabel.en-US', 'tsLabel.vi-VN'];

export default tsLabelFactory;
