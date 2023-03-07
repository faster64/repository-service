import footerLayoutCmp from './components/footer-layout/footer-layout.component';
import pageInitCmp from './components/page-init/page-init.component';
import errorModule from './pages/error/error.module';
import timesheetModule from './pages/timesheet/timesheet.module';
import paysheetModule from './pages/paysheet/paysheet.module';
import tsLabelEnglish from './shared/factories/locales/en-US/ts-label.en-US.factory';
import tsLabelVietnamese from './shared/factories/locales/vi-VN/ts-label.vi-VN.factory';
import tsFnFactory from './shared/factories/ts-fn.factory';
import tsLabelFactory from './shared/factories/ts-label.factory';

const mobileModule = angular.module('mobile', [
  'ngRoute',
  timesheetModule.name,
  paysheetModule.name,
  errorModule.name,
]);
mobileModule
  .component('pageInitCmp', pageInitCmp)
  .component('footerLayoutCmp', footerLayoutCmp);
mobileModule
  .factory('tsLabel.vi-VN', tsLabelVietnamese)
  .factory('tsLabel.en-US', tsLabelEnglish)
  .factory('tsLabelFactory', tsLabelFactory)
  .factory('tsFnFactory', tsFnFactory);

mobileModule.config([
  '$routeProvider',
  function ($routeProvider) {
    $routeProvider
      .when('/init', {
        template: "<page-init-cmp class='main-page'></page-init-cmp>",
      })
      .otherwise({
        redirectTo: '/error/404',
      });
  },
]);

export default mobileModule;
