import template from "./timesheet.html";

function timesheetCmpController($state) {
  //#region variables
  let vm = this;
  vm.firstLoad = undefined;
  vm.startDate = undefined;
  vm.endDate = undefined;
  vm.selectedDate = undefined;
  vm.branchId = undefined;
  vm.viewBy = undefined;
  vm.instance = undefined;
  vm.shiftId = undefined;
  //#endregion

  //#region vm
  vm.$onInit = function () {
  };

  vm.setDate = function (startDate, endDate, selectedDate) {
    vm.startDate = startDate;
    vm.endDate = endDate;
    vm.selectedDate = selectedDate;
    if (vm.instance && vm.instance.viewInit) {
      switch (vm.viewBy) {
        case 2:
          if ($state.current.name !== "week") $state.go("week");
          break;
        case 3:
          if ($state.current.name !== "month") $state.go("month");
          break;
        default:
          if ($state.current.name !== "date") $state.go("date");
          else vm.instance.getClockings(vm.startDate, vm.endDate);
          break;
      }
    }
  };
  
  vm.setBranchId = function (branchId) {
    vm.branchId = branchId;
  };

  vm.setShiftId = function (shiftId) {
    vm.shiftId = shiftId;
  };

  vm.setViewBy = function (viewBy) {
    vm.viewBy = viewBy;
    switch (vm.viewBy) {
      case 2:
        $state.go("week");
        break;
      case 3:
        $state.go("month");
        break;
      case 4:
        $state.go("paysheet");
        break;
      default:
        $state.go("date");
        break;
    }
  };

  //#endregion
}

timesheetCmpController.$inject = ["$state"];

const timesheetCmp = {
  restrict: "E",
  bindings: {},
  template: template,
  controller: timesheetCmpController,
  controllerAs: "vm",
};

export default timesheetCmp;
