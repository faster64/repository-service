import "../assets/styles.scss";
import pageInitCmp from "./compoment/pageInit/pageInit.component";
import findEmployeeCmp from "./compoment/find-employee/find-employee.component";
import errorCmp from "./compoment/error/error.component";
import clockingGpsService from "./service/clockingGps.service";
import gpsInfoService from "./service/gpsInfo.service";
import clockingCmp from "./compoment/clocking/clocking.component";
import twoFaPinCmp from "./compoment/two-fa-pin/two-fa-pin.component";
import successCmp from "./compoment/success/success.component";

var clockingGpsApp = angular.module("clockingGpsApp", ["ngRoute"]);

clockingGpsApp.component("pageInitCmp", pageInitCmp);
clockingGpsApp.component("findEmployeeCmp", findEmployeeCmp);
clockingGpsApp.component("clockingCmp", clockingCmp);
clockingGpsApp.component("errorCmp", errorCmp);
clockingGpsApp.component("twoFaPinCmp", twoFaPinCmp);
clockingGpsApp.component("successCmp", successCmp);

clockingGpsApp.service("clockingGpsService", clockingGpsService);
clockingGpsApp.service("gpsInfoService", gpsInfoService);

clockingGpsApp.config([
  "$routeProvider",
  "$httpProvider",
  function ($routeProvider, $httpProvider) {
    $routeProvider
      .when("/init", {
        template: "<page-init-cmp class='main-page'></page-init-cmp>",
      })
      .when("/find-employee", {
        template: "<find-employee-cmp class='main-page'></find-employee-cmp>",
      })
      .when("/verify-two-fa-pin", {
        template: "<two-fa-pin-cmp></two-fa-pin-cmp>",
      })
      .when("/clocking", {
        template: "<clocking-cmp class='main-page'></clocking-cmp>",
      })
      .when("/error", {
        template: "<error-cmp class='main-page'></error-cpm>",
      })
      .when("/success", {
        template: "<success-cmp class='main-page'></success-cpm>",
      });
    // .otherwise({
    //   redirectTo: "/init",
    // });

    function authenticate(config) {
      var sessionJson = localStorage["kvSession"]
        ? JSON.parse(localStorage["kvSession"])
        : null;
      if (sessionJson != null) {
        if (angular.isDefined(sessionJson.bearerToken))
          config.headers["authorization"] = "Bearer " + sessionJson.bearerToken;
        if (angular.isDefined(sessionJson.retailer)) {
          config.headers["retailer"] = sessionJson.retailer.code;
          config.headers["x-group-id"] = sessionJson.retailer.groupId;
          config.headers["x-retailer-code"] = sessionJson.retailer.code;
        }
        if (angular.isDefined(sessionJson.branchId))
          config.headers["BranchId"] = sessionJson.branchId;
      }

      return config;
    }
    $httpProvider.interceptors.push(function () {
      return {
        request: function (config) {
          if (config.url.indexOf("/login-clocking-gps") > -1) {
            return config;
          }
          return authenticate(config);
        },
      };
    });
  },
]);

export default clockingGpsApp;
