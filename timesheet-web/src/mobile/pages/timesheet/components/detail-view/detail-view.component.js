import template from './detail-view.html';

function detailViewCmpController(
  $routeParams,
  tsFn,
  tsLabelFactory,
  timesheetService
) {
  //#region variables
  let vm = this;
  vm._l = tsLabelFactory;
  vm.clocking = undefined;
  vm.shift = undefined;
  vm.employee = undefined;
  //#endregion

  //#region vm
  vm.$onInit = function () {
    initEvents();
    initData();
  };

  vm.renderClockingStatusClass = function (clocking) {
    let statusClass = '';
    switch (clocking.clockingStatus) {
      case 0:
        statusClass = 'clocking-work-off';
        break;
      case 1:
        statusClass = 'clocking-created';
        break;
      case 2:
        statusClass = 'clocking-checkedIn-no-checkOut';
        break;
      case 3:
        statusClass = classCheckInCheckOut(clocking);
        break;
      case 4:
        statusClass = 'clocking-work-off';
        break;
      default:
        statusClass = 'clocking-created';
        break;
    }
    return statusClass;
  };
  //#endregion

  //#region function
  function initData() {
    if (localStorage.getItem('forceDetailRequest') == '0') {
      initDataByStorage();
    }
    else {
      initDataByRequest();
    }
  }

  function initDataByStorage() {
    const selectedClocking = JSON.parse(
      localStorage.getItem('selectedClocking') || '{}'
    );
    const shifts = JSON.parse(localStorage.getItem('shifts') || '[]');
    if (selectedClocking) {
      vm.clocking = selectedClocking;
      vm.shift = shifts.find((x) => x.id == selectedClocking.shiftId);
      vm.employee = selectedClocking.employee;
      renderViewDetail();
    }
  }

  function initDataByRequest() {
    timesheetService.getClockingById($routeParams.id).then(
      function (resp) {
        if (resp.result) {
          vm.clocking = resp.result;
          timesheetService.getShiftById(vm.clocking.shiftId).then(
            function (resp) {
              if (resp.result) {
                vm.shift = resp.result;
              }
            },
            function (err) {
              vm.error = err.message;
            }
          );
          timesheetService.getEmployeeById(vm.clocking.employeeId).then(
            function (resp) {
              if (resp.result) {
                vm.employee = resp.result;

                renderViewDetail();
              }
            },
            function (err) {
              vm.error = err.message;
            }
          );
        }
      },
      function (err) {
        vm.error = err.message;
      }
    );
  }

  function initEvents() {
    $.fn.pullToRefresh(
      '#header-container',
      vm._l.pullToRefresh,
      vm._l.releaseToRefresh,
      function () {
        if (location.hash != localStorage.getItem('oldHash')) {
          localStorage.setItem('forceDetailRequest', 1);
        } else {
          localStorage.setItem('forceDetailRequest', 0);
        }
      }
    );
  }

  function renderViewDetail() {
    let tsCalendarDetailElement = $('<div class="ts-calender-detail"></div>');

    let boxMainDetailCheckin = $(
      '<div class="box-main box-main-md  detail-checkin ' +
        vm.renderClockingStatusClass(vm.clocking) +
        '"></div>'
    );

    let shiftInfo = $(
      '<div class="shift-info">' +
        '<div class="shift-info-profle ts-staff">' +
        '<h3 class="shift-name">' +
        vm.employee.name +
        '</h3>' +
        '<p class="shift-code">' +
        vm.employee.code +
        '</p>' +
        '</div>' +
        '<div class="shift-info-avatar">' +
        `<span class="${
          (vm.employee.profilePictures && 'avatar') || 'avatar has-text'
        }">` +
        getAvatar(vm.employee) +
        '</span>' +
        '</div>' +
        '</div>'
    );
    boxMainDetailCheckin.append(shiftInfo);

    let shiftTime = renderShiftTimeAbsence(vm.clocking);
    boxMainDetailCheckin.append(shiftTime);
    tsCalendarDetailElement.append(boxMainDetailCheckin);

    let boxMainDetailInfo = $(
      '<div class="box-main box-main-md detail-info">' +
        '<h4>' +
        vm._l.workingInfo +
        '</h4>' +
        '<div class="detail-info-content">' +
        '<div class="detail-info-item">' +
        '<span class="ts-icon no-images">' +
        '<i class="far fa-calendar-day"></i>' +
        '</span>' +
        '<span>' +
        moment(vm.clocking.startTime)
          .format('dddd, DD MMMM, YYYY')
          .capitalize() +
        '</span>' +
        '</div>' +
        '<div class="detail-info-item">' +
        '<span class="ts-icon no-images">' +
        '<i class="far fa-clock"></i>' +
        '</span>' +
        '<span>' +
        '<span>' +
        vm.shift.name +
        '</span>' +
        `<span class="detail-info-time">${vm._l.from} ` +
        tsFn.toHHMM(vm.shift.from) +
        ` ${vm._l.to} ` +
        tsFn.toHHMM(vm.shift.to) +
        '</span>' +
        '</span>' +
        '</div>' +
        '</div>' +
        '</div>'
    );
    tsCalendarDetailElement.append(boxMainDetailInfo);

    let boxMainDetailNote = $(
      '<div class="box-main box-main-md detail-note">' +
        `<h4>${vm._l.note}</h4>` +
        `<p class="${(!vm.clocking.note && 'ts-no-note') || ''}">` +
        (vm.clocking.note ? vm.clocking.note : vm._l.noNote) +
        '</p>' +
        '</div>'
    );
    tsCalendarDetailElement.append(boxMainDetailNote);

    $('.ts-main').append(tsCalendarDetailElement);
    $('#loading').hide();
  }

  function renderShiftTimeAbsence(clocking) {
    if (clocking.absenceType) {
      return $(
        '<div class="shift-time">' +
          '<div class="shift-time-item time-off">' +
          '<div class="shift-time-of">' +
          `<span>${vm._l.offWork}</span>` +
          '<h3>' +
          (clocking.absenceType == 1 ? vm._l.paidLeave : vm._l.unPaidLeave) +
          '</h3>' +
          '</div>' +
          '</div>' +
          '</div>'
      );
    } else {
      return renderShiftTime(clocking);
    }
  }

  function renderShiftTime(clocking) {
    return $(
      '<div class="shift-time">' +
        '<div class="shift-time-item">' +
        '<div class="shift-time-in">' +
        `<span>${vm._l.timeIn}</span>` +
        `<h3 class="${clocking.checkInDate ? '' : 'notOutIn'}">` +
        (clocking.checkInDate
          ? moment(clocking.checkInDate).format('HH:mm')
          : vm._l.notIn) +
        '</h3>' +
        (clocking.timeIsLate > 0
          ? `<p>${vm._l.beLateToWork}: ` +
            tsFn.toWorkTime(clocking.timeIsLate) +
            ' </p>'
          : '') +
        (clocking.overTimeBeforeShiftWork > 0
          ? `<p>${vm._l.overTime}: ` +
            tsFn.toWorkTime(clocking.overTimeBeforeShiftWork) +
            ' </p>'
          : '') +
        '</div>' +
        '<div class="shift-time-icon">' +
        '<span><i class="far fa-arrow-right"></i></span>' +
        '</div>' +
        '<div class="shift-time-out ">' +
        `<span>${vm._l.timeOut}</span>` +
        `<h3 class="${clocking.checkOutDate ? '' : 'notOutIn'}">` +
        (clocking.checkOutDate
          ? moment(clocking.checkOutDate).format('HH:mm')
          : vm._l.notOut) +
        '</h3>' +
        (clocking.timeIsLeaveWorkEarly > 0
          ? `<p>${vm._l.leaveWorkEarly}: ` +
            tsFn.toWorkTime(clocking.timeIsLeaveWorkEarly) +
            ' </p>'
          : '') +
        (clocking.overTimeAfterShiftWork > 0
          ? `<p>${vm._l.overTime}: ` +
            tsFn.toWorkTime(clocking.overTimeAfterShiftWork) +
            ' </p>'
          : '') +
        '</div>' +
        '</div>' +
        '</div>'
    );
  }

  function classCheckInCheckOut(clocking) {
    if (clocking.checkInDate) {
      return 'clocking-checkedIn-checkOut';
    } else {
      return 'clocking-checkOut-no-checkIn';
    }
  }

  function getAvatar(employee) {
    if (employee.profilePictures && employee.profilePictures.length > 0) {
      let picture = employee.profilePictures.find((x) => x.isMainImage);
      return '<img src="' + picture.imageUrl + '" alt="">';
    }
    return `<span>${employee.name.charAt(0).toUpperCase()}</span>`;
  }
  //#endregion
}

detailViewCmpController.$inject = [
  '$routeParams',
  'tsFnFactory',
  'tsLabelFactory',
  'timesheetService',
];

const detailViewCmp = {
  restrict: 'E',
  bindings: {},
  template: template,
  controller: detailViewCmpController,
  controllerAs: 'vm',
};

export default detailViewCmp;
