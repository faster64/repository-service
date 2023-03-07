import headerPaySheetLayoutCmp from './components/header-paysheet-layout/header-paysheet-layout.component';
import paysheetCmp from './components/paysheet/paysheet.component';
import paysheetCreateViewCmp from './components/paysheet-create-view/paysheet-create-view.component';
import paysheetDetailViewCmp from './components/paysheet-detail-view/paysheet-detail-view.component';
import paysheetSearchViewCmp from './components/paysheet-search-view/paysheet-search-view.component';
import payslipDetailViewCmp from './components/payslip-detail-view/payslip-detail-view.component';
import payslipSearchViewCmp from './components/payslip-search-view/payslip-search-view.component';
import paysheetService from './services/paysheet.service';

const paysheetModule = angular.module('paysheet', [
  'ngRoute',
  'ui.router',
  'ngSanitize',
]);
paysheetModule.service('paysheetService', paysheetService);
paysheetModule
  .component('paysheetCmp', paysheetCmp)
  .component('headerPaySheetLayoutCmp', headerPaySheetLayoutCmp)  
  .component('paysheetDetailViewCmp', paysheetDetailViewCmp)
  .component('paysheetCreateViewCmp', paysheetCreateViewCmp)
  .component('paysheetSearchViewCmp', paysheetSearchViewCmp)
  .component('payslipDetailViewCmp', payslipDetailViewCmp)
  .component('payslipSearchViewCmp', payslipSearchViewCmp);

paysheetModule.config([
  '$routeProvider',
  function ($routeProvider) {
    $routeProvider
      .when('/paysheet', {
        template: "<paysheet-cmp class='main-page'></paysheet-cmp>",
      })
      .when('/paysheet/detail/:id', {
        template: "<paysheet-detail-view-cmp class='main-page'></paysheet-detail-view-cmp>",
      })
      .when('/paysheet/create/', {
        template: "<paysheet-create-view-cmp class='main-page'></paysheet-create-view-cmp>",
      })
      .when('/paysheet/search/', {
        template: "<paysheet-search-view-cmp class='main-page'></paysheet-search-view-cmp>",
      })
      .when('/paysheet/payslip-detail/', {
        template: "<payslip-detail-view-cmp class='main-page'></payslip-detail-view-cmp>",
      })
      .when('/paysheet/paysheet-payslip-search/', {
        template: "<payslip-search-view-cmp class='main-page'></payslip-search-view-cmp>",
      })
      .otherwise({
        redirectTo: '/paysheet',
      });
  },
]);

export default paysheetModule;
