import toastr from "toastr";
import template from "./day-view.html";

function dayViewCmpController(
  $scope,
  $compile,
  $location,
  tsFn,
  tsLabelFactory,
  timesheetService,
  $state
) {
  //#region variables
  let vm = this;
  vm._l = tsLabelFactory;
  vm.viewInit = undefined;
  vm.typeCalendar = 2;
  vm.clockingList = [];
  vm.isNoClockingData = false;
  //#endregion

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

  //#region vm
  vm.$onInit = function () {
    vm.instance = vm;
    vm.viewInit = true;
    vm.getShifts();
    $('#headerTimesheet').show();
    $('#headerPaysheet').hide();
  };

  vm.getShifts = function () {
    timesheetService.getShiftsMultipleBranchOrderbyFromTo(vm.branchId).then(
      function (resp) {
        vm.shifts = (resp.result || []).filter((x) => x.isActive);
        localStorage.setItem("shifts", JSON.stringify(vm.shifts));
        let selectedDate = localStorage.getItem("selectedDate");
        let startDate = selectedDate ? new Date(selectedDate) : new Date();
        let endDate = new Date(startDate).setDate(startDate.getDate() + 1);
        let weekDays = days(selectedDate ? new Date(selectedDate) : new Date());
        vm.getClockings(
          moment(startDate).format("YYYY-MM-DD"),
          moment(endDate).format("YYYY-MM-DD")
        );
      },
      function (err) {
        vm.error = err.message;
        $("#loading").hide();
      }
    );
  };

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
            let result = groupBy(resp.result.data, (item) => item.resourceId);
            vm.clockings = result || new Map();
            vm.clockingList = resp.result.data || [];
            vm.isNoClockingData = false;
            vm.renderTsShift();
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

  vm.renderTsShift = function () {
    try {
      vm.tsCalenderTimeline = $("#ts-calender");
      var ids = Array.from(vm.clockings.keys());
      vm.shifts
        .filter((x) => !ids.includes(x.id.toString()))
        .map((x) => x.id)
        .forEach((x) => vm.clockings.set(x.toString(), []));
      for (const [key, val] of vm.clockings) {
        let tsShift = $('<div class="ts-shift"></div>');
        let tsShiftHeader = $('<div class="ts-shift-header"></div>');
        let shift = vm.shifts.find((x) => x.id == key) || {};
        let shiftElement = $(
          '<div class="ts-shift-header-left"> <h4 class="ts-shift-title">' +
            shift.name +
            "</h4></div>" +
            '<div class="ts-shift-header-right">' +
            '<div class="ts-shift-time">' +
            '<span class="ts-icon-sm"><i class="far fa-clock"></i></span>' +
            "<span>" +
            tsFn.toHHMM(shift.from) +
            " - " +
            tsFn.toHHMM(shift.to) +
            "</span>" +
            "</div>" +
            "</div>"
        );
        tsShiftHeader.append(shiftElement);
        tsShift.append(tsShiftHeader);

        let tsShiftList = $('<ul class="ts-shift-list"></ul>');
        val.forEach(function (c) {
          let item = vm.renderClockingItem(c);
          tsShiftList.append(item);
        });
        tsShift.append(tsShiftList);
        vm.tsCalenderTimeline.append(tsShift);
        $("#loading").hide();
      }
    } catch (error) {
      console.log(error);
      $("#loading").hide();
    }
  };

  vm.renderClockingItem = function (clocking) {
    let dateNow = moment();
    let futureClass = "event-future";
    let clockingElement = $('<li class="ts-shift-item"></li>');

    let tsEvent = $compile(
      `<a class="ts-timeline-event" ng-click="redirectToDetail(${clocking.id})"></a>`
    )($scope);
    let tsTimelineEvent = $(tsEvent);
    if (
      moment(clocking.clocking.startTime) > dateNow &&
      clocking.clocking.clockingStatus == 1
    ) {
      tsTimelineEvent.addClass(futureClass);
    } else {
      tsTimelineEvent.addClass(vm.renderClockingStatusClass(clocking.clocking));
    }

    let tsTimelineEventTitle = $(
      '<h4 class="name-title">' + clocking.title + "</h4>"
    );
    tsTimelineEvent.append(tsTimelineEventTitle);

    let tsTimelineContainer = vm.renderTimelineContainerTimer(
      clocking.clocking
    );
    tsTimelineEvent.append(tsTimelineContainer);

    clockingElement.append(tsTimelineEvent);
    return clockingElement;
  };

  vm.renderTimelineContainerTimer = function (clocking) {
    let container = $(
      '<span class="timeline-container--timer time-check-in-check-out-weekends"></span>'
    );
    if (clocking.clockingStatus == 4) {
      var workOff = $("<span>" + vm._l.workOff + "</span>");
      container.append(workOff);
    } else {
      if (clocking.checkInDate) {
        var timeCheckIn = $(
          '<span class="time-check-in' +
            (moment(clocking.checkInDate).diff(
              moment(clocking.startTime),
              "seconds"
            ) != 0
              ? "-red"
              : "") +
            '">' +
            moment(clocking.checkInDate).format("HH:mm") +
            "</span>"
        );
        container.append(timeCheckIn);
      } else {
        container.append($('<span class="time-check-in">--</span>'));
      }

      if (clocking.checkInDate && clocking.checkOutDate) {
        container.append($("<span> - </span>"));
      } else {
        container.append($("<span> </span>"));
      }

      if (clocking.checkOutDate) {
        var timeCheckOut = $(
          '<span class="time-check-out' +
            (moment(clocking.checkOutDate).diff(
              moment(clocking.endTime),
              "seconds"
            ) != 0
              ? "-red"
              : "") +
            '">' +
            moment(clocking.checkOutDate).format("HH:mm") +
            "</span>"
        );
        container.append(timeCheckOut);

        let workedTime = "";
        if (clocking.checkInDate) {
          workedTime = tsFn.toWorkTime(
            moment(clocking.checkOutDate).diff(
              moment(clocking.checkInDate),
              "minutes"
            )
          );
        } else {
          workedTime = tsFn.toWorkTime(
            moment(clocking.checkOutDate).diff(
              moment(clocking.startTime),
              "minutes"
            )
          );
        }
        if (workedTime) {
          container.append(
            $('<span class="worked-time">' + workedTime + "</span>")
          );
        }
      } else {
        container.append($('<span class="time-check-out">--</span>'));
      }
    }

    let footArr = [];
    if (clocking.timeIsLate) {
      footArr.push(
        vm._l.beLateToWork + " " + tsFn.toWorkTime(clocking.timeIsLate)
      );
    }
    if (clocking.overTimeBeforeShiftWork > 0) {
      footArr.push(
        vm._l.OTBeforeShiftWork +
          " " +
          tsFn.toWorkTime(clocking.overTimeBeforeShiftWork)
      );
    }
    if (clocking.timeIsLeaveWorkEarly) {
      footArr.push(
        vm._l.leaveWorkEarly +
          " " +
          tsFn.toWorkTime(clocking.timeIsLeaveWorkEarly)
      );
    }
    if (clocking.overTimeAfterShiftWork > 0) {
      footArr.push(
        vm._l.OTAfterShiftWork +
          " " +
          tsFn.toWorkTime(clocking.overTimeAfterShiftWork)
      );
    }
    if (clocking.clockingPenalizesDto.length > 0) {
      clocking.clockingPenalizesDto.forEach((item) => {
        let penalize = item.penalizeDto.name + " " + item.timesCount;
        footArr.push(penalize);
      });
    }
    if (footArr) {
      container.append(
        $('<span class="footer">' + footArr.join(", ") + "</span>")
      );
    }

    return container;
  };

  vm.renderClockingStatusClass = function (clocking) {
    let statusClass = "";
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
  };
  //#endregion

  //#region function
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

  function classCheckInCheckOut(clocking) {
    if (clocking.checkInDate) {
      return "clocking-checkedIn-checkOut";
    } else {
      return "clocking-checkOut-no-checkIn";
    }
  }
  //#endregion
}

dayViewCmpController.$inject = [
  "$scope",
  "$compile",
  "$location",
  "tsFnFactory",
  "tsLabelFactory",
  "timesheetService",
  "$state",
];

const dayViewCmp = {
  restrict: "E",
  bindings: {
    startDate: "=",
    endDate: "=",
    selectedDate: "=",
    branchId: "=",
    instance: "=",
  },
  template: template,
  controller: dayViewCmpController,
  controllerAs: "vm",
};

export default dayViewCmp;
