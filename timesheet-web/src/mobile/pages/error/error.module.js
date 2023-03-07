import error401Cmp from './components/error401/error-401.component';
import error404Cmp from './components/error404/error-404.component';
import error500Cmp from './components/error500/error-500.component';

const errorModule = angular.module('error', ['ngRoute']);
errorModule.component('error401Cmp', error401Cmp);
errorModule.component('error404Cmp', error404Cmp);
errorModule.component('error500Cmp', error500Cmp);

errorModule.config([
  '$routeProvider',
  function ($routeProvider) {
    $routeProvider
      .when('/error/401', {
        template: '<error-401-cmp></error-401-cmp>',
      })
      .when('/error/404', {
        template: '<error-404-cmp></error-404-cmp>',
      })
      .when('/error/500', {
        template: '<error-500-cmp></error-500-cmp>',
      })
      .otherwise({
        redirectTo: '/error/404',
      });
  },
]);

export default errorModule;
