import template from "./two-fa-pin.html";
import UAParser from "ua-parser-js";

function twoFaPinCmpController(clockingGpsService, $rootScope, $location) {
    let vm = this;
    vm.verifyCode1 = "";
    vm.verifyCode2 = "";
    vm.verifyCode3 = "";
    vm.verifyCode4 = "";
    vm.verifyCode5 = "";
    vm.verifyCode6 = "";
    vm.errorMessage = "";
    vm.isValidInput = false;

    vm.$onInit = function () {
        var uaP = new UAParser();
        vm.os = uaP.getOS();
        vm.device = uaP.getDevice();
    };

    vm.onChange = (val) => {
        var container = document.getElementsByClassName("input-code")[0];
        container.onkeyup = function (e) {
            var target = e.target;

            if (/^-?\d+$/.test(e.key)) {
                target.value = e.key;
            }

            var maxLength = parseInt(target.attributes["maxlength"].value, 10);
            var currentLength = target.value.length;

            if (currentLength >= maxLength) {
                var next = target;
                while (next = next.nextElementSibling) {
                    if (next == null)
                        break;
                    if (next.tagName.toLowerCase() == "input") {
                        next.focus();
                        break;
                    }
                }
            } else if (currentLength < maxLength) {
                var prev = target;
                while (prev = prev.previousElementSibling) {
                    if (prev == null)
                        break

                    if (prev.tagName.toLowerCase() == "input") {
                        prev.focus();
                        break;
                    }
                }
            }

        }

        vm.isValidInput = true;
        let inputs = document.getElementsByTagName('input');
        for (var i = 0; i < inputs.length; ++i) {
            let input = inputs[i];
            if (input.value == null || input.value == "") {
                vm.isValidInput = false;
                break;
            }
        }
    };

    vm.onSubmit = function () {
        vm.errorMessage = "";

        if (!angular.isDefined($rootScope.currentEmployee) || !angular.isDefined($rootScope.currentEmployee.id)) {
            $rootScope.errorMessage = "Có lỗi xảy ra. Vui lòng quét lại mã QR.";
            $location.path("error");
        } else {
            if (vm.verifyCode1 == null) vm.verifyCode1 = "";
            if (vm.verifyCode2 == null) vm.verifyCode2 = "";
            if (vm.verifyCode3 == null) vm.verifyCode3 = "";
            if (vm.verifyCode4 == null) vm.verifyCode4 = "";
            if (vm.verifyCode5 == null) vm.verifyCode5 = "";
            if (vm.verifyCode6 == null) vm.verifyCode6 = "";
            let verifyCode = vm.verifyCode1.toString() + vm.verifyCode2.toString() + vm.verifyCode3.toString() + vm.verifyCode4.toString() + vm.verifyCode5.toString() + vm.verifyCode6.toString();
            clockingGpsService
                .updateDevice($rootScope.currentEmployee.id, verifyCode, vm.os.name, vm.os.version, vm.device.type)
                .then(
                    function (resp) {
                        localStorage["kvSessionEmployee"] = JSON.stringify(resp.result);
                        $location.path("clocking/").search({ IsPhone: $rootScope.findEmployeeQuery.IsPhone, KeyWord: $rootScope.findEmployeeQuery.KeyWord });
                    },
                    function (err) {
                        if (angular.isArray(err.errors) && err.errors.length > 0) {
                            vm.errorMessage = err.errors[0].message;
                            vm.verifyCode1 = "";
                            vm.verifyCode2 = "";
                            vm.verifyCode3 = "";
                            vm.verifyCode4 = "";
                            vm.verifyCode5 = "";
                            vm.verifyCode6 = "";
                            vm.isValidInput = false;
                            document.getElementById("verifyCode1").focus();
                        }
                    }
                );
        }
    };
}

twoFaPinCmpController.$inject = [
    "clockingGpsService",
    "$rootScope",
    "$location",
];

const twoFaPinCmp = {
    restrict: "E",
    bindings: {},
    template: template,
    controller: twoFaPinCmpController,
    controllerAs: "vm",
};

export default twoFaPinCmp;
