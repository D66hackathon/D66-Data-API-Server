/*global angular */

(function (angular) {
    "use strict";

    angular.module("d66Hackathon")
        .config(["$stateProvider", "$urlRouterProvider", function ($stateProvider, $urlRouterProvider) {
            //var electionId = "GR2014_0344"; // @todo make this dynamic
            //var electionId = "REF_2005_0344";
        var electionId = "GR2014_0344";

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
                .state("404", {
                    url: "/404",
                    templateUrl: "views/404.html"
                });
        }]);
}(angular));
