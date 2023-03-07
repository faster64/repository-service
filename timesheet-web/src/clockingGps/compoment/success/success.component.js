import template from "./success.html";

function successCmpController($rootScope, $location) {
    let vm = this;
    vm.isCheckIn = $rootScope.isCheckIn;
    vm.params = $location.search();
    vm.$onInit = function () {
        if (angular.isUndefined($rootScope.isCheckIn) && angular.isUndefined($rootScope.successMessage)) {
            $location.path("init").search(vm.params);
        }
    };
    // vm.successMessage = $rootScope.successMessage;
    
    if($rootScope.successMessage) {
        vm.successMessage = $rootScope.successMessage;
        vm.confirmClocking = true;
        if (!angular.isUndefined($rootScope.acceptWrongGps)) {
            vm.wrongGps = $rootScope.acceptWrongGps;
        }
    } else {
        vm.successMessage = vm.isCheckIn ? "Chấm công vào thành công." : "Chấm công ra thành công."
    }

}

successCmpController.$inject = ["$rootScope", "$location"];

const successCmp = {
    restrict: "E",
    bindings: {},
    template: template,
    controller: successCmpController,
    controllerAs: "vm",
};

export default successCmp;
