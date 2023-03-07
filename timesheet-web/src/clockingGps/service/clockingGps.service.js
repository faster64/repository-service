function clockingGpsService($http, $q) {
  return {
    showAlert: function () {
        alert("Show alert function");
    },
    findEmployee: function (os, osVersion, type, isPhone, keyWordSearch) {
        var defer = $q.defer();
        $http({
            method: "POST",
            url: "/clocking-gps/get-employee",
            data: { Os : os, OsVersion : osVersion, Type : type, IsPhone : isPhone, Keyword : keyWordSearch },
        })
        .success(function (resp) {
            defer.resolve(resp);
        })
        .error(function (error) {
            defer.reject(error);
        });
        return defer.promise;
    },

    getClockings: function (branchId, employeeId) {
        var defer = $q.defer();
        $http({
            method: "GET",
            url: "/clocking-gps/get-clockings?branchId=" + branchId + "&employeeId=" + employeeId,
        })
        .success(function (resp) {
            defer.resolve(resp);
        })
        .error(function (error) {
            defer.reject(error);
        });
        return defer.promise;
    },

    login: function (data) {
        var defer = $q.defer();
        $http({
            method: "POST",
            url: "/login-clocking-gps",
            data: data,
            headers: {
                retailer: data.TenantCode,
            },
        })
        .success(function (resp) {
        defer.resolve(resp);
        })
        .error(function (error) {
            defer.reject(error);
        });
        return defer.promise;
    },

    submitClocking: function (data) {
        var defer = $q.defer();
        var sessionJson = localStorage["kvSession"]
        ? JSON.parse(localStorage["kvSession"])
        : null;
        $http({
            method: "PUT",
            url: "/clocking-gps/update-clocking",
            data: data,
            headers: {
                tenantId: sessionJson.retailer.id,
                retailer: sessionJson.retailer.code
            },
        })
        .success(function (resp) {
            defer.resolve(resp);
        })
        .error(function (error) {
            defer.reject(error);
        });
        return defer.promise;
    },

    updateDevice: function (employeeId, verifyCode, os, osVersion, type) {
        var defer = $q.defer();
        $http({
            method: "PUT",
            url: "/clocking-gps/update-device",
            data: {EmployeeId : employeeId, VerifyCode : verifyCode, Os : os, OsVersion : osVersion, Type : type}
        })
        .success(function (resp) {
            defer.resolve(resp);
        })
        .error(function (error) {
            defer.reject(error);
        });
        return defer.promise;
    },

    getEmployeeByIdentityKey: function (identityKey) {
        var defer = $q.defer();
        $http({
            method: "GET",
            url: "/clocking-gps/get-employee-by-identity-key?identityKeyClocking=" + identityKey,
        })
        .success(function (resp) {
            defer.resolve(resp);
        })
        .error(function (error) {
            defer.reject(error);
        });
        return defer.promise;
    },

    getCurrentTimeServer: function () {
        var defer = $q.defer();
        $http({
            method: "GET",
            url: "/clocking-gps/getcurrenttimeserver",
        })
        .success(function (resp) {
            defer.resolve(resp);
        })
        .error(function (error) {
            defer.reject(error);
        });
        return defer.promise;
    },

  };
}

clockingGpsService.$inject = ["$http", "$q"];

export default clockingGpsService;
