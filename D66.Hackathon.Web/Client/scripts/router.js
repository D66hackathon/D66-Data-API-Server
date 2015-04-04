/*global angular */

(function (angular) {
    "use strict";

    angular.module("d66Hackathon")
        .config(["$stateProvider", "$urlRouterProvider", function ($stateProvider, $urlRouterProvider) {
            var electionId = "GR2014_0344"; // @todo make this dynamic

            // Unmatched routes will 404
            $urlRouterProvider.otherwise("/404");

            // Set home path
            $urlRouterProvider.when("", "/");

            // Define states
            $stateProvider
                .state("intro", {
                    url: "/",
                    templateUrl: "views/intro.html",
                    controller: "IntroCtrl as ctrl",
                    resolve: {
                        data: ["electionData", function(electionData) {
                            return electionData.resolve(electionId);
                        }]
                    }
                })
                .state("regions", {
                    url: "/regions",
                    templateUrl: "views/regions.html",
                    controller: "RegionsCtrl as ctrl",
                    resolve: {
                        data: ["regionalData", "electionData", function(regionalData, electionData) {
                            electionData.resolve(electionId);
                            regionalData.resolve(electionId);
                        }]
                    }
                })
                .state("map", {
                    url: "/map",
                    templateUrl: "views/map.html",
                    controller: "MapCtrl as ctrl",
                    resolve: {
                        data: ["regionalData", "electionData", function(regionalData, electionData) {
                            electionData.resolve(electionId);
                            regionalData.resolve(electionId);
                        }]
                    }
                })
                .state("404", {
                    url: "/404",
                    templateUrl: "views/404.html"
                });
        }]);
}(angular));
