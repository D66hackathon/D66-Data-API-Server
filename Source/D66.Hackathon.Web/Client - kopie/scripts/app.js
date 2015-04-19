/*global angular */

(function (angular) {
    "use strict";

    // instantiate application
    var app = angular.module("d66Hackathon", [
        "ngCookies",
        "ui.router",
        "classy"
    ]);

    // configure classy to not add all properties & methods to $scope
    // use "SomeCtrl as some" syntax instead
    app.classy.options.controller = {
        addFnsToScope: false
    };
}(angular));
