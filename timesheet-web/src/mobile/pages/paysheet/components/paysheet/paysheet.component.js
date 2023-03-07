import toastr from "toastr";
import template from "./paysheet.html";

function paysheetCmpController(
  $scope,
  $compile,
  $location,
  tsFn,
  tsLabelFactory,
  paysheetService,
  $window
) {
  //#region variables
  let vm = this;
  vm._l = tsLabelFactory;
  vm.branchId = undefined;
  vm.branchs = [];
  vm.branchIds = [];
  vm.paysheets = [];
  vm.paysheetDelete = undefined;
  vm.paysheetStatuses = [
    {
      id: 1,
      checked: false,
      name: vm._l.paysheet_temporarySalary
    },
    {
      id: 2,
      checked: false,
      name: vm._l.paysheet_approved
    },
    {
      id: 0,
      checked: false,
      name: vm._l.paysheet_void
    },
  ];
  vm.paysheetStatusIds = [];
  vm.isNoPaysheetData = false;
  vm.salaryPeriods = [];
  vm.skip = 0;
  vm.take = 15;
  vm.calendarFilterStartDate = undefined;
  vm.calendarFilterEndDate = undefined;
  vm.filterStartDate = localStorage.getItem("filterStartDate") ? new Date(localStorage.getItem("filterStartDate")) : null;
  vm.filterEndDate = localStorage.getItem("filterEndDate") ? new Date(localStorage.getItem("filterEndDate")) : null;
  vm.paysheetId = null;
  vm.modifedDate = null;
  vm.timerId = undefined;
  var runCount = 0;
  var timer;
  //#endregion

  //#region scope
  $scope.redirectToDetail = function(id){
    const item = vm.paysheets.find((x) => x.id == id) || {};
    if (item && item.paysheetStatus > 0) {
      $location.path(`/paysheet/detail/${id}`);
    }
  };
  //#endregion

  //#region vm
  vm.$onInit = function () {
    vm.paysheetDelete = JSON.parse(localStorage.getItem("paysheetDelete"));
    if (vm.paysheetDelete) {
      $('#ts-toast-delete-payroll').show();
      timerToast(); 
      localStorage.removeItem('paysheetDelete');
    }
    $("body").removeClass("body-hidden");
    localStorage.removeItem("paysheet");
    $('#headerTimesheet').hide();
    $('#headerPaysheet').show();

    initCalendar();
    initBranches();
    initStatuses();
    initSalaryPeriod();
    initModal();

    $("#loading").show();
    vm.getPaysheets();
  };

  vm.getBranchs = function () {
    paysheetService.getBranchs().then(
      function (resp) {
        vm.branchs = resp.result || [];
        for (var i = 0; i < vm.branchIds.length; i++) {
          var item = vm.branchIds[i];
          let branchIndex = vm.branchs.findIndex((branch => branch.id == item));
          vm.branchs[branchIndex].checked = true;
        }
      },
      function (err) {
        vm.error = err.message;
      }
    );
  };  

  vm.setFilterStartDate = function () {
    vm.filterStartDate = vm.calendarFilterStartDate.selectDates()[0] || new Date();
    closeModal($(".ts-modal-payroll-calendar").closest(".ts-modal"));
  };

  vm.setFilterEndDate = function () {
    vm.filterEndDate = vm.calendarFilterEndDate.selectDates()[0] || new Date();
    closeModal($(".ts-modal-payroll-calendar").closest(".ts-modal"));
  };

  vm.selectBranchItems = function (event) {
    closeModal($(".js-ts-modal-close").closest(".ts-modal"));

    vm.branchIds = [];
    for (var i = 0; i < vm.branchs.length; i++) {
      var item = vm.branchs[i];
      if (item.checked) {
        vm.branchIds.push(parseInt(item.id, 10));
      }
    }
    localStorage.setItem("branchIds", JSON.stringify(vm.branchIds));
    resetHtml();
    vm.getPaysheets();
  };

  vm.selectStatusItems = function (event) {
    closeModal($(".js-ts-modal-close").closest(".ts-modal"));

    vm.paysheetStatusIds = [];
    for (var i = 0; i < vm.paysheetStatuses.length; i++) {
      var item = vm.paysheetStatuses[i];
      if (item.checked) {
        vm.paysheetStatusIds.push(parseInt(item.id, 10));
      }
    }
    localStorage.setItem("paysheetStatusIds", JSON.stringify(vm.paysheetStatusIds));
    localStorage.setItem("isFirstLoad", false);
    resetHtml();
    vm.getPaysheets();
  };

  vm.selectSalaryPeriod = function (event) {
    closeModal($(".js-ts-modal-close").closest(".ts-modal"));

    if(vm.salaryPeriodSelected && vm.salaryPeriodSelected.value > 0) {
      localStorage.setItem("salaryPeriodSelected", JSON.stringify(vm.salaryPeriodSelected));
      if(vm.filterStartDate) {
        localStorage.setItem("filterStartDate", vm.filterStartDate);
      }
      else {
        localStorage.removeItem("filterStartDate");
      }
      if(vm.filterEndDate) {
        localStorage.setItem("filterEndDate", vm.filterEndDate);
      }
      else {
        localStorage.removeItem("filterEndDate");
      }
    } else {
      localStorage.removeItem("salaryPeriodSelected");
      localStorage.removeItem("filterStartDate");
      localStorage.removeItem("filterEndDate");
    }
    resetHtml();
    vm.getPaysheets();
  };  

  vm.chooseSalaryPeriod = function (event, item) {
    event.stopPropagation();
    vm.salaryPeriodSelected = item;
    closeModal($(".js-ts-modal-close").closest(".ts-modal-salaryPeriod"));
  };

  vm.resetBranchs = function (event) {
    vm.branchIds = [];
    for (var i = 0; i < vm.branchs.length; i++) {
      var item = vm.branchs[i];
      item.checked = false;
    }
  };

  vm.resetStatuses = function (event) {
    vm.paysheetStatusIds = [];
    for (var i = 0; i < vm.paysheetStatuses.length; i++) {
      var item = vm.paysheetStatuses[i];
      item.checked = false;
    }
  };

  vm.resetSalaryPeriods = function (event) {
    vm.calendarFilterStartDate.selectDates([]);
    vm.calendarFilterEndDate.selectDates([]);
    vm.salaryPeriodSelected = null;
    vm.filterStartDate = null;
    vm.filterEndDate = null;
  };

  vm.backFromFilter = function (event, fromFilter) {
    if(fromFilter == "branch") {
      vm.branchs = JSON.parse(localStorage.getItem("branchs"));
      localStorage.removeItem("branchs");
    }
    else if(fromFilter == "salaryPeriodSelected") {
      vm.salaryPeriodSelected = JSON.parse(localStorage.getItem("salaryPeriodSelected"));
      vm.filterStartDate = localStorage.getItem("filterStartDate") ? new Date(localStorage.getItem("filterStartDate")) : null;
      vm.filterEndDate = localStorage.getItem("filterEndDate") ? new Date(localStorage.getItem("filterEndDate")) : null;
    }
    else if (fromFilter == "paysheetStatus") {
      vm.paysheetStatuses = JSON.parse(localStorage.getItem("paysheetStatuses"));
      localStorage.removeItem("paysheetStatuses");
    }
  };

  vm.backToFilterPeriod = function (event) {
    $("#ts-salaryPeriod-modal").removeClass("open");
    $("#ts-period-modal").removeClass("body-hidden");
  };

  vm.getPaysheets = function () {   
    $("#branchFilter").removeClass("active");
    $("#statusFilter").removeClass("active"); 
    $("#salaryPeriodFilter").removeClass("active");

    if(vm.branchIds.length > 0) {
      $("#branchFilter").addClass("active");
    }
    if(vm.paysheetStatusIds.length > 0) {
      $("#statusFilter").addClass("active");
    }
    if(vm.salaryPeriodSelected) {
      $("#salaryPeriodFilter").addClass("active");
    }

    // $("#loading").show();
    var data = getAdditionalParam({ 
      take: vm.take,
      skip: vm.skip
    });
    paysheetService.getPaysheets(data).then(
      function (resp) {
        if (resp && resp.result) {
          if (resp.result.data.length == 0) {
            if(vm.skip == 0) {
              vm.isNoPaysheetData = true;
              $("#ts-main").addClass("box-main-empty-wrap");
              $("#loading").hide();
            }
          } else {
            vm.isNoPaysheetData = false;
            $("#ts-main").removeClass("box-main-empty-wrap");
            vm.renderTsPaysheet(resp.result.data);
          }
        }
        else {
          vm.isNoPaysheetData = true;
          $("#loading").hide();
        }
      },
      function (err) {
        vm.error = err.message;
        $("#loading").hide();
      }
    );
  };

  vm.renderTsPaysheet = function (paysheets) {
    try {
      vm.tsPaySheetMain = $("#ts-staff-main");
      for (let i = 0; i < paysheets.length; i++) {
        let paysheet = paysheets[i];
        vm.paysheets.push(paysheet);
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
    let paysheetElement = $('<div class="ts-staff-wrap box-main ts-payroll-list"></div>');
    let payrollElement = $('<div class="ts-payroll-item"></div>');
    let tsHeader = $('<div class="ts-payroll-item-head"></div>');
    let tsMain = $('<div class="ts-payroll-item-main"></div>'); 
    let tsFooter = $('<div class="ts-payroll-item-footer"></div>');

    let tsEvent = $compile(
      `<a ng-click="redirectToDetail(${paysheet.id})"></a>`
    )($scope);
    let tsTimelineEvent = $(tsEvent);

    //#region header
    let headerElement = $(
    ' <div class="block-col-main"> ' +
      ' <h4>' + vm._l.paysheet + " " + paysheet.code + '</h4> ' +
      ' <span>' + paysheet.paysheetPeriodName + '</span> ' +
    ' </div> ' +
    ' <div class="block-col-button"> ' +
      ' <span class=" ' + vm.renderPaysheetStatusClass(paysheet.paysheetStatus) + '">' + vm.renderPaysheetStatusName(paysheet.paysheetStatus) + '</span> ' +
    ' </div> ');
    tsTimelineEvent.append(headerElement);    
    tsHeader.append(tsTimelineEvent);
    //#endregion

    //#region main
    let tsMainElement = $(
    ' <a> ' +
      ' <div class="ts-item-title"> ' +
        ' <span>Kỳ hạn trả</span> ' +
        ' <strong>' + vm.renderSalaryPeriodName(paysheet.salaryPeriod) + '</strong> ' +
      ' </div> ' +
        ' <div class="ts-item-total"> ' +
        ' <span>Tổng lương</span> ' +
        ' <strong> ' + vm.formatNumber(paysheet.totalNetSalary) + ' </strong> ' +
      ' </div> ' +
      ' <div class="ts-item-total-paid"> ' +
        ' <span>Đã trả nhân viên</span> ' +
        ' <strong> ' +  vm.formatNumber(paysheet.totalPayment) + ' </strong> ' +
      ' </div> ' +
    ' </a> ');

    tsMain.append(tsMainElement);
    //#endregion

    //#region footer
    let tsFooterElement = $(
      ' <div class="block-col-main"> ' +
          '  <span> ' + vm._l.paysheet_calculate_created + ' </span> ' +
          '  <strong> ' + moment(paysheet.paysheetCreatedDate).format("DD/MM/yyyy HH:mm:ss") + '</strong> ' +
        ' </div> '
      );
    tsFooter.append(tsFooterElement);

    if(paysheet.paysheetStatus == 1) {
      let tsDivLoadingElement = $(
        ' <div class="block-col-button"' +
        ' </div> '
      );

      let tsLoadingEvent = $compile(
        `<a class="reload" rel="reload-paysheet-modal" ng-click="vm.openModalLoadingPaysheet(${paysheet.id}, ${paysheet.modifedDate}, $event)"></a>`
      )($scope);
      let tsFooterEvent = $(tsLoadingEvent);

      let tsLoadingElement = $(
        ' <i class="far fa-sync"></i> ' +
        ' <span>Tải lại</span> '
      );
      tsFooterEvent.append(tsLoadingElement);
      tsDivLoadingElement.append(tsFooterEvent);
      tsFooter.append(tsDivLoadingElement);
    }
    //#endregion

    payrollElement.append(tsHeader);
    payrollElement.append(tsMain);
    payrollElement.append(tsFooter);
    paysheetElement.append(payrollElement);
    return paysheetElement;
  };

  vm.renderPaysheetStatusClass = function (paysheetStatus) {
    let statusClass = "";
    switch (paysheetStatus) {
      case 0:
        statusClass = "status-error";
        break;
      case 1:
        statusClass = "status-default";
        break;
      case 2:
        statusClass = "status-success";
        break;
      case 3:
        statusClass = "status-default";
        break;
      case 4:
        statusClass = "status-default";
        break;
      default:
        statusClass = "status-default";
        break;
    }
    return statusClass;
  };

  vm.renderPaysheetStatusName = function (paysheetStatus) {
    let statusName = "";
    switch (paysheetStatus) {
      case 0:
        statusName = vm._l.paysheet_void;
        break;
      case 1:
        statusName = vm._l.paysheet_temporarySalary;
        break;
      case 2:
        statusName = vm._l.paysheet_approved;
        break;
      case 3:
        statusName = vm._l.paysheet_draft;
        break;
      case 4:
        statusName = vm._l.paysheet_pending;
          break;
      default:
        statusName = vm._l.paysheet_temporarySalary;
        break;
    }
    return statusName;
  }; 

  vm.renderSalaryPeriodName = function (salaryPeriod) {
    let periodName = "";
    switch (salaryPeriod) {
      case 1:
        periodName = vm._l.payRate_salaryPeriodEveryMonthly;
        break;
      case 2:  
        periodName = vm._l.payRate_salaryPeriodTwiceMonthly;
        break;
      case 3:
        periodName = vm._l.payRate_salaryPeriodEveryWeekly;
        break;
      case 4:
        periodName = vm._l.payRate_salaryPeriodTwiceWeekly;
        break;
      case 5:
        periodName = vm._l.payRate_salaryPeriodOption;
          break;
      default:
        periodName = vm._l.payRate_salaryPeriodOption;
        break;
    }
    return periodName;
  };

  vm.cancelLoadingAndUpdatePaysheet = function (event) {
    vm.paysheetId = null;
    vm.modifedDate = null;
    closeModal($(".js-ts-modal-close").closest(".ts-modal"));
  }

  vm.autoLoadingAndUpdatePaysheet = function (event) {
    $("#loading").show();
    runCount = 0;
    paysheetService.autoLoadingAndUpdatePaysheet(vm.paysheetId, vm.modifedDate, vm.branchs).then(
      function (resp) {
        if (resp && resp.result) {
          vm.timerId = setInterval(timerMethod, 5000);
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

  vm.openModal = function (event) {
    var popupid = $(event.currentTarget).attr("rel");
    if(popupid == "ts-branch-modal") {
      localStorage.setItem("branchs", JSON.stringify(vm.branchs));
    } else if (popupid == "ts-period-modal") {
      if(vm.salaryPeriodSelected) {
        localStorage.setItem("salaryPeriodSelected", JSON.stringify(vm.salaryPeriodSelected));
      }
      if(vm.filterStartDate) {
        localStorage.setItem("filterStartDate", vm.filterStartDate);
      }
      if(vm.filterEndDate) {
        localStorage.setItem("filterEndDate", vm.filterEndDate);
      }
    } else if (popupid == "ts-status-modal") { 
      localStorage.setItem("paysheetStatuses", JSON.stringify(vm.paysheetStatuses));
    } 
    $("#" + popupid).addClass("open");
    $("body").addClass("body-hidden");
  };

  vm.openModalLoadingPaysheet = function (paysheetId, modifedDate, event) {
    vm.paysheetId = paysheetId;
    vm.modifedDate = modifedDate;

    $("#" + $(event.currentTarget).attr("rel")).addClass("open");
    $("body").addClass("body-hidden");
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

  //#endregion

  //#region function
  function initCalendar() {
    $("#calendarStartDate").kendoCalendar({
      footer: false,
      animation: false,
      selectable: "multiple",
      change: function () {
        this.selectDates([this.selectDates()[0] || new Date()]);
        vm.calendarFilterEndDate.min(new Date(this.selectDates()[0]));
      }
    });
    vm.calendarFilterStartDate = $("#calendarStartDate").data("kendoCalendar");

    $("#calendarEndDate").kendoCalendar({
      footer: false,
      animation: false,
      selectable: "multiple",
      change: function () {
        this.selectDates([this.selectDates()[0] || new Date()]);
        vm.calendarFilterStartDate.max(new Date(this.selectDates()[0]));
      }
    });
    vm.calendarFilterEndDate = $("#calendarEndDate").data("kendoCalendar");
  }
  
  function initBranches() {
    vm.branchIds = JSON.parse(localStorage.getItem('branchIds') || '[]');
    vm.getBranchs();
  }
  
  function initStatuses() {
    let isFirstLoad = localStorage.getItem('isFirstLoad');
    if(isFirstLoad == null) {
      vm.paysheetStatusIds = [1,2];
      setCheckedPaysheetStatuses();
    }
    else {
      vm.paysheetStatusIds = JSON.parse(localStorage.getItem('paysheetStatusIds') || '[]');
      if (vm.paysheetStatusIds && vm.paysheetStatusIds.length > 0) {
        setCheckedPaysheetStatuses();
      }
    }
  }

  function setCheckedPaysheetStatuses() {
    for (var i = 0; i < vm.paysheetStatusIds.length; i++) {
      var item = vm.paysheetStatusIds[i];
      let statusIndex = vm.paysheetStatuses.findIndex((status => status.id == item));
      vm.paysheetStatuses[statusIndex].checked = true;
    }
  }

  function initSalaryPeriod() {
    vm.salaryPeriodSelected = JSON.parse(localStorage.getItem('salaryPeriodSelected'));
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
                    
          if(vm.salaryPeriodSelected) {
            const item = vm.salaryPeriods.find((x) => x.value == vm.salaryPeriodSelected.value) || {};
            if(item == null) {
              vm.salaryPeriodSelected = null;
              localStorage.removeItem("salaryPeriodSelected");
            }
          }
        }
    })
    .catch(function (err) {
        logger.error(getErrorMsg(err));
    });
  }

  function initModal() {
    $(".ts-modal").click(function (event) {
      if ($(event.target).hasClass("ts-modal")) {
        closeModal($(this));
      }
    });

    $(".js-ts-modal-close").click(function (_) {
      closeModal($(this).closest(".ts-modal"));
    }); 

    $("#toTop").click(function () {
      $("html, body").animate({scrollTop: 0}, 1000);
    });

    $('.ts-toast-close').click(function () {
      $('.ts-toast').stop().hide(); // Close click
    });

    window.onscroll = function() {
      // var paysheet = JSON.parse(localStorage.getItem('paysheet'));
      // if(paysheet == null) {
      //   var totalPageHeight = document.body.scrollHeight; 
      //   var scrollPoint = window.scrollY + window.innerHeight;    
      //   if(scrollPoint >= totalPageHeight) {
      //     vm.skip += 15;
      //     vm.getPaysheets();
      //   }
      // }
      if ($(this).scrollTop()) {
        $('#toTop').fadeIn();
      } else {
        $('#toTop').fadeOut();
      }

      vm.skip += 15;
      vm.getPaysheets();
    };
  }

  function closeModal($target) {
    $target.removeClass("open");
    $("body").removeClass("body-hidden");
  }

  function resetHtml() {
    $("#ts-staff-main").html('');
    localStorage.removeItem("branchs");
    localStorage.removeItem("paysheetStatuses");
    vm.skip = 0;
  }
  
  function timerMethod() {
    runCount++;
    if(runCount > 3) {
      clearInterval(vm.timerId);
      toastr.error(vm._l.paysheet_Error);
      $("#loading").hide();
    }

    paysheetService.getPaysheetById(vm.paysheetId).then(
      function (resp) {
        if (resp && resp.result) {
          if(resp.result.paysheetStatus == 1) {
            clearInterval(vm.timerId);
            $("#loading").hide();
            location.reload();
          }
        }
      },
      function (err) {
        vm.error = err.message;
        $("#loading").hide();
      }
    );
  }
  
  function timerToast() {
    window.clearTimeout(timer);
    timer = setTimeout(() => {
      $('#ts-toast-delete-payroll').hide();
    }, 2000);
  }

  //#endregion

  const getAdditionalParam = function (data) {
    // default sort
    if (!data || !data.sort || data.sort.length === 0) {
        data = data || {};
        data.sort = [{ dir: 'desc', field: 'CreatedDate' }];
    }
    const param = convertKendoDsDataToQueryParams(data);
    param.BranchIds = vm.branchIds;
    if (vm.paysheetStatusIds && vm.paysheetStatusIds.length > 0) {
      param.PaysheetStatuses = JSON.stringify(vm.paysheetStatusIds);
    }
    if (vm.salaryPeriodSelected && vm.salaryPeriodSelected.value > 0) {
      param.SalaryPeriod = parseInt(vm.salaryPeriodSelected.value);
      
      if (vm.filterStartDate && vm.filterEndDate) {
        param.StartTime = vm.filterStartDate;
        param.EndTime = vm.filterEndDate;
      }
    }
    
    return param;
  };

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

paysheetCmpController.$inject = [
  "$scope",
  "$compile",
  "$location",
  "tsFnFactory",
  "tsLabelFactory",
  "paysheetService",
  "$window",
];

const paysheetCmp = {
  restrict: "E",
  bindings: {},
  template: template,
  controller: paysheetCmpController,
  controllerAs: "vm",
};

export default paysheetCmp;
