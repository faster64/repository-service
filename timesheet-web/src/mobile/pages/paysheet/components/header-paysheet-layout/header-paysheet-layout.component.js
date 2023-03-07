import moment from "moment";
import template from "./header-paysheet-layout.html";

function headerPaySheetLayoutCmpController(
  $route,
  tsFn,
  tsLabelFactory,
  paysheetService,
  $location,
  $state,
  $compile,
  $scope
) {
  //#region variables
  let vm = this;
  vm._l = tsLabelFactory;
  //#endregion

  //#region public
  vm.$onInit = function () {
    init();
    initModal();
    initEvents();
    vm.tsFn = tsFn;
  };

  vm.openModal = function (event) {
    var popupid = $(event.currentTarget).attr("rel");
    $("#" + popupid).addClass("open");
    $("body").addClass("body-hidden");
  };

  vm.backToMainApp = function () {
    if (isAndroid()) {
      Android.backToMainApp();
    } else {
      callIosNative("backToMainApp");
    }
  };

  vm.searchPaysheet = function (event) {
    $location.path(`/paysheet/search/`);
  };

  vm.createPaysheet = function (event) {
    $location.path(`/paysheet/create/`);
  };

  //#endregion

  //#region function
  function init() {
  }

  function initEvents() {
    $.fn.pullToRefresh(
      "#header-container",
      vm._l.pullToRefresh,
      vm._l.releaseToRefresh,
      function () {
        localStorage.removeItem("selectedDate");
      }
    );
  }

  function initModal() {  
    $(".ts-modal").click(function (event) {
      if ($(event.target).hasClass("ts-modal")) {
        closeModal($(this));
      }
    });
    $(".js-modal-close").click(function (_) {
      closeModal($(this).closest(".ts-modal"));
    });

    $('a.ts-link-search').click(function() {
      $(".ts-footer").addClass("open");
    });
  }

  function closeModal($target) {
    $target.removeClass("open");
    $("body").removeClass("body-hidden");
  }

  function isAndroid() {
    return typeof Android != "undefined";
  }

  function callIosNative(funcName) {
    if (
      window.webkit &&
      window.webkit.messageHandlers &&
      window.webkit.messageHandlers.sendToNative
    ) {
      window.webkit.messageHandlers.sendToNative.postMessage(funcName);
    }
  }
  //#endregion
}

headerPaySheetLayoutCmpController.$inject = [
  "$route",
  "tsFnFactory",
  "tsLabelFactory",
  "paysheetService",
  "$location",
  "$state",
  "$compile",
  "$scope",
];

const headerPaySheetLayoutCmp = {
  restrict: "E",
  bindings: {},
  template: template,
  controller: headerPaySheetLayoutCmpController,
  controllerAs: "vm",
};

export default headerPaySheetLayoutCmp;
