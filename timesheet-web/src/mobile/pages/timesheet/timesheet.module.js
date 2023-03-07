import dayViewCmp from './components/day-view/day-view.component';
import detailViewCmp from './components/detail-view/detail-view.component';
import headerTimeSheetLayoutCmp from './components/header-timesheet-layout/header-timesheet-layout.component';
import monthViewCmp from './components/month-view/month-view.component';
import timesheetCmp from './components/timesheet/timesheet.component';
import weekViewCmp from './components/week-view/week-view.component';
import timesheetService from './services/timesheet.service';

const timesheetModule = angular.module('timesheet', [
  'ngRoute',
  'ui.router',
  'ngSanitize',
]);
timesheetModule.service('timesheetService', timesheetService);
timesheetModule
  .component('timesheetCmp', timesheetCmp)
  .component('headerTimeSheetLayoutCmp', headerTimeSheetLayoutCmp)
  .component('dayViewCmp', dayViewCmp)
  .component('weekViewCmp', weekViewCmp)
  .component('monthViewCmp', monthViewCmp)
  .component('detailViewCmp', detailViewCmp);

timesheetModule.config([
  '$routeProvider',
  '$stateProvider',
  function ($routeProvider, $stateProvider) {
    $routeProvider
      .when('/timesheet', {
        template: "<timesheet-cmp class='main-page'></timesheet-cmp>",
      })
      .when('/timesheet/detail/:id', {
        template: "<detail-view-cmp class='main-page'></detail-view-cmp>",
      })
      .otherwise({
        redirectTo: '/timesheet',
      });

    $stateProvider
      .state({
        name: 'date',
        template: `<day-view-cmp start-Date="vm.startDate" end-Date="vm.endDate" selected-Date="vm.selectedDate" branch-Id="vm.branchId" instance="vm.instance"></day-view-cmp>`,
      })
      .state({
        name: 'week',
        template: `<week-view-cmp start-Date="vm.startDate" end-Date="vm.endDate" selected-Date="vm.selectedDate" branch-Id="vm.branchId" shift-Id="vm.shiftId" instance="vm.instance"></week-view-cmp>`,
      })
      .state({
        name: 'month',
        template: `<month-view-cmp></month-view-cmp>`,
      })
  },
]);

export default timesheetModule;
