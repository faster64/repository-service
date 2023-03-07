import template from "./find-employee.html";
import $ from "jquery";
import UAParser from "ua-parser-js";

function findEmployeeCmpController(clockingGpsService, $rootScope, $location) {
    let vm = this;
    vm.isPhone = true;
    $rootScope.errorMessage = "Message error init on $rootScope";
    vm.placeholder = "Nhập số điện thoại";
    vm.error = '';

    vm.$onInit = function () {
        var uaP = new UAParser();
        vm.os = uaP.getOS();
        vm.device = uaP.getDevice();

        vm.kvSessionEmployee = localStorage["kvSessionEmployee"] ? JSON.parse(localStorage["kvSessionEmployee"]) : null;
        if (vm.kvSessionEmployee) {
            $rootScope.currentEmployee = vm.kvSessionEmployee;
            vm.isPhone = false;
            vm.keyWord = $rootScope.currentEmployee.code;
        }
        vm.findEmployee();
    };

    vm.findEmployee = function () {
        console.log(vm.isPhone);
        if (vm.isPhone && !vm.checkPhoneNumber(vm.keyWord)) {
            vm.error = "Số điện thoại bạn nhập không hợp lệ.";
        } else {
            clockingGpsService.findEmployee(vm.os.name, vm.os.version, vm.device.type, vm.isPhone, vm.keyWord).then(
                function (resp) {
                    $rootScope.currentEmployee = resp.result.employee;
                    localStorage["kvSessionEmployee"] = JSON.stringify(resp.result.employee);
                    $location.path("clocking/").search({IsPhone : vm.isPhone, KeyWord : vm.keyWord});
                },
                function (err) {
                    if (err.errors[0].code == "new_device") {
                        let kvSessionEmployee = localStorage["kvSessionEmployee"] ? JSON.parse(localStorage["kvSessionEmployee"]) : null;
                        if (kvSessionEmployee) {
                            localStorage.removeItem("kvSessionEmployee");
                        } else {
                            if (angular.isDefined(err.result.employee)) {
                                $rootScope.currentEmployee = err.result.employee;
                                localStorage["kvSessionEmployee"] = JSON.stringify(err.result.employee);
                            }
                            $rootScope.findEmployeeQuery = {IsPhone : vm.isPhone, KeyWord : vm.keyWord};
                            $location.path("verify-two-fa-pin");
                        }
                    } else if (err.errors[0].code == "wrong_device") {
                        $rootScope.findEmployeeQuery = {IsPhone : vm.isPhone, KeyWord : vm.keyWord};
                        $rootScope.identityKeyClocking = err.result.identityKeyClocking;
                        localStorage["kvSessionEmployee"] = JSON.stringify(err.result.employee);
                        $rootScope.currentEmployee = err.result.employee;
                        $location.path("clocking/").search({IsPhone : vm.isPhone, KeyWord : vm.keyWord});
                    } else {
                        vm.error = err.message;
                    }
                }
            );
        }
    };

    vm.changePhoneOrCode = function (isPhone) {
        vm.placeholder = isPhone ? "Nhập số điện thoại" : "Nhập mã nhân viên";
        $("#input-phone-Code").attr("placeholder", vm.placeholder);
    };

    vm.checkPhoneNumber = function(phone) {
        var vnf_regex = /((09|03|07|08|05)+([0-9]{8})\b)/g;
        if(phone ==='' || vnf_regex.test(phone) == false){
            return false;
        } else {
            return true;
        }
    };
}

findEmployeeCmpController.$inject = [
    "clockingGpsService",
    "$rootScope",
    "$location",
];

const findEmployeeCmp = {
    restrict: "E",
    bindings: {},
    template: template,
    controller: findEmployeeCmpController,
    controllerAs: "vm",
};

export default findEmployeeCmp;
