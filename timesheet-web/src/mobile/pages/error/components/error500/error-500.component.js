import template from './error-500.html';

function error500CmpController(tsLabelFactory) {
  //#region variables
  let vm = this;
  vm._l = tsLabelFactory;
  //#endregion
}

error500CmpController.$inject = ['tsLabelFactory'];

const error500Cmp = {
  restrict: 'E',
  bindings: {},
  template: template,
  controller: error500CmpController,
  controllerAs: 'vm',
};

export default error500Cmp;
