import toastr from "toastr";
import template from './paysheet-create-view.html';
import { addDays } from '../../../../shared/helpers/convert-time.helpers';

function paysheetCreateViewCmpController(
  tsFn,
  tsLabelFactory,
  paysheetService,
  $window
) {
  //#region variables
  let vm = this;
  vm._l = tsLabelFactory;
  vm.salaryPeriods = [];
  vm.branches = [];
  vm.workingPeriods = [];
  vm.createPaysheetPeriodOptionDays = 90;
  vm.timerId = undefined;
  var runCount = 0;
  vm.startTime = new Date();
  vm.endTime = new Date();
  
  //#endregion

  //#region vm
  vm.$onInit = function () {
    initCalendar();
    initBranches();
    initSalaryPeriod();
  };

  vm.selectSalaryPeriod = function (event) {
    closeModal($(".js-ts-modal-close").closest(".ts-modal"));
  };

  vm.chooseSalaryPeriod = function (event, item) {
    event.stopPropagation();
    closeModal($(".js-ts-modal-close").closest(".ts-modal-salaryPeriod"));

    vm.salaryPeriodSelected = item;    
    $("#loading").show();
    handleWorkingPeriod();
  };

  vm.chooseWorkingPeriod = function (event, item) {
    event.stopPropagation();
    closeModal($(".js-ts-modal-close").closest(".ts-modal-salaryPeriod"));

    vm.workingPeriodSelected = item;
  };

  vm.setStartTime = function () {
    vm.startTime = vm.calendarStartTime.selectDates()[0] || new Date();
    closeModal($(".ts-modal-payroll-calendar").closest(".ts-modal"));
  };

  vm.setEndTime = function () {
    vm.endTime = vm.calendarEndTime.selectDates()[0] || new Date();
    closeModal($(".ts-modal-payroll-calendar").closest(".ts-modal"));
  };
  
  vm.createPaysheet = function() {
    let working = vm.workingPeriods.find((x) => x.id === vm.workingPeriodSelected.id);
    let startTime = moment(vm.startTime).format('DD/MM/YYYY');
    let endTime = moment(vm.endTime).format('DD/MM/YYYY');

    if (vm.salaryPeriodSelected.value !== 5) {
      startTime = working ? moment(working.startTime).format('DD/MM/YYYY') : null;
      endTime = working ? moment(working.endTime).format('DD/MM/YYYY') : null;
    }

    $("#loading").show();
    if (!vm.validateDateTime()) {
      $("#loading").hide();
      return;
    }
    
    paysheetService.createPaysheet(startTime, endTime, vm.salaryPeriodSelected.value, vm.branches).then(
      function (resp) {
        if (resp && resp.result) {
          vm.paysheetId = resp.result.id;
          runCount = 0;
          vm.timerId = setInterval(timerMethod, 5000);
        }
        else {
          $("#loading").hide();
          vm.backToPaysheet();
        }
      },
      function (err) {
        vm.error = err.message;
        $("#loading").hide();
      }
    );
  }

  vm.validateDateTime = function () {
    if (!vm.startTime) {
      toastr.error(vm._l.msg_Invalid.replace('{0}', vm._l.paysheet_periodPayFrom));
      return false;
    }

    if (!vm.endTime) {
      toastr.error(vm._l.msg_Invalid.replace('{0}', vm._l.paysheet_periodPayTo));
      return false;
    }

    if (vm.salaryPeriodSelected.value == 5) {
      if (vm.endTime < vm.startTime) {
        toastr.error(vm._l.paysheet_msgStartTimeMustBeEndTime);
        return false;
      }
      else if (vm.startTime.addDays(vm.createPaysheetPeriodOptionDays).setHours(0, 0, 0, 0) < vm.endTime.setHours(0, 0, 0, 0)) {
        toastr.error(vm._l.paysheet_msgPayPeriodOn90Days.replace('{0}', vm.createPaysheetPeriodOptionDays));
        return false;
      }
    }

    return true;
  };
  
  vm.urlBase64Decode = function(str) {
    var output = str.replace(/-/g, '+').replace(/_/g, '/');
    switch (output.length % 4) {
      case 0: { break; }
      case 2: { output += '=='; break; }
      case 3: { output += '='; break; }
      default: {
        throw 'Illegal base64url string!';
      }
    }
    return $window.decodeURIComponent(escape($window.atob(output)));
  };

  vm.decodeToken = function (token) {
    var parts = token.split('.');

    if (parts.length !== 3) {
      throw new Error('JWT must have 3 parts');
    }

    var decoded = this.urlBase64Decode(parts[1]);
    if (!decoded) {
      throw new Error('Cannot decode the token');
    }

    return angular.fromJson(decoded);
  };

  vm.backToPaysheet = function () {
    history.back();
  };

  vm.backFromModal = function(event) {
    $("#ts-salaryPeriod-modal").removeClass("open");
    $("#ts-workingPeriod-modal").removeClass("open");
    $("#ts-period-modal").removeClass("body-hidden");
  }

  vm.openModal = function (event) {
    var popupid = $(event.currentTarget).attr("rel");
    $("#" + popupid).addClass("open");
    $("body").addClass("body-hidden");
  };

  //#endregion

  //#region function
  function initCalendar() {
    $("#calendarStartTime").kendoCalendar({
      footer: false,
      animation: false,
      selectable: "multiple",
      change: function () {
        this.selectDates([this.selectDates()[0] || new Date()]);
        vm.calendarEndTime.min(new Date(this.selectDates()[0]));
      }
    });
    vm.calendarStartTime = $("#calendarStartTime").data("kendoCalendar");
    
    $("#calendarEndTime").kendoCalendar({
      footer: false,
      animation: false,
      selectable: "multiple",
      change: function () {
        this.selectDates([this.selectDates()[0] || new Date()]);
        vm.calendarStartTime.max(new Date(this.selectDates()[0]));
      }
    });
    vm.calendarEndTime = $("#calendarEndTime").data("kendoCalendar");
  }

  function initBranches() {
    paysheetService.getBranchs().then(function (resp) {
      if (resp && resp.result) {
        vm.branches = resp.result || [];
      }
    })
    .catch(function (err) {
      vm.error = err.message;
    });
  }
  
  function initSalaryPeriod() {
    $("#loading").show();
    var sessionJson = localStorage["kvSession"]
      ? JSON.parse(localStorage["kvSession"])
      : null;
    let jwt = vm.decodeToken(sessionJson.bearerToken);
    paysheetService.getSettingByTenantId(jwt.kvrid).then(
      function (resp) {
        if (resp && resp.result) {
          vm.salaryPeriods = [];
          if (resp.result.data.IsDateOfEveryMonth === 'True' ? true : false) {
            vm.salaryPeriods.push({
              value: 1,
              name: vm._l.payRate_salaryPeriodEveryMonthly,
            });
          }
          if (resp.result.data.IsDateOfTwiceAMonth === 'True' ? true : false) {
            vm.salaryPeriods.push({
              value: 2,
              name: vm._l.payRate_salaryPeriodTwiceMonthly,
            });
          }
          if (resp.result.data.IsDayOfWeekEveryWeek === 'True' ? true : false) {
            vm.salaryPeriods.push({
              value: 3,
              name: vm._l.payRate_salaryPeriodEveryWeekly,
            });
          }
          if (resp.result.data.IsDayOfWeekTwiceWeekly === 'True' ? true : false) {
            vm.salaryPeriods.push({
              value: 4,
              name: vm._l.payRate_salaryPeriodTwiceWeekly,
            });
          }
          if (vm.salaryPeriods < 1) {
            vm.salaryPeriods.push({
              value: 1,
              name: vm._l.payRate_salaryPeriodEveryMonthly,
            });
          }
          vm.salaryPeriods.push({
            value: 5,
            name: vm._l.payRate_salaryPeriodOption,
          });
          
          vm.salaryPeriodSelected = vm.salaryPeriods[0];
          handleWorkingPeriod();
        }
      },
      function (err) {
        vm.error = err.message;
        $("#loading").hide();
      }
    )
  }  

  function timerMethod() {
    runCount++;
    if(runCount > 3) {
      clearInterval(vm.timerId);
      toastr.error(vm._l.paysheet_Error);
      vm.backToPaysheet();
    }

    paysheetService.getPaysheetById(vm.paysheetId).then(
      function (resp) {
        if (resp && resp.result) {
          if(resp.result.paysheetStatus == 1) {
            clearInterval(vm.timerId);
            $("#loading").hide();
            vm.backToPaysheet();
          }
        }
      },
      function (err) {
        vm.error = err.message;
        $("#loading").hide();
      }
    );
  }  

  function handleWorkingPeriod() {
    vm.workingPeriods = [];
    if (!vm.startTime || !vm.endTime)
        vm.startTime = vm.endTime = new Date();

    if (vm.salaryPeriodSelected.value !== 5) {
      paysheetService
        .generateWorkingPeriod(false, vm.salaryPeriodSelected.value, vm.startTime, vm.endTime)
        .then(
          function (resp) {
            if (resp && resp.result) {
              vm.workingPeriods = resp.result;
              const workingPeriods = vm.workingPeriods
                  .filter(function (item) {
                    return moment(item.endTime) < moment(new Date());
                  })
                  .sort(function (a, b) {
                    return moment(b.endTime).diff(moment(a.endTime));
                  });
              vm.workingPeriodSelected = workingPeriods[0];
              $("#loading").hide();
            }
          },
          function (err) {
            vm.error = err.message;
            $("#loading").hide();
          }
        )
    }
    else {
      $("#loading").hide();
    }
  }

  function closeModal($target) {
    $target.removeClass("open");
    $("body").removeClass("body-hidden");
  }
  
  //#endregion
}

paysheetCreateViewCmpController.$inject = [
  'tsFnFactory',
  'tsLabelFactory',
  'paysheetService',
  '$window',
];

const paysheetCreateViewCmp = {
  restrict: 'E',
  bindings: {},
  template: template,
  controller: paysheetCreateViewCmpController,
  controllerAs: 'vm',
};

export default paysheetCreateViewCmp;
