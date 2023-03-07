import template from './footer-layout.html';

function footerLayoutCmpController(tsLabelFactory, $state) {
  let vm = this;
  vm._l = tsLabelFactory;

  vm.$onInit = function () {
    vm.setActive();
    vm.hasPaysheetReadPrivilege = checkPaysheetReadPrivilege();
  }

  vm.setActive = function () {
    if(window.location.href.indexOf("paysheet") > -1) {
      $("#timesheet").removeClass('active');
      $("#paysheet").addClass('active');
    }
    else {
      $("#paysheet").removeClass('active');
      $("#timesheet").addClass('active');
    }
  };
  function checkPaysheetReadPrivilege(){
    var sessionJson = localStorage["kvSession"]
        ? JSON.parse(localStorage["kvSession"])
        : null;
      if (sessionJson) {
        return sessionJson.hasPaysheetReadPrivilege === 'true';
      }
      return false;
  }
}

footerLayoutCmpController.$inject = ['tsLabelFactory','$state'];

const footerLayoutCmp = {
  restrict: 'E',
  bindings: {},
  template: template,
  controller: footerLayoutCmpController,
  controllerAs: 'vm',
};

export default footerLayoutCmp;
