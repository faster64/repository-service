import template from "./clocking.html";
import moment from 'moment';
import $ from "jquery";

function clockingCmpController(clockingGpsService, $rootScope, $location, gpsInfoService, $timeout) {
    let vm = this;
    vm.message = "Clocking page";
    vm.params = $location.search();
    vm.clockings = [0];
    vm.isCheckIn = true;
    vm.isHistoryMode = false;
    vm.milliseconds = 0;
    $rootScope.successMessage = '';

    vm.$onInit = function () {
        $('#loading').hide();
        if (!$rootScope.currentEmployee) {
            $location.path("init").search(vm.params);
        }
        moment.locale('vi');
        let now = moment();
        vm.currentDate = now.format('dddd, DD/MM/YYYY');

        vm.employee = localStorage["kvSessionEmployee"] ? JSON.parse(localStorage["kvSessionEmployee"]) : null;
        if (!vm.employee) {
            $location.path("find-employee");
        }

        if ($rootScope.identityKeyClocking) {
            vm.identityKeyClocking = $rootScope.identityKeyClocking;
        } else {
            vm.identityKeyClocking = vm.employee.identityKeyClocking;
        }

        vm.getClockings(vm.employee.id);
        vm.getCurrentTimeServer();
        vm.clockTimer(vm.milliseconds);
    };

    vm.getClockings = function (employeeId) {
        var sessionJson = localStorage["kvSession"]
          ? JSON.parse(localStorage["kvSession"])
          : null;
        clockingGpsService.getClockings(sessionJson.branchId, employeeId).then(
          function (resp) {
            if (resp && resp.result) {
              console.log(resp.result);
              vm.clockings.length = 0;
              resp.result.clockingsDto.forEach((element) => {
                var now = moment();
                var shift = element.shift;
                var timeStart = moment(element.startTime);
                var timeEnd = moment(element.endTime);

                var minutesInDay = 0;
                var dayCheckInBeforeShift = 0;
    
                if (
                  timeEnd.date() == timeStart.date() &&
                  shift.checkOutAfter < shift.checkInBefore &&
                  timeEnd.minutes() + timeEnd.hours() * 60 > shift.checkOutAfter
                ) {
                  minutesInDay = 1440; //24 * 60;
                }
    
                if (
                  shift.checkOutAfter < shift.checkInBefore &&
                  timeStart.minute() + timeStart.hour() * 60 < shift.checkInBefore
                ) {
                  dayCheckInBeforeShift = -1440;
                }
    
                // check ca: ca thuong + ca qua dem cham cong vao
                if (
                  // (timeStart.toDate() >= now.clone().startOf("day") ||
                  //   (timeStart.toDate() < now.clone().startOf("day") &&
                  //     element.isCheckIn == true)) &&
                  now >=
                    timeStart
                      .clone()
                      .add((shift.from > shift.checkInBefore? -(shift.from - shift.checkInBefore): -((shift.from + 1440) - shift.checkInBefore)), "minutes")
                      .toDate() &&
                  now <=
                    timeEnd
                      .clone()
                      .add((shift.checkOutAfter > shift.to? shift.checkOutAfter - shift.to : (shift.checkOutAfter + 1440) - shift.to), "minutes")
                      .toDate()
                ) {
                  vm.clockings.push(element);
                }
              });
              //vm.clockings = resp.result.clockingsDto;
              vm.clockings.sort(function (a, b) {
                if (a.startTime < b.startTime) {
                  return -1;
                }
                if (a.startTime > b.startTime) {
                  return 1;
                }
                return 0;
              });
              vm.branchName = resp.result.branchName;
              vm.confirmClockingDto = resp.result.confirmClockingDto;
              vm.checkShowHistory(vm.clockings);
            }
          },
          function (err) {
            console.log(err);
            vm.error = err.message;
          }
        );
      };

    vm.getCurrentTimeServer = function() {
        clockingGpsService.getCurrentTimeServer()
            .then(function(resp){
                vm.startAt = new Date(resp);
            });
    };

    vm.getCheckInDate = function() {
        for (let i = 0; i < vm.confirmClockingDto.length; ++i) {
            let extra = vm.confirmClockingDto[i].extra ? JSON.parse(vm.confirmClockingDto[i].extra) : null;
            if (extra && (extra.clocking.id == vm.clocking.id)) {
                return extra.clocking.checkInDate;
            } else {
               return null;
            }
        }
    };
    
    vm.submitClocking = function (acceptWrongGps) {
        $('#loading').show();
        let hasSetCheckInDate = false;
        if (vm.clocking) {
            var sessionJson = localStorage["kvSession"]
            ? JSON.parse(localStorage["kvSession"])
            : null;
            let clocking = vm.clocking;
            let now = new Date();

            if (vm.isCheckIn) {
                clocking.checkInDate = now.toISOString();
                hasSetCheckInDate = true;
            } else {
                if (!clocking.checkInDate) {
                    clocking.checkInDate = vm.getCheckInDate();
                    hasSetCheckInDate = true;
                }

                clocking.checkOutDate = now.toISOString();
            }

            console.log(now.toISOString());
            if (!clocking || clocking.id <= 0)
                vm.error = { message : "Vui lòng chọn ca làm việc." };
            else {
                let clockingHistory = {
                    "id": 0,
                    "clockingId": clocking.id,
                    "checkedInDate": vm.isCheckIn ? now.toISOString() : clocking.checkInDate,
                    "currentCheckInDate": now.getDate()  + "/" + (now.getMonth()+1) + "/" + now.getFullYear(),
                    "checkedOutDate": vm.isCheckIn ? null : now.toISOString(),
                    "currentCheckOutDate": now.getDate()  + "/" + (now.getMonth()+1) + "/" + now.getFullYear(),
                    "branchId": sessionJson.branchId,
                    "timeIsLate": 0,
                    "overTimeBeforeShiftWork": 0,
                    "timeIsLeaveWorkEarly": 0,
                    "overTimeAfterShiftWork": 0,
                    "clockingStatus": 1,
                    "timeKeepingType": 1,
                    "absenceType": null,
                    "checkInDateType": 2,
                    "checkOutDateType": 2,
                    "checkTime": now.toISOString()
                };

                let inputData = {
                    Clocking : clocking,
                    ClockingHistory : clockingHistory,
                    identityKeyClocking: vm.identityKeyClocking,
                    geoCoordinate : $rootScope.geoCoordinate,
                    acceptWrongGps: acceptWrongGps
                };

                console.log(inputData);
                clockingGpsService.submitClocking(inputData).then(
                    function (resp) {
                        $rootScope.isCheckIn = vm.isCheckIn;
                        if(resp.result == null){
                            $rootScope.successMessage = resp.message;
                            if(acceptWrongGps) {
                                $rootScope.acceptWrongGps = acceptWrongGps;
                            }
                        }
                        $('#loading').hide();
                        console.log(resp);
                        $location.path("success").search(vm.params);
                    },
                    function (err) {
                        $('#loading').hide();
                        console.log(err);
                        vm.error = err.errors[0];
                        if (angular.isDefined(vm.error.code) && vm.error.code == "wrong_gps") {
                            gpsInfoService.setGeoCoordinate();
                        }
                        if (hasSetCheckInDate) clocking.checkInDate = null;
                    }
                );
            }
        } else {
            vm.error = { message : "Vui lòng chọn ca làm việc." };
            $('#loading').hide();
        }
    };

    vm.toHHMM = function (minutes) {
        var min_num = parseInt(minutes, 10);
        var hours   = Math.floor(min_num / 60);
        var minutes = Math.floor(min_num % 60);
    
        return [hours,minutes]
            .map(v => v < 10 ? "0" + v : v)
            .join(":")
    };

    vm.dateToHHMM = function (date) {
        if (date) {
            return moment(date).format("hh:mm");
        }
        return "--:--"
    };

    vm.clockTimer = function (ms) {
        vm.milliseconds += ms;
        $timeout(function() {
            var time = moment(vm.startAt);
            time.add(vm.milliseconds, 'ms');

            var h = time.hours();
            var m = time.minutes();
            var s = time.seconds();
            if (m < 10) {
                m = "0" + m;
            }
            if (h < 10) {
                h = "0" + h;
            }
            if (s < 10) {
                s = "0" + s;
            }
            $(".cloking-main .time-clock").text(h + " : " + m);
            vm.clockTimer(1000);
        },1000);
    };

    vm.setIsCheckIn = function(clocking) {
        // Hiển thị chấm công ra khi:
        // Đã chấm công vào
        // hoặc thời gian hiện tại đang nằm trong khoảng thời gian trước giờ ra ca 30p
        // hoặc đã có cltv chờ xác nhận chấm công
        let now = moment();
        if (clocking.checkInDate || (now >= moment(clocking.endTime).add(-30, 'minutes'))) {
            vm.isCheckIn = false;
        } else if (vm.confirmClockingDto && vm.confirmClockingDto.length > 0) {
            vm.isCheckIn = true;
            vm.confirmClockingDto.forEach(c => {
                let extra = c.extra ? JSON.parse(c.extra) : null;
                if (extra && (extra.clocking.id == clocking.id)) {
                    vm.isCheckIn = false;
                    return;
                }
            });
        } else vm.isCheckIn = true;
    };

    vm.checkClockingTime = function (clocking) {
        if (clocking) {
          var now = moment();
          var shift = clocking.shift;
          var timeStart = moment(clocking.startTime);
          var timeEnd = moment(clocking.endTime);
    
          var minutesInDay = 0;
          var dayCheckInBeforeShift = 0;
          var flag = false;
    
          if (
            timeEnd.date() == timeStart.date() &&
            shift.checkOutAfter < shift.checkInBefore &&
            timeEnd.minutes() + timeEnd.hours() * 60 > shift.checkOutAfter
          ) {
            minutesInDay = 1440; //24 * 60;
          }
    
          if (
            shift.checkOutAfter < shift.checkInBefore &&
            timeStart.minute() + timeStart.hour() * 60 < shift.checkInBefore
          ) {
            dayCheckInBeforeShift = -1440;
          }
    
          // check ca: ca thuong + ca qua dem cham cong vao
          if (        
            now >=
            timeStart
              .clone()
              .add((shift.from > shift.checkInBefore? -(shift.from - shift.checkInBefore): -((shift.from + 1440) - shift.checkInBefore)), "minutes")
              .toDate() &&
            now <=
            timeEnd
              .clone()
              .add((shift.checkOutAfter > shift.to? shift.checkOutAfter - shift.to : (shift.checkOutAfter + 1440) - shift.to), "minutes")
              .toDate()
          ) {
            flag = true;
          }
          
          return flag;
        }
      };

      vm.onChangeShift = function (clocking) {
        vm.error = null;
        if (vm.isHistoryMode) {
          $(".history-mode .cloking-shift .ts-dropdowm-result").text(
            clocking.shift.name +
              " : " +
              vm.toHHMM(clocking.shift.from) +
              " - " +
              vm.toHHMM(clocking.shift.to)
          );
          $("#his-clocking-in").text(
            "Vào: " + clocking.checkInDate
              ? moment(clocking.checkInDate).format("hh:mm")
              : ""
          );
          $("#his-clocking-out").text(
            "Ra: " + clocking.checkOutDate
              ? moment(clocking.checkOutDate).format("hh:mm")
              : ""
          );
        } else {
          if (vm.checkClockingTime(clocking)) {
            vm.clocking = clocking;
            vm.setIsCheckIn(clocking);
            $(".clocking-mode .cloking-shift .ts-dropdowm-result").text(
              clocking.shift.name +
                " : " +
                vm.toHHMM(clocking.shift.from) +
                " - " +
                vm.toHHMM(clocking.shift.to)
            );
            if (vm.isCheckIn) {
              $(".main-content .page-cloking-wrap").removeClass("cloking-in");
              $(".main-content .page-cloking-wrap").removeClass("cloking-out");
              $(".main-content .page-cloking-wrap").addClass("cloking-in");
            } else {
              $(".main-content .page-cloking-wrap").removeClass("cloking-in");
              $(".main-content .page-cloking-wrap").removeClass("cloking-out");
              $(".main-content .page-cloking-wrap").addClass("cloking-out");
            }
          }
        }
      };

    vm.openAndCloseDropdown = function(event) {
        event.stopPropagation();
        $("#shift-dropdown").show();
    };

    vm.checkShiftActive = function (clocking) {
        if (clocking) {
          var now = moment();
          var shift = clocking.shift;
          var timeBefore = moment(clocking.startTime);
          var timeAfter = moment(clocking.endTime);
    
          var startTime = moment(clocking.startTime);
          var endTime = moment(clocking.endTime);
    
          var minutesInDay = 0;
          var dayCheckInBeforeShift = 0;
          if (
            timeAfter.date() == timeBefore.date() &&
            shift.checkOutAfter < shift.checkInBefore &&
            timeAfter.minutes() + timeAfter.hours() * 60 > shift.checkOutAfter
          ) {
            minutesInDay = 1440; //24 * 60;
          }
    
          if (
            shift.checkOutAfter < shift.checkInBefore &&
            timeBefore.minute() + timeBefore.hour() * 60 < shift.checkInBefore
          ) {
            dayCheckInBeforeShift = -1440;
          }
    
          if (now > startTime.toDate() && now < endTime.toDate()) {
            return "now";
          } else if (
            now >=
            timeStart
              .clone()
              .add((shift.from > shift.checkInBefore? -(shift.from - shift.checkInBefore): -((shift.from + 1440) - shift.checkInBefore)), "minutes")
              .toDate() &&
            now <=
            timeEnd
              .clone()
              .add((shift.checkOutAfter > shift.to? shift.checkOutAfter - shift.to : (shift.checkOutAfter + 1440) - shift.to), "minutes")
              .toDate()
          ) {
            return "";
          } else {
            return "time-before";
          }
        }
      };

    vm.checkShowHistory = function(clockings) {
        let now  = moment();
        let minutes = now.hour() * 60 + now.minutes();
        let isHistory = false;
        let shiftTo = 0;
        if (clockings.length > 0) {
            clockings.forEach(c => {
                if (minutes >= c.shift.from && minutes <= c.shift.to){
                    vm.onChangeShift(c);
                }
                if(c.shift.to > shiftTo)
                    shiftTo = c.shift.to
            });

            // Comment code show history
            // if (minutes > shiftTo) {
            //     vm.isHistoryMode = true;
            //     if(clockings.length > 0) {
            //         setTimeout(() => {
            //             vm.onChangeShift(clockings[0]);
            //         }, 100);
            //     }
            // }
        } else {
            vm.error = { message : "Hiện tại không có ca làm việc để chấm công." };
        }
    };
}

$(window).click(function() {
    $("#shift-dropdown").hide();
});

clockingCmpController.$inject = [
    "clockingGpsService",
    "$rootScope",
    "$location",
    "gpsInfoService",
    "$timeout"
];

const clockingCmp = {
    restrict: "E",
    bindings: {},
    template: template,
    controller: clockingCmpController,
    controllerAs: "vm",
};

export default clockingCmp;
