function timesheetService($http, $q) {
  return {
    showAlert: function () {
      alert('Show alert function');
    },

    getClockingMultipleBranchForCalendar: function (
      branchId,
      branchIds,
      clockingStatusExtension,
      employeeIds,
      startTime,
      endTime,
      typeCalendar
    ) {
      var defer = $q.defer();
      $http({
        method: 'GET',
        url:
          '/clockings/get-clocking-multiple-branch-for-calendar?' +
          'BranchId=' +
          branchId +
          '&' +
          'BranchIds=' +
          branchIds +
          '&' +
          'ClockingStatusExtension=' +
          clockingStatusExtension +
          '&' +
          'EmployeeIds=' +
          employeeIds +
          '&' +
          'StartTime="' +
          startTime +
          '"&' +
          'EndTime="' +
          endTime +
          '"&' +
          'TypeCalendar=' +
          typeCalendar,
      })
        .success(function (resp) {
          defer.resolve(resp);
        })
        .error(function (error) {
          defer.reject(error);
        });
      return defer.promise;
    },

    getShiftsMultipleBranchOrderbyFromTo: function (branchIds) {
      var defer = $q.defer();
      $http({
        method: 'GET',
        url: '/shifts/multiple-branch/orderby-from-to?BranchIds=' + branchIds,
      })
        .success(function (resp) {
          defer.resolve(resp);
        })
        .error(function (error) {
          defer.reject(error);
        });
      return defer.promise;
    },

    getEmployeesMultipleBranch: function (branchIds) {
      var defer = $q.defer();
      $http({
        method: 'GET',
        url: '/employees/multiple-branch?OrderBy=Name&WithDeleted=false&IdIn=&BranchIds=' + branchIds,
      })
        .success(function (resp) {
          defer.resolve(resp);
        })
        .error(function (error) {
          defer.reject(error);
        });
      return defer.promise;
    },

    getBranchs: function () {
      var defer = $q.defer();
      $http({
        method: 'GET',
        url: '/mobile/branchs',
      })
        .success(function (resp) {
          defer.resolve(resp);
        })
        .error(function (error) {
          defer.reject(error);
        });
      return defer.promise;
    },

    getClockingById: function (id) {
      var defer = $q.defer();
      $http({
        method: 'GET',
        url: '/clockings/' + id,
      })
        .success(function (resp) {
          defer.resolve(resp);
        })
        .error(function (error) {
          defer.reject(error);
        });
      return defer.promise;
    },

    getEmployeeById: function (id) {
      var defer = $q.defer();
      $http({
        method: 'GET',
        url: '/employees/' + id,
      })
        .success(function (resp) {
          defer.resolve(resp);
        })
        .error(function (error) {
          defer.reject(error);
        });
      return defer.promise;
    },

    getShiftById: function (id) {
      var defer = $q.defer();
      $http({
        method: 'GET',
        url: '/shifts/' + id,
      })
        .success(function (resp) {
          defer.resolve(resp);
        })
        .error(function (error) {
          defer.reject(error);
        });
      return defer.promise;
    },

    // getClockings: function (branchId, employeeId) {
    //     var defer = $q.defer();
    //     $http({
    //         method: "POST",
    //         url: "/mobile/get-clockings",
    //     })
    //     .success(function (resp) {
    //         defer.resolve(resp);
    //     })
    //     .error(function (error) {
    //         defer.reject(error);
    //     });
    //     return defer.promise;
    // },
  };
}

timesheetService.$inject = ['$http', '$q'];

export default timesheetService;
