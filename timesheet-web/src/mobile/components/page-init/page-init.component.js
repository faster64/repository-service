import template from './page-init.html';

function pageInitCmpController($rootScope, $location) {
  $rootScope.identityKeyClocking = null;
  $rootScope.geoCoordinate = null;
  init();

  function init() {
    $('#loading-init').show();
    var para = $location.search();
    if (!para) {
      $location.url('error/404');
      return;
    }
    var sessionData = {
      bearerToken: para.accessToken,
      branchId: para.branchId,
      hasPaysheetReadPrivilege: para.hasPaysheetReadPrivilege
    };
    localStorage['kvSession'] = JSON.stringify(sessionData);
    $('#loading-init').hide();
    $location.url('timesheet');
  }
}

pageInitCmpController.$inject = ['$rootScope', '$location'];

const pageInitCmp = {
  restrict: 'E',
  bindings: {},
  template: template,
  controller: pageInitCmpController,
  controllerAs: 'vm',
};

export default pageInitCmp;
