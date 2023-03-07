import angularLib from "../lib/angular";
import angularRouterLib from "../lib/angular-route.js";
import clockingGpsApp from "./clockingGps/clockingGps.module";

var app = angular.module("app", [clockingGpsApp.name, "ngRoute"]);

app.config([
  "$httpProvider",
  function ($httpProvider) {
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
      }
      return config;
    }
    function setBranchId(config) {
      var sessionJson = localStorage["kvSession"]
        ? JSON.parse(localStorage["kvSession"])
        : null;
      config.headers["BranchId"] = (sessionJson && sessionJson.branchId) || 0;
    }
    $httpProvider.interceptors.push(function () {
      return {
        request: function (config) {
          setBranchId(config);
          if (config.url.indexOf('/login-clocking-gps') > -1) {
            return config;
          }
          return authenticate(config);
        },
      };
    });
  },
]);
