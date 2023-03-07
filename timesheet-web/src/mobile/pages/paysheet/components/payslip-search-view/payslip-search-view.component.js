import template from './payslip-search-view.html';

function payslipSearchViewCmpController(
  $routeParams,
  tsFn,
  tsLabelFactory,
  paysheetService,
  $state,
  $compile,
  $scope,
  $location
) {
  //#region variables
  let vm = this;
  vm._l = tsLabelFactory;
  vm.branchId = undefined;
  vm.employees = [];
  vm.searchParam = "";
  //#endregion
  
  //#region scope
  $scope.redirectToDetail = function(id){
    $location.path(`/paysheet/detail/${vm.paysheet.id}`);
  };
  //#endregion

  //#region vm
  vm.$onInit = function () {
    $("#loading").hide();    
    vm.searchParam = localStorage.getItem("paramSearchEmployee") == null ? "" : localStorage.getItem("paramSearchEmployee");
    vm.paysheet = JSON.parse(localStorage.getItem('paysheet'));
    var sessionJson = localStorage["kvSession"]
    ? JSON.parse(localStorage["kvSession"])
    : null;
    vm.branchId = sessionJson.branchId;
    vm.search();
  };

  vm.search = function () {
    if (vm.searchParam.length > 2) {
      vm.getEmployeeForPaysheet();
    }
    else {
      vm.employees = [];
      $("#ts-staff-items").html('');
    }
  };

  vm.clearSearchParam = function () {
    vm.searchParam = "";
    vm.search();
  };

  vm.backToPaysheetDetail = function () {
    localStorage.setItem("tsViewByPaysheet", 4);
    localStorage.removeItem("paramSearchEmployee");
    vm.paysheet = localStorage.getItem("paysheet");
    $location.path(`/timesheet/paysheet-detail/${vm.paysheet.id}`);
  };
  
  vm.getEmployeeForPaysheet = function () {
    var data = getAdditionalParam({ 
      skip: 0,
      take: 10
    });
      
    paysheetService.getEmployeeForPaysheet(data).then(
      function (resp) {
        if (resp.result?.data) {
          vm.employees = resp.result.data || [];
          vm.renderTsEmployee();
        }
      },
      function (err) {
        vm.error = err.message;
        $("#loading").hide();
      }
    );
  };

  vm.renderTsEmployee = function () {
    try {
      $("#ts-staff-items").html('');
      vm.tsEmployeeMain = $("#ts-staff-items");
      for (let i = 0; i < vm.employees.length; i++) {
        let employee = vm.employees[i];        
        let item = vm.renderEmployeeItem(employee);
        vm.tsEmployeeMain.append(item);
      }
      vm.tsEmployeeMain.append(vm.tsEmployeeMain);
      $("#loading").hide();
    } catch (error) {
      console.log(error);
      $("#loading").hide();
    }
  };

  vm.renderEmployeeItem = function (employee) {
    let employeeElement = $('<div class="ts-staff-item"></div>');

    let tsEvent = $compile(
      `<a ng-click="redirectToDetail(${employee.id})"></a>`
    )($scope);
    let tsTimelineEvent = $(tsEvent);

    let itemElement = $(
    ' <div class="ts-payroll"> ' +
      ' <span><i class="far fa-search"></i></span> ' +
      ' <span> ' + employee.name + ' </span> ' +
    ' </div> ');
    tsTimelineEvent.append(itemElement);    
    employeeElement.append(tsTimelineEvent);
    return employeeElement;
  };
  //#endregion

  //#region function
  function getAdditionalParam(data) {
    const param = convertKendoDsDataToQueryParams(data);
    param.Keyword = vm.searchParam.toLowerCase().trim();
    param.SalaryPeriod = vm.paysheet.salaryPeriod;
    param.StartTime = vm.paysheet.startTime;
    param.EndTime = vm.paysheet.endTime;
    param.BranchId = vm.branchId;
    return param;
  }

  const convertKendoDsDataToQueryParams = (data) => {
    const params = {
        skip: data.skip,
        take: data.take,
    };

    if (data && data.sort && data.sort.length > 0) {
        if (data.sort[0].dir === 'desc') {
            params.OrderByDesc = data.sort[0].field;
        } else {
            params.OrderBy = data.sort[0].field;
        }
    } else {
      params.OrderBy = 'id';
    }

    return params;
  };
  //#endregion
}

payslipSearchViewCmpController.$inject = [
  '$routeParams',
  'tsFnFactory',
  'tsLabelFactory',
  'paysheetService',
  '$state',
  '$compile',
  '$scope',
  '$location',
];

const payslipSearchViewCmp = {
  restrict: 'E',
  bindings: {},
  template: template,
  controller: payslipSearchViewCmpController,
  controllerAs: 'vm',
};

export default payslipSearchViewCmp;
