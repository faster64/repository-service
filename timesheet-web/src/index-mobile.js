import toastr from 'toastr';
import '../lib/css/kendo-custom.scss';
import '../lib/css/kendo.css';
import './assets/styles-mobile.scss';
import { getErrorMsg } from './mobile/shared/helpers/notify-reponse.helpers';
import mobileModule from './mobile/mobile.module';

var app = angular.module('app', [mobileModule.name, 'ngRoute']);

app.config([
  '$httpProvider',
  function ($httpProvider) {
    //#region function
    function getKvSession() {
      const sessionJson = localStorage['kvSession']
        ? JSON.parse(localStorage['kvSession'])
        : null;
      return sessionJson;
    }
    function setKendoCulture() {
      var lang = localStorage.getItem('lang') || 'vi-VN';
      kendo && kendo.culture(lang);
      var dayInfo = kendo.culture().calendar.days;
      for (var i = 0; i < dayInfo.namesAbbr.length; i++) {
        dayInfo.namesShort[i] = dayInfo.namesAbbr[i];
      }
      var monthInfo = kendo.culture().calendar.months;
      for (var i = 0; i < monthInfo.names.length; i++) {
        monthInfo.names[i] = monthInfo.names[i] + ',';
      }
      moment.locale(lang.substring(0, 2).toLowerCase());
    }
    function backToApp() {
      if (typeof Android != 'undefined') {
        Android.backToMainApp();
      } else {
        if (
          window.webkit &&
          window.webkit.messageHandlers &&
          window.webkit.messageHandlers.sendToNative
        ) {
          window.webkit.messageHandlers.sendToNative.postMessage(
            'backToMainApp'
          );
        }
      }
    }
    function authenticate(config) {
      const sessionJson = getKvSession();
      if (sessionJson) {
        if (sessionJson.bearerToken)
          config.headers['authorization'] = 'Bearer ' + sessionJson.bearerToken;
        if (sessionJson.retailer) {
          config.headers['retailer'] = sessionJson.retailer.code;
          config.headers['x-group-id'] = sessionJson.retailer.groupId;
          config.headers['x-retailer-code'] = sessionJson.retailer.code;
        }
        if (sessionJson.branchId) {
          config.headers['BranchId'] = sessionJson.branchId;
        }
      }
      return config;
    }
    function redirect401() {
      location.href = location.pathname + '#/error/401';
    }
    function checkResponse(response) {
      switch (response.status) {
        case 401:
          redirect401();
          break;
        case 400:
        case 500:
          // if (response.data && response.data.message) {
          //   let errMsg = response.data.message;
          //   if (
          //     response.config &&
          //     response.config.url == "/mobile/branchs" &&
          //     errMsg.includes("không có quyền thực hiện")
          //   ) {
          //     var lang = localStorage.getItem("lang") || "vi-VN";
          //     if (lang === "en-US") {
          //       errMsg = "You do not have permission at this brand";
          //     } else {
          //       errMsg = "Người dùng không có quyền tại chi nhánh này";
          //     }
          //   }
          //   toastr.error(errMsg);
          // } else if (
          //   response.data &&
          //   response.data.responseStatus &&
          //   response.data.responseStatus.message
          // )
          //   toastr.error(response.data.responseStatus.message);
          // break;
          if (response.data && response.data.message) {
            let errMsg = response.data.message;
            if (
              response.config &&
              response.config.url == "/mobile/branchs" &&
              errMsg.includes("không có quyền thực hiện")
            ) {
              var lang = localStorage.getItem("lang") || "vi-VN";
              if (lang === "en-US") {
                errMsg = "You do not have permission at this brand";
              } else {
                errMsg = "Người dùng không có quyền tại chi nhánh này";
              }
            }
          }
          toastr.error(getErrorMsg(response.data));
          break;
        case 503:
          location.reload();
          break;
        default:
          break;
      }
    }
    function initHttpProvider() {
      $httpProvider.interceptors.push(function () {
        return {
          request: function (config) {
            return authenticate(config);
          },
          response: function (response) {
            checkResponse(response);
            return response;
          },
          responseError: function (response) {
            checkResponse(response);
            return response;
          },
        };
      });
    }
    function initEvents() {
      window.addEventListener('storage', function () {
        const session = this.localStorage.getItem('kvSession');
        if (!session) redirect401();
      });
    }
    function initAjaxSetup() {
      const sessionJson = getKvSession();
      var _headers = {};
      if (sessionJson) {
        if (sessionJson.bearerToken)
          _headers['authorization'] = 'Bearer ' + sessionJson.bearerToken;
        if (sessionJson.retailer) {
          _headers['retailer'] = sessionJson.retailer.code;
          _headers['x-group-id'] = sessionJson.retailer.groupId;
          _headers['x-retailer-code'] = sessionJson.retailer.code;
        }
        if (sessionJson.branchId) {
          _headers['BranchId'] = sessionJson.branchId;
        }
      }
      $.ajaxSetup({
        headers: _headers,
        beforeSend: function () {
          $.fn.showLoading();
        },
        success: function () {
          initUser();
        },
        error: function () {
          //$.fn.hideLoading();
        },
      });
    }
    function initUser() {
      const sessionJson = getKvSession();
      if (!sessionJson) {
        var lang = localStorage.getItem('lang');
        if (!lang) localStorage.setItem('lang', 'vi-VN');
        return;
      }
      $.ajax({
        async: false,
        method: 'GET',
        url: '/mobile/user',
        success: function (resp) {
          if (resp && resp.result) {
            if (resp.result.id)
              localStorage.setItem('lang', resp.result.language || 'vi-VN');
            else if (!resp.result.language)
              backToApp();
          } else {
            backToApp();
          }
          //$.fn.hideLoading();
        },
      });
    }
    function initToastr() {
      toastr.options = {
        progressBar: true,
        preventDuplicates: true,
        closeDuration: 0,
        closeButton: true,
        closeOnHover: false,
        // tapToDismiss: false,
        // positionClass: 'toast-top-center',
        timeOut: 3000,
      };
    }
    //#endregion

    initToastr();
    initHttpProvider();
    initEvents();
    initAjaxSetup();
    setKendoCulture();
  },
]);
