import template from './payslip-detail-view.html';

function payslipDetailViewCmpController(
  tsFn,
  tsLabelFactory,
  paysheetService
) {
  //#region variables
  let vm = this;
  vm._l = tsLabelFactory;
  //#endregion

  //#region vm
  vm.$onInit = function () {
    vm.payslip = JSON.parse(localStorage.getItem('paysheetPayslipDetail'));
    vm.paysheet = JSON.parse(localStorage.getItem('paysheet'));
  };

  vm.back = function () {
    localStorage.removeItem("paysheetPayslipDetail");
    history.back();
  };

  //#endregion
}

payslipDetailViewCmpController.$inject = [
  'tsFnFactory',
  'tsLabelFactory',
  'paysheetService',
];

const payslipDetailViewCmp = {
  restrict: 'E',
  bindings: {},
  template: template,
  controller: payslipDetailViewCmpController,
  controllerAs: 'vm',
};

export default payslipDetailViewCmp;
