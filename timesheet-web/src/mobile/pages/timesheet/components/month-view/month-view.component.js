import template from './month-view.html';

function monthViewCmpController(tsLabelFactory, timesheetService) {
  //#region variables
  let vm = this;
  vm._l = tsLabelFactory;
  //#endregion

  vm.$onInit = function () {};
}

monthViewCmpController.$inject = ['tsLabelFactory', 'timesheetService'];

const monthViewCmp = {
  restrict: 'E',
  bindings: {},
  template: template,
  controller: monthViewCmpController,
  controllerAs: 'vm',
};

export default monthViewCmp;
