import template from './error-404.html';

function error404CmpController(tsLabelFactory) {
  //#region variables
  let vm = this;
  vm._l = tsLabelFactory;
  //#endregion
}

error404CmpController.$inject = ['tsLabelFactory'];

const error404Cmp = {
  restrict: 'E',
  bindings: {},
  template: template,
  controller: error404CmpController,
  controllerAs: 'vm',
};

export default error404Cmp;
