import template from "./pageInit.html";
// import fingerprint2 from "../../../lib/fingerprint2";
import $ from "jquery";
import UAParser from "ua-parser-js";

function pageInitCmpController(clockingGpsService, $rootScope, $location, gpsInfoService) {
    let vm = this;
    $rootScope.identityKeyClocking = null;
    $rootScope.geoCoordinate = null;

    init();
    function init() {
        var uaP = new UAParser();
        console.log(uaP.getOS());
        console.log(uaP.getDevice());
        
        // console.log(window.navigator.userAgent);
        // fingerprint2.get(function (components) {
        //   var identityKeyClocking =
        //     fingerprint2.x64hash128(components.map(function (pair) { return pair.value }).join(), 31);
        //   console.log('identityKeyClocking=' + identityKeyClocking);
        //   $rootScope.identityKeyClocking = identityKeyClocking;
        // });
    $('#loading').show();
    var para = $location.search();
    console.log(para);
    clockingGpsService
        .login(para)
        .then(function (resp) {
            if (angular.isDefined(resp) && angular.isDefined(resp.result)) {
                var sessionData = {
                    retailer: resp.result.retailer,
                    bearerToken: resp.result.accessToken,
                    branchId: resp.result.branchId,
                };
                localStorage["kvSession"] = JSON.stringify(sessionData);

                $rootScope.gpsInfo = resp.result.gpsInfo;

                try {
                    gpsInfoService.setGeoCoordinate();
                } catch {
                    alert("getLocation error")
                }

                vm.employee = localStorage["kvSessionEmployee"] ? JSON.parse(localStorage["kvSessionEmployee"]) : null;
                if (!vm.employee || (vm.employee && angular.isUndefined(vm.employee.identityKeyClocking))) {
                    $('#loading').hide();
                    $location.path("find-employee");
                } else {
                    clockingGpsService.getEmployeeByIdentityKey(vm.employee.identityKeyClocking)
                    .then(function (resp){
                        console.log(resp.result);
                        let kvSessionEmployee = resp.result;
                        $('#loading').hide();
                        if (kvSessionEmployee) {
                            let isPhone = false;
                            let keyWord = kvSessionEmployee.code;
                            $location.path("clocking/").search({IsPhone : isPhone, KeyWord : keyWord});
                        }
                        $location.path("find-employee");
                    },
                    function (err){
                        $location.path("find-employee");
                    });
                }
            }
        })
        .catch(function (error) {
            $rootScope.errorMessage = error.message;
            $location.path("error");
            throw error;
        });
    }
}

pageInitCmpController.$inject = [
    "clockingGpsService",
    "$rootScope",
    "$location",
    "gpsInfoService"
];

const pageInitCmp = {
    restrict: "E",
    bindings: {},
    template: template,
    controller: pageInitCmpController,
    controllerAs: "vm",
};

export default pageInitCmp;
