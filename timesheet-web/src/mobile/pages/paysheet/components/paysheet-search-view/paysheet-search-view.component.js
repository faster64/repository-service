import toastr from "toastr";
import template from './paysheet-search-view.html';

function paysheetSearchViewCmpController(
  $routeParams,
  tsFn,
  tsLabelFactory,
  paysheetService,
  $compile,
  $scope,
  $location
) {
  //#region variables
  let vm = this;
  vm._l = tsLabelFactory;
  vm.paysheets = [];
  vm.paysheetIds = [];
  vm.paysheetStatusIds = [1,2,4];
  vm.searchParam = "";
  vm.isNoSearchData = false;
  var timer;
  //#endregion
  
  //#region scope
  $scope.redirectToDetail = function(id){
    const item = vm.paysheets.find((x) => x.id == id) || {};
    if (item) {
      localStorage.setItem("paramSearchPaysheet", vm.searchParam);
      localStorage.setItem("paysheet", JSON.stringify(item));
      $location.path(`/paysheet/detail/${id}`);
    } else {
      toastr.error(vm._l.noData);
    }
  };
  //#endregion

  //#region vm
  vm.$onInit = function () {
    $("#loading").hide();
    vm.searchParam = localStorage.getItem("paramSearchPaysheet") == null ? "" : localStorage.getItem("paramSearchPaysheet");
    vm.search();
    vm.isNoSearchData = false;

    vm.paysheetDelete = JSON.parse(localStorage.getItem("paysheetDelete"));
    if (vm.paysheetDelete) {
      $('#ts-toast-delete-payroll-search-paysheet').show();
      timerToast();
      localStorage.removeItem('paysheetDelete');
    }
  };

  vm.search = function () {
    if (vm.searchParam.length == 0) {
      vm.isNoSearchData = false;
      $("#ts-staff-items").html('');
    }
    else if (vm.searchParam.length > 3) {
      vm.getPaysheets();
    }
  };

  vm.clearSearchParam = function () {
    vm.searchParam = "";
    vm.search();
  };

  vm.getPaysheets = function () {
    var data = getAdditionalParam({ 
      skip: 0,
      take: 10
    });
      
    paysheetService.getPaysheets(data).then(
      function (resp) {
        if (resp && resp.result) {
          if (resp.result.data.length == 0) {
            vm.isNoSearchData = true;
          }
          else {
            vm.paysheets = resp.result.data || [];
            vm.isNoSearchData = false;
            vm.renderTsPaysheet();
          }
        }
      },
      function (err) {
        vm.error = err.message;
        $("#loading").hide();
      }
    );
  };

  vm.renderTsPaysheet = function () {
    try {
      $("#ts-staff-items").html('');
      vm.tsPaySheetMain = $("#ts-staff-items");
      for (let i = 0; i < vm.paysheets.length; i++) {
        let paysheet = vm.paysheets[i];        
        let item = vm.renderPaysheetItem(paysheet);
        vm.tsPaySheetMain.append(item);
      }
      vm.tsPaySheetMain.append(vm.tsPaySheetMain);
      $("#loading").hide();
    } catch (error) {
      console.log(error);
      $("#loading").hide();
    }
  };

  vm.renderPaysheetItem = function (paysheet) {
    let paysheetElement = $('<div class="ts-staff-item"></div>');

    let tsEvent = $compile(
      `<a ng-click="redirectToDetail(${paysheet.id})"></a>`
    )($scope);
    let tsTimelineEvent = $(tsEvent);

    let itemElement = $(
    ' <div class="ts-payroll"> ' +
      ' <span><i class="far fa-search"></i></span> ' +
      ' <span> ' + vm._l.paysheet + ' ' + paysheet.code + ' </span> ' +
    ' </div> ');
    tsTimelineEvent.append(itemElement);    
    paysheetElement.append(tsTimelineEvent);
    return paysheetElement;
  };

  vm.backToPaysheet = function () {
    localStorage.removeItem("paramSearchPaysheet");
    history.back();
  };
  //#endregion

  //#region function
  function getAdditionalParam(data) {
    const param = convertKendoDsDataToQueryParams(data);
    param.PaysheetKeyword = vm.searchParam;
    param.BranchIds = vm.branchIds;
    param.PaysheetStatuses = [1,2,4];
    return param;
  }
  
  function timerToast() {
    window.clearTimeout(timer);
    timer = setTimeout(() => {
      $('#ts-toast-delete-payroll-search-paysheet').hide();
    }, 2000);
  }
  //#endregion

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
        params.OrderByDesc = 'id';
    }

    return params;
  };
}

paysheetSearchViewCmpController.$inject = [
  '$routeParams',
  'tsFnFactory',
  'tsLabelFactory',
  'paysheetService',
  '$compile',
  '$scope',
  '$location',
];

const paysheetSearchViewCmp = {
  restrict: 'E',
  bindings: {},
  template: template,
  controller: paysheetSearchViewCmpController,
  controllerAs: 'vm',
};

export default paysheetSearchViewCmp;
