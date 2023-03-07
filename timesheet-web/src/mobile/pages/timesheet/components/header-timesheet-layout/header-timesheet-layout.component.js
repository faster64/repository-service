import moment from "moment";
import template from "./header-timesheet-layout.html";

function headerTimeSheetLayoutCmpController(
  $route,
  tsFn,
  tsLabelFactory,
  timesheetService,
  $state
) {
  //#region variables
  let vm = this;
  vm._l = tsLabelFactory;
  vm.error = undefined;
  vm.branchId = undefined;
  vm.shiftId = undefined;
  vm.calendar = undefined;
  vm.startDate = vm.endDate = new Date();
  vm.selectedDate = localStorage.getItem("selectedDate")
    ? new Date(localStorage.getItem("selectedDate"))
    : new Date();
  vm.now = moment(vm.selectedDate).format("DD/MM/YYYY");
  vm.monthNow = moment(vm.selectedDate).format("M, YYYY");
  vm.dateName = vm._l.today;
  vm.weekOfMonth = Math.ceil(moment(vm.selectedDate).date() / 7);
  vm.viewBy = $state.current.name == "date" ? 1 : 2;
  vm.selectedView = vm.viewBy;
  vm.weekNumber = vm.viewBy == 1 ? false : true;
  //#endregion

  //#region public
  vm.$onInit = function () {
    //initSelectedDate();
    init();
    initCalendar();
    initBranches();
    initShifts();
    initModal();
    initEvents();
    vm.getShifts();
    vm.tsFn = tsFn;
  };

  vm.getBranchs = function () {
    timesheetService.getBranchs().then(
      function (resp) {
        vm.branchs = resp.result || [];
        vm.branchSelected = vm.branchs.find((x) => x.id == vm.branchId);
        vm.branchsFilter = vm.branchs.filter((x) => x.id != vm.branchId);
        vm.branchSelected && vm.branchsFilter.unshift(vm.branchSelected);
      },
      function (err) {
        vm.error = err.message;
      }
    );
  };

  vm.getShifts = function () {
    timesheetService.getShiftsMultipleBranchOrderbyFromTo(vm.branchId).then(
      function (resp) {
        vm.shifts = (resp.result || []).filter((x) => x.isActive);
        vm.shiftSelected =
          vm.shiftId > 0
            ? vm.shifts.find((x) => x.id == vm.shiftId)
            : vm.shifts[0];
        vm.shiftId = vm.shiftSelected.id;
        vm.setShiftId(vm.shiftId);
        vm.shiftsFilter = vm.shifts;
      },
      function (err) {
        vm.error = err.message;
        $("#loading").hide();
      }
    );
  };

  vm.filterBranch = function () {
    if (vm.branchFilterParam.length > 2)
      vm.branchsFilter = vm.branchs.filter((x) =>
        x.name.toLowerCase().includes(vm.branchFilterParam.toLowerCase())
      );
    else vm.branchsFilter = vm.branchs;
  };

  vm.filterShift = function () {
    if (vm.shiftFilterParam.length > 2)
      vm.shiftsFilter = vm.shifts.filter((x) =>
        x.name.toLowerCase().includes(vm.shiftFilterParam.toLowerCase())
      );
    else vm.shiftsFilter = vm.shifts;
  };

  vm.openModal = function (event) {
    $('.k-calendar-tbody tr').removeClass('week-selected');
    let currentDate = localStorage.getItem("selectedDate")
      ? new Date(localStorage.getItem("selectedDate"))
      : new Date();
    if (vm.viewBy == 1) {
      vm.calendar.selectDates([currentDate]);
    } else {
      vm.calendar.selectDates(days(currentDate));
      $('.k-state-selected').closest('tr').toggleClass('week-selected');
    }

    var popupid = $(event.currentTarget).attr("rel");
    $("#" + popupid).addClass("open");
    $("body").addClass("body-hidden");
  };

  vm.selectBranchItem = function (event, item) {
    event.stopPropagation();
    $(this).closest(".ts-modal").removeClass("open");
    $("body").removeClass("body-hidden");
    vm.branchId = item.id;
    vm.branchSelected = item;
    var sessionJson = localStorage["kvSession"]
      ? JSON.parse(localStorage["kvSession"])
      : null;
    sessionJson.branchId = item.id;
    sessionJson.shiftId = null;
    localStorage["kvSession"] = JSON.stringify(sessionJson);

    $route.reload();
  };

  vm.selectShiftItem = function (event, item) {
    event.stopPropagation();
    $(this).closest(".ts-modal").removeClass("open");
    $("body").removeClass("body-hidden");
    vm.shiftId = item.id;
    vm.shiftSelected = item;
    var sessionJson = localStorage["kvSession"]
      ? JSON.parse(localStorage["kvSession"])
      : null;
    sessionJson.shiftId = item.id;
    localStorage["kvSession"] = JSON.stringify(sessionJson);
    vm.setShiftId(item.id);
    $route.reload();
    closeModal($(document.getElementById("shift-modal")));
  };

  vm.changeDate = function () {
    let currentView = $state.current.name == "date" ? 1 : 2;
    vm.selectedDate = vm.calendar.selectDates()[0] || new Date();
    localStorage.setItem(
      "selectedDate",
      vm.calendar.selectDates()[0] || new Date()
    );
    vm.now = moment(vm.selectedDate).format("DD/MM/YYYY");
    vm.monthNow = moment(vm.selectedDate).format("M, YYYY");
    //vm.dateName = tsFn.getDayName(kendo, vm.selectedDate, 0);
    vm.weekOfMonth = Math.ceil(moment(vm.selectedDate).date() / 7);
    vm.viewBy = vm.selectedView;
    closeModal($(document.getElementById("calendar-modal")));
    // if not change view
    if (vm.selectedView == currentView) {
      $("#ts-calender").empty();
      vm.startDate =
        vm.endDate =
        vm.selectedDate =
          vm.calendar.selectDates()[0];
      vm.now = moment(vm.selectedDate).format("DD/MM/YYYY");
      localStorage.setItem("tsViewBy", vm.selectedView);
      $route.reload();
    } else {
      localStorage.setItem("tsViewBy", vm.selectedView);
      if (currentView == 1) {
        $state.go("week");
      } else {
        $state.go("date");
      }
    }
  };

  vm.viewByClick = function (mode) {
    vm.selectedView = mode;
    vm.weekNumber = vm.selectedView == 1 ? false : true;
    let currentDate = localStorage.getItem("selectedDate")
      ? new Date(localStorage.getItem("selectedDate"))
      : new Date();
    // if change view  
    if (vm.selectedView != vm.viewBy) {
      // get now
      if (vm.selectedView == 2) {
        vm.calendar.selectDates(days(new Date()));
      } else {
        vm.calendar.selectDates([new Date()]);
      }
    }
    else {
      // get selected date
      if (vm.selectedView == 2) {
        vm.calendar.selectDates(days(currentDate));
      } else {
        vm.calendar.selectDates([currentDate]);
      }
    }
    // add class selected week
    $('.k-calendar-tbody tr').removeClass('week-selected');
    if (vm.selectedView == 2) {
      $('.k-state-selected').closest('tr').toggleClass('week-selected');
    }
  };

  vm.backToMainApp = function () {
    if (isAndroid()) {
      Android.backToMainApp();
    } else {
      callIosNative("backToMainApp");
    }
  };

  //#endregion

  //#region function
  function init() {
    vm.setViewBy(vm.viewBy);
    setDateByView();
  }

  function initSelectedDate() {
    var selectedDate = localStorage.getItem("selectedDate");
    if (selectedDate) {
      vm.selectedDate = moment(selectedDate).toDate();
      vm.dateName = tsFn.getDayName(kendo, vm.selectedDate, 0);
      vm.now = moment(vm.selectedDate).format("DD/MM/YYYY");
      localStorage.removeItem("selectedDate");
    }
  }

  function initTodayHeader() {
    var day = moment(new Date()).format("d");
    var lang = localStorage.getItem("lang") || "vi-VN";
    if (lang === "vi-VN") day = day - 1;
    var el = $("#calendar tr.k-calendar-tr th.k-calendar-th").eq(day);
    if (el) el.addClass("kv-calendar-today-header");
  }

  function initEvents() {
    $.fn.pullToRefresh(
      "#header-container",
      vm._l.pullToRefresh,
      vm._l.releaseToRefresh,
      function () {
        localStorage.removeItem("selectedDate");
      }
    );
  }

  function initModal() {
    $(".ts-modal").click(function (event) {
      if ($(event.target).hasClass("ts-modal")) {
        closeModal($(this));
        revertCalendar();
      }
    });
    $(".js-modal-close").click(function (_) {
      closeModal($(this).closest(".ts-modal"));
    });
  }

  function initCalendar() {
    $("#calendar").kendoCalendar({
      footer: false,
      animation: false,
      selectable: "multiple",
      change: function () {
        $('.k-calendar-tbody tr').removeClass('week-selected');
        if (vm.selectedView == 2) {
          let daysOfWeek = days(this.selectDates()[0] || new Date());
          this.selectDates(daysOfWeek);
          $('.k-state-selected').closest('tr').toggleClass('week-selected');
        } else {
          this.selectDates([this.selectDates()[0] || new Date()]);
        }
      },
      navigate: function () {
        var current = this.current();
        if (
          new Date().getMonth() === current.getMonth() &&
          new Date().getUTCFullYear() === current.getUTCFullYear()
        )
          initTodayHeader();
          setTimeout(function(){
            $('.k-calendar-tbody tr').removeClass('week-selected');
            if (vm.selectedView == 2) {
              
              $('.k-state-selected').closest('tr').toggleClass('week-selected');
            }
          },5)
      },
    });
    vm.calendar = $("#calendar").data("kendoCalendar");
    initTodayHeader();
  }

  function initBranches() {
    var sessionJson = localStorage["kvSession"]
      ? JSON.parse(localStorage["kvSession"])
      : null;
    vm.branchId = sessionJson.branchId;
    vm.setBranchId(vm.branchId);
    vm.getBranchs();
  }

  function initShifts() {
    var sessionJson = localStorage["kvSession"]
      ? JSON.parse(localStorage["kvSession"])
      : null;

    vm.shiftId = sessionJson.shiftId;
    vm.setShiftId(vm.shiftId);
    vm.getShifts();
  }

  function revertCalendar() {
    vm.calendar.value(vm.selectedDate);
  }

  function closeModal($target) {
    $target.removeClass("open");
    $("body").removeClass("body-hidden");
  }

  function setDateByView() {
    switch (vm.viewBy) {
      case 2:
        break;
      case 3:
        break;
      default:
        var n = moment(new Date()).format("DD/MM/YYYY");
        if (n === vm.now) {
          vm.dateName = vm._l.today;
        } else {
          vm.dateName = tsFn.getDayName(kendo, vm.selectedDate, 0);
        }
        vm.startDate = vm.selectedDate;
        vm.endDate = moment(vm.startDate).add(1, "day").toDate();
        vm.weekOfMonth = Math.ceil(moment(vm.selectedDate).date() / 7);
        break;
    }
    vm.setDate(vm.startDate, vm.endDate, vm.selectedDate);
  }

  function getWeekOfMonth(date) {
    const startWeekDayIndex = 1; // 1 MonthDay 0 Sundays
    const firstDate = new Date(date.getFullYear(), date.getMonth(), 1);
    const firstDay = firstDate.getDay();

    let weekNumber = Math.ceil((date.getDate() + firstDay) / 7);
    if (startWeekDayIndex === 1) {
      if (date.getDay() === 0 && date.getDate() > 1) {
        weekNumber -= 1;
      }

      if (firstDate.getDate() === 1 && firstDay === 0 && date.getDate() > 1) {
        weekNumber += 1;
      }
    }
    return weekNumber;
  }

  function days(current) {
    var week = [];
    // Starting Monday not Sunday
    var currentWeekDay = current.getDay();
    var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1;
    var first = current.getDate() - lessDays;
    current.setDate(first);
    for (var i = 0; i < 7; i++) {
      week.push(new Date(+current));
      current.setDate(current.getDate() + 1);
    }
    return week;
  }

  function isAndroid() {
    return typeof Android != "undefined";
  }

  function callIosNative(funcName) {
    if (
      window.webkit &&
      window.webkit.messageHandlers &&
      window.webkit.messageHandlers.sendToNative
    ) {
      window.webkit.messageHandlers.sendToNative.postMessage(funcName);
    }
  }
  //#endregion
}

headerTimeSheetLayoutCmpController.$inject = [
  "$route",
  "tsFnFactory",
  "tsLabelFactory",
  "timesheetService",
  "$state",
];

const headerTimeSheetLayoutCmp = {
  restrict: "E",
  bindings: { setDate: "<", setBranchId: "<", setShiftId: "<", setViewBy: "<" },
  template: template,
  controller: headerTimeSheetLayoutCmpController,
  controllerAs: "vm",
};

export default headerTimeSheetLayoutCmp;

$(window).scroll(function () {
  var sticky = $(".ts-header"),
    scroll = $(window).scrollTop();

  if (scroll >= 100) sticky.addClass("header-fixed");
  else sticky.removeClass("header-fixed");
});
