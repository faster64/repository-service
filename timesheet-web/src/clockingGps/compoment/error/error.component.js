import template from "./error.html";

function errorCmpController($rootScope) {
  let vm = this;
  vm.errorMessage = $rootScope.errorMessage;
}

errorCmpController.$inject = ["$rootScope"];

const errorCmp = {
  restrict: "E",
  bindings: {},
  template: template,
  controller: errorCmpController,
  controllerAs: "vm",
};

export default errorCmp;
