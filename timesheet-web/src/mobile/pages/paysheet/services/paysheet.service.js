function paysheetService($http, $q) {
  return {
    showAlert: function () {
      alert('Show alert function');
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

    getPaysheets: function (data) {
      var defer = $q.defer();
      $http({
        method: 'GET',
        url: "/paysheets",
        params: data,
      })
        .success(function (resp) {
          defer.resolve(resp);
        })
        .error(function (error) {
          defer.reject(error);
        });
      return defer.promise;
    },
    
    getPaysheetById: function (id) {
      var defer = $q.defer();
      $http({
        method: 'GET',
        url: '/paysheets/' + id,
      })
        .success(function (resp) {
          defer.resolve(resp);
        })
        .error(function (error) {
          defer.reject(error);
        });
      return defer.promise;
    }, 

    getPayslipsByPaysheetId: function (data) {
      var defer = $q.defer();
      $http({
        method: 'GET',
        url: '/payslip/getPayslipsByPaysheetId',
        params: data,
      })
        .success(function (resp) {
          defer.resolve(resp);
        })
        .error(function (error) {
          defer.reject(error);
        });
      return defer.promise;
    },

    /**
     * Kiểm tra bảng lương có bị thay đổi không nếu có sẽ thông báo cho người dùng
     * @param {any} id paysheet id
     * @param {any} branchId branchId
     * @param {number} kvSessionBranchId kvSessionBranchId
     * @return {Promise} promise
     */
    getAndCheckChangeById: function(id, branchId, kvSessionBranchId) {
      const defer = $q.defer();

      $http
        .get('/paysheets/get-and-check-change-by-id/' + id, {
            params: {
              BranchId: branchId,
              KvSessionBranchId: kvSessionBranchId,
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

    cancelPaysheet: function (id, isCheckPayslipPayment, isCancelPayment) {
      var defer = $q.defer();
      $http
        .put('/paysheets/cancel-paysheet', {
            Id: id,
            isCheckPayslipPayment: isCheckPayslipPayment,
            isCancelPayment: isCancelPayment,
        })
        .success(function (resp) {
          defer.resolve(resp);
        })
        .error(function (error) {
          defer.reject(error);
        });
      return defer.promise;
    },

    /**
      * Thực hiện tạo lại bảng lương
      * @param {any} paysheetId Mã bảng lương
      * @param {any} modifiedDate Ngày cập nhật gần nhất của bảng lương
      * @return {Promise} promise
    */
    autoLoadingAndUpdatePaysheet: function(paysheetId, modifiedDate, branchs) {
      const defer = $q.defer();

      $http
        .put('/paysheets/auto-loading-and-update-data-source/' + paysheetId, {
          modifiedDate,
          branches: branchs,
        })
        .success(function (resp) {
          defer.resolve(resp);
        })
        .error(function (error) {
          defer.reject(error);
        });

      return defer.promise;
    },

    getEmployeeForPaysheet: function (data) {
      var defer = $q.defer();
      $http({
        method: 'GET',
        url: "/employees/for-paysheet",
        params: data,
      })
        .success(function (resp) {
          defer.resolve(resp);
        })
        .error(function (error) {
          defer.reject(error);
        });
      return defer.promise;
    },

    getSettingByTenantId: function (tenantId) {
      var defer = $q.defer();
      $http({
        method: 'GET',
        url: '/settings/' + tenantId,
      })
        .success(function (resp) {
          defer.resolve(resp);
        })
        .error(function (error) {
          defer.reject(error);
        });
      return defer.promise;
    },

    /**
      * Lấy danh sách kì làm việc theo kỳ hạn trả lương
      * @param {any} isUpdatePaysheet trạng thái bảng lương
      * @param {any} salaryPeriodType kỳ hạn trả lương
      * @param {any} startDate Ngày bắt đầu kì làm việc
      * @param {any} endDate Ngày kết thúc kì làm việc
      * @return {Promise} promise
    */
    generateWorkingPeriod: function(isUpdatePaysheet, salaryPeriodType, startDate, endDate) {
      const defer = $q.defer();

      $http
        .get('/paysheets/generate-working-period', {
            params: {
              isUpdatePaysheet: isUpdatePaysheet,
              salaryPeriodType: salaryPeriodType,
              startDate: startDate,
              endDate: endDate,
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
    
    /**
     * Thực hiện tạo bảng lương
     * @param {any} startTime Ngày bắt đầu
     * @param {any} endTime Ngày kết thúc
     * @param {any} salaryPeriod Kì hạn trả lương
     * @return {Promise} promise
     */
    createPaysheet: function (startTime, endTime, salaryPeriod, branchs) {
      const defer = $q.defer();

      $http
          .post("/paysheets", {
              startTime: startTime,
              endTime: endTime,
              salaryPeriod: salaryPeriod,
              branches: branchs,
          })
          .success(function (resp) {
              defer.resolve(resp);
          })
          .error(function (error) {
              defer.reject(error);
          });

      return defer.promise;
    },

    /**
     * Thực hiện chốt lương
     * @param {any} paysheet Thông tin bảng lương
     * @param {any} isCheckPayslipPayment cờ để hiện thị popup xác nhận hủy phiếu thanh toán
     * @param {any} isCancelPayment xác nhận hủy
     * @return {Promise} promise
     */
    completePaysheetTemp: function (paysheet, isCheckPayslipPayment, isCancelPayment) {
      const defer = $q.defer();

      $http
        .post('/paysheets/complete', {
            paysheet: paysheet,
            isCheckPayslipPayment: isCheckPayslipPayment,
            isCancelPayment: isCancelPayment,
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

paysheetService.$inject = ['$http', '$q'];

export default paysheetService;
