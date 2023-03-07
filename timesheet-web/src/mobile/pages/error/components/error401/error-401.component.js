import template from './error-401.html';

function error401CmpController(tsFn, tsLabelFactory) {
  //#region variables
  let vm = this;
  vm._l = tsLabelFactory;
  vm.fn = tsFn;
  //#endregion
}

error401CmpController.$inject = ['tsFnFactory', 'tsLabelFactory'];

const error401Cmp = {
  restrict: 'E',
  bindings: {},
  template: template,
  controller: error401CmpController,
  controllerAs: 'vm',
};

export default error401Cmp;
