import toastr from "toastr";
import { getErrorMsg } from '../../../../shared/helpers/notify-reponse.helpers';
import template from './paysheet-detail-view.html';

function paysheetDetailViewCmpController(
  $routeParams,
  tsFn,
  tsLabelFactory,
  paysheetService,
  $location,
  $scope,
  $compile
) {
  //#region variables
  let vm = this;
  vm._l = tsLabelFactory;
  vm.branchId = undefined;
  vm.paysheet = undefined;
  vm.payslips = [];
  vm.isNoPayslipData = false;
  vm.isTemporary = false;
  vm.searchParam = false; 
  vm.skip = 0;
  vm.take = 1000;
  var timer;
  //#endregion

  //#region scope
  $scope.redirectToPayslipDetail = function(id){
    const item = vm.payslips.find((x) => x.id == id) || {};
    if (item) {
      localStorage.setItem("paysheetPayslipDetail", JSON.stringify(item));
      $location.path(`/paysheet/payslip-detail/`);
    }
  };
  //#endregion

  //#region vm
  vm.$onInit = function () {
    initBranches();  
    initPaysheet();
    initModal();
  };

  vm.searchPaysheetDetail = function (event) {
    
  };

  vm.cancelDeleteDetail = function (event) {
    closeModal($(".js-ts-modal-close").closest(".ts-modal"));
  }

  vm.cancelDeletePayslipPayment = function (event) {
    vm.deletePaysheetDetail(false, false);
  }

  vm.deletePayslipPayment = function (event) {
    vm.deletePaysheetDetail(false, true);
  }

  vm.deletePaysheetDetail = function (isCheckPayslipPayment, isCancelPayment) {
    $("#loading").show();
    paysheetService.cancelPaysheet(vm.paysheet.id, isCheckPayslipPayment, isCancelPayment).then(
      function (resp) {
        $("#loading").hide();
        if (resp && resp.result) {
          localStorage.setItem('paysheetDelete', JSON.stringify(vm.paysheet));
          vm.backToPaysheet();
        }
        else {
          vm.closeDeletePaysheetModal();
          vm.openDeletePayslipPaymentModal();
        }
      },
      function (err) {
        vm.error = err.message;
        $("#loading").hide();
      }
    );
  };

  vm.closeDeletePaysheetModal = function () {
    $("#delete-payroll-modal").removeClass("open");
    $("body").removeClass("body-hidden");
  };

  vm.openDeletePayslipPaymentModal = function () {
    $("#cancel-payslip-payment-modal").addClass("open");
    $("body").addClass("body-hidden");
  };

  vm.backToPaysheet = function () {
    history.back();
  };

  vm.completePaysheetTemp = function () {
    $("#loading").show();
    paysheetService.completePaysheetTemp(transformPaysheet(vm.paysheet), true, false).then(
      function (resp) {
        $("#loading").hide();
        if (resp && resp.result) {
          var result = resp.result || [];
          if(result.paysheetStatus == 2){
            vm.isTemporary = false;
            vm.paysheet.paysheetStatus = result.paysheetStatus;
            $('.ts-toast').show();
            timerToast(); 
          } else {
            vm.isTemporary = true;
          }
        }
      },
      function (err) {
        vm.error = err.message;
        $("#loading").hide();
      }
    );
  };

  vm.formatNumber = function (number) {
    var value = number.toLocaleString(undefined, { minimumFractionDigits: 0 })
    return value;
  };

  vm.getPayslips = function () { 
    var data = getAdditionalParam({
      skip: vm.skip,
      take: vm.take 
    });

    paysheetService.getPayslipsByPaysheetId(data).then(
      function (resp) {
        if (resp && resp.result) {
          if (resp.result.data.length == 0) {
            vm.isNoPayslipData = true;
            $("#ts-main").addClass("box-main-empty-wrap");
            $("#loading").hide();
          } else {
            vm.isNoPayslipData = false;
            vm.renderPayslips(resp.result.data);
          }
        }
        else {
          vm.isNoPayslipData = true;
          $("#ts-main").addClass("box-main-empty-wrap");
          $("#loading").hide();
        }
      },
      function (err) {
        vm.error = err.message;
        $("#loading").hide();
      }
    );
  }

  vm.getAndCheckChangeById = function () {
    paysheetService.getAndCheckChangeById(vm.paysheet.id, vm.paysheet.branchId, vm.branchId).then(
      function (resp) {
        if (resp && resp.result) {
          localStorage.setItem("paysheet", JSON.stringify(vm.paysheet));
          
          if (resp.result.payslips.length == 0) {
            toastr.error(vm._l.paysheet_payslip_empty);  
            $("#ts-main").addClass("box-main-empty-wrap");
            vm.isNoPayslipData = true;
            $("#loading").hide();
            return;
          }
          vm.paysheet = resp.result;    
          vm.isNoPayslipData = false;
          vm.renderPayslips(resp.result.payslips);
          
          $("#loading").hide();
        } else {
          $("#ts-main").addClass("box-main-empty-wrap");
          vm.isNoPayslipData = true;
          $("#loading").hide();
        }
      },
      function (err) {
        vm.error = err.message;
        $("#loading").hide();
      }
    );
  }

  vm.renderPayslips = function (payslips) {
    try {
      vm.tsPaySlipMain = $("#ts-staff-main-payslip");  
      for (let i = 0; i < payslips.length; i++) {
        let payslip = payslips[i];
        vm.payslips.push(payslip);
        let item = vm.renderPayslipItem(payslip);
        vm.tsPaySlipMain.append(item);
      }
      vm.tsPaySlipMain.append(vm.tsPaySlipMain);
      $("#loading").hide();
    } catch (error) {
      vm.error = err.message;
      $("#loading").hide();
    }
  };
  
  vm.renderPayslipItem = function (payslip) {
    let payslipElement = $('<div class="ts-staff-wrap box-main ts-payroll-detail-list"></div>');
    let payrollElement = $('<div class="ts-payroll-detail-item"></div>');
    let tsHeader = $('<div class="ts-payroll-detail-item-head"></div>');
    let tsMain = $('<div class="ts-payroll-detail-item-main"></div>'); 

    let tsEvent = $compile(
      `<a ng-click="redirectToPayslipDetail(${payslip.id})"></a>`
    )($scope);
    let tsTimelineEvent = $(tsEvent);

    //#region header
    let headerElement = $(
      '<h5>' + payslip.employee.name + '</h5> ' +
      '<span>' + payslip.employee.code + '</span> ');
    tsTimelineEvent.append(headerElement);    
    tsHeader.append(tsTimelineEvent);
    //#endregion

    //#region main
    let tsMainElement = $(
    ' <div class="ts-box"> ' +
      ` <span>${vm._l.employee_payslipsAmountToPay}</span> ` +
      ' <strong> ' + vm.formatNumber(payslip.totalNeedPay) + ' </strong> ' +
    ' </div> ' +
    ' <div class="ts-box"> ' +
      ` <span>${vm._l.employee_payslipsMainSalary}</span> ` +
      ' <strong> ' + vm.formatNumber(payslip.mainSalary) + ' </strong> ' +
    ' </div> ' +
    ' <div class="ts-box"> ' +
      ` <span>${vm._l.timesheet_overTime}</span> ` +
      ' <strong> ' + vm.formatNumber(payslip.overtimeSalary) + ' </strong> ' +
    ' </div> ' +
    ' <div class="ts-box"> ' +
      ` <span>${vm._l.payRate_commission}</span> ` +
      ' <strong> ' + vm.formatNumber(payslip.commissionSalary) + ' </strong> ' +
    ' </div> ');
    tsMain.append(tsMainElement);
    //#endregion

    payrollElement.append(tsHeader);
    payrollElement.append(tsMain);
    payslipElement.append(payrollElement);
    return payslipElement;
  };

  vm.openModal = function (event) {
    var popupid = $(event.currentTarget).attr("rel");
    $("#" + popupid).addClass("open");
    $("body").addClass("body-hidden");
  };

  //#endregion

  //#region function
  function initBranches() {
    var sessionJson = localStorage["kvSession"]
      ? JSON.parse(localStorage["kvSession"])
      : null;
    vm.branchId = sessionJson.branchId;
  }
  
  function initPaysheet() {
    $("#loading").show();
    paysheetService.getPaysheetById($routeParams.id).then(
      function (resp) {
        if (resp && resp.result) {
          vm.paysheet = resp.result || [];

          localStorage.setItem("paysheet", JSON.stringify(vm.paysheet));
          checkPaysheetStatus();
        }
      },
      function (err) {
        vm.error = err.message;
        $("#loading").hide();
      }
    );
  }

  function initModal() {
    $('.ts-toast-close').click(function () {
      $('.ts-toast').stop().hide(); // Close click
    });

    window.onscroll = function() {
      if ($(this).scrollTop()) {
        $('#toTop').fadeIn();
      } else {
        $('#toTop').fadeOut();
      }
    };
  }

  function checkPaysheetStatus() {
    if(vm.paysheet.paysheetStatus == 1) {
      vm.isTemporary = true;
      vm.getAndCheckChangeById();
    }
    else {
      vm.isTemporary = false;
      vm.getPayslips();
    }
  }
  
  /**
    * Hàm thực hiện xử lý dữ liệu trước khi gửi lên backend
    * @param {any} paysheet bảng lương
    * @return {any} paysheet sau khi transform
  */
  function transformPaysheet(paysheet) {
    if (paysheet && vm.payslips && vm.payslips.length) {
      for (let i = 0; i < vm.payslips.length; i++) {
        //xử lý tách vi phạm và giảm trừ nếu có xuất hiện
        let payslip = processPayslipDeductionAndPenalize(paysheet.payslips[i]);

        // Xử lý phiếu thanh toán
        processPayments(payslip);
      }
    }
    return paysheet;
  }

  /**
   * Xử lý phiếu thanh toán trước khi gửi lên backend
   * @param {any} payslip phiếu lương
   * @return {any} phiếu lương
   */
  function processPayments(payslip) {
    if (payslip && payslip.payslipPayments && payslip.payslipPayments.length > 0) {
      let payment = payslip.payslipPayments[0];
      if (
        payslip.payingAmount > payslip.needPay &&
        payslip.refundAdvanceOption === tsConstants.refundAdvanceTypes.employeeRefund
      ) {
          payment.amount -= payslip.excessAmount;
      }
    }

    return payslip;
  }

  function processPayslipDeductionAndPenalize(payslip) {
    if (!payslip.deductionRuleParam || !payslip.deductionRuleParam.deductions) return payslip;

    //Nếu chưa merge vi phạm giảm trừ return
    const checkExistPenalizeInDeduction = payslip.deductionRuleParam.deductions.some(function (item) {
        return item.isPenalize;
    });
    if (!checkExistPenalizeInDeduction) return payslip;

    let deductions = payslip.deductionRuleParam.deductions.filter(function (item) {
        if (!item.isPenalize) return item;
    });

    let payslipPenalizes = [];
    $.each(payslip.deductionRuleParam.deductions, function (key, item) {
      if (item.isPenalize && item.penalizeId !== 0 && item.id !== -2) {
        let payslipPenalize = {
          payslipId: payslip.id,
          penalizeId: item.deductionId,
          penalizeName: item.name,
          timesCount: item.numberValue,
          isActive: item.selectedItem,
          value: item.value,
          moneyType: 1,
        };
        payslipPenalizes.push(payslipPenalize);
      }
    });

    payslip.deductionRuleParam.deductions = deductions;

    payslip.payslipPenalizes = payslipPenalizes;

    return payslip;
  }

  function timerToast() {
    window.clearTimeout(timer);
    timer = setTimeout(() => {
      $('#ts-toast-salary-closing').hide();
    }, 2000);
  }

  function getAdditionalParam(data) {
    const param = convertKendoDsDataToQueryParams(data);
    param.PaySheetID = vm.paysheet.id;
    param.PayslipStatuses = [1,2,4];
    param.OrderBy = 'Id';
    return param;
  }
  
  function closeModal($target) {
    $target.removeClass("open");
    $("body").removeClass("body-hidden");
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

paysheetDetailViewCmpController.$inject = [
  '$routeParams',
  'tsFnFactory',
  'tsLabelFactory',
  'paysheetService',
  '$location',
  '$scope',
  '$compile',
];

const paysheetDetailViewCmp = {
  restrict: 'E',
  bindings: {},
  template: template,
  controller: paysheetDetailViewCmpController,
  controllerAs: 'vm',
};

export default paysheetDetailViewCmp;
