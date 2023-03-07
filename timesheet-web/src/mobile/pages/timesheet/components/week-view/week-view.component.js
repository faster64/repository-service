import * as $ from "jquery";
import "fullcalendar";
import template from "./week-view.html";
function weekViewCmpController(
  tsLabelFactory,
  $scope,
  $compile,
  $location,
  timesheetService,
  $state
) {
  //#region variables
  let vm = this;
  vm._l = tsLabelFactory;

  vm.typeCalendar = 0;
  vm.dayNames = ["T2", "T3", "T4", "T5", "T6", "T7", "CN"];
  vm.isNoClockingData = false;
  //#endregion

  vm.$onInit = function () {
    //vm.initTimeSheetCalendar();
    setShift();
    var input = localStorage.getItem("selectedDate");
    vm.daysOfWeek = days(input ? new Date(input) : new Date());
    vm.getEmployees();
    $('#headerTimesheet').show();
    $('#headerPaysheet').hide();
  };

  //#region scope
  $scope.redirectToDetail = function (id) {
    const item = vm.clockingList.find((x) => x.id == id) || {};
    if (item && item.clocking) {
      localStorage.setItem("selectedClocking", JSON.stringify(item.clocking));
      localStorage.setItem("selectedDate", vm.selectedDate);
      $location.path(`/timesheet/detail/${id}`);
    } else {
      toastr.error(vm._l.noData);
    }
  };
  //#endregion

  vm.getEmployees = function () {
    timesheetService.getEmployeesMultipleBranch(vm.branchId).then(
      function (resp) {
        if (resp.result?.data) {
          vm.employees = resp.result.data || [];
          vm.getClockings(
            moment(vm.daysOfWeek[0]).format("YYYY-MM-DD"),
            moment(vm.daysOfWeek[6]).add(1, "days").format("YYYY-MM-DD")
          );
        } else {
          $("#loading").hide();
        }
      },
      function (err) {
        vm.error = err.message;
        $("#loading").hide();
      }
    );
  };

  vm.getClockings = function (start, end) {
    let clockingStatusExtension = "1,4,2,3,5";
    let employeeIds = "";
    let startTime = moment(start).format("YYYY-MM-DD");
    let endTime = moment(end).format("YYYY-MM-DD");

    timesheetService
      .getClockingMultipleBranchForCalendar(
        vm.branchId,
        vm.branchId,
        clockingStatusExtension,
        employeeIds,
        startTime,
        endTime,
        vm.typeCalendar
      )
      .then(
        function (resp) {
          if (resp.result?.data) {
            let dataFilter = resp.result.data.filter(function (item) {
              return item.shiftId == vm.shiftId;
            });
            let result = groupBy(dataFilter, (item) => item.employeeId);
            vm.clockings = dataFilter || [];
            vm.clockingList = resp.result.data || [];
            if (vm.clockings.length > 0) {
              vm.isNoClockingData = false;
              renderHeaderCalendar();
              renderBodyCalendar();
            } else {
              vm.isNoClockingData = true;
              $("#loading").hide();
            }
            
          } else {
            vm.isNoClockingData = true;
            $("#loading").hide();
          }
        },
        function (err) {
          vm.error = err.message;
          $("#loading").hide();
        }
      );
  };

  function groupBy(list, keyGetter) {
    const map = new Map();
    list.forEach((item) => {
      const key = keyGetter(item);
      if (!map.has(key)) {
        map.set(key, [item]);
      } else {
        map.get(key).push(item);
      }
    });
    return map;
  }

  function renderHeaderCalendar() {
    let date = new Date();
    let headerRow = $(".ts-calender-row");
    headerRow.append($('<div class="ts-col-item shift-name"></div>'));

    for (let i = 0; i < vm.daysOfWeek.length; i++) {
      let item = $(
        '<div class="ts-col-item ' +
          (vm.daysOfWeek[i].getDate() == date.getDate() ? "ts-today" : "") +
          '">' +
          '<div class="col-time">' +
          '<span class="ts-day">' +
          vm.dayNames[i] +
          "</span>" +
          '<span class="ts-date">' +
          vm.daysOfWeek[i].getDate() +
          "</span>" +
          "</div>" +
          "</div>"
      );
      headerRow.append(item);
    }
  }

  function renderBodyCalendar() {
    let contentCalendar = $(".ts-calender-content");
    let employeeIds = vm.clockings.filter((element, index) => {
      return vm.clockings.indexOf(element) === index;
    }).map(x => x.employeeId);
    let filterdEmployees = vm.employees.filter(x => employeeIds.includes(x.id));
    for (let i = 0; i < filterdEmployees.length; i++) {
      let empClockings = vm.clockings.filter(
        (x) => x.employeeId == filterdEmployees[i].id
      );
      let contentRow = $('<div class="ts-calender-row"></div>');
      let employeeName = filterdEmployees[i]?.name || '';
      let isLongName = employeeName.trim().indexOf(' ') <= 0;
      let textTruncateClass = isLongName ? 'text-truncate' : '';
      let employeeItem = $(
        '<div class="ts-col-item shift-name">' +
          '<h6 class=' + '"' + textTruncateClass + '"' + '>' +
          employeeName +
          '</h6>' +
          '</div>'
      );
      contentRow.append(employeeItem);
      for (let i = 0; i < vm.daysOfWeek.length; i++) {
        let clockingToday = empClockings.find(
          (x) => new Date(x.start).getDate() == vm.daysOfWeek[i].getDate()
        );
        if (clockingToday) {
          let tsEvent = $compile(
            `<a class="col-event " ng-click="redirectToDetail(${clockingToday.id})"><span></span></a>`
          )($scope);
          let tsTimelineEvent = $(tsEvent);
          tsTimelineEvent.addClass(
            renderClockingStatusClass(clockingToday.clocking)
          );
          let clockingItem = $('<div class="ts-col-item"></div>');
          clockingItem.append(tsTimelineEvent);
          contentRow.append(clockingItem);
        } else {
          let clockingItemEmpty = $('<div class="ts-col-item"></div>');
          contentRow.append(clockingItemEmpty);
        }
      }
      contentCalendar.append(contentRow);
    }
    $("#loading").hide();
  }

  function renderClockingStatusClass(clocking) {
    let statusClass = "";
    let futureClass = "event-future";
    let dateNow = moment();
    if (moment(clocking.startTime) > dateNow && clocking.clockingStatus == 1) {
      return futureClass;
    }
    switch (clocking.clockingStatus) {
      case 0:
        statusClass = "clocking-work-off";
        break;
      case 1:
        statusClass = "clocking-created";
        break;
      case 2:
        statusClass = "clocking-checkedIn-no-checkOut";
        break;
      case 3:
        statusClass = classCheckInCheckOut(clocking);
        break;
      case 4:
        statusClass = "clocking-work-off";
        break;
      default:
        statusClass = "clocking-created";
        break;
    }
    return statusClass;
  }

  function classCheckInCheckOut(clocking) {
    if (clocking.checkInDate) {
      return "clocking-checkedIn-checkOut";
    } else {
      return "clocking-checkOut-no-checkIn";
    }
  }

  function setShift() {
    var sessionJson = localStorage["kvSession"]
      ? JSON.parse(localStorage["kvSession"])
      : null;

    vm.shiftId = sessionJson.shiftId;
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
}

weekViewCmpController.$inject = [
  "tsLabelFactory",
  "$scope",
  "$compile",
  "$location",
  "timesheetService",
  "$state",
];

const weekViewCmp = {
  restrict: "E",
  bindings: {
    startDate: "=",
    endDate: "=",
    selectedDate: "=",
    branchId: "=",
    shiftId: "=",
    instance: "=",
  },
  template: template,
  controller: weekViewCmpController,
  controllerAs: "vm",
};

export default weekViewCmp;
