/*global angular */

(function (angular) {
    "use strict";

    angular.module("d66Hackathon")
        .classy.controller({
            name: "IntroCtrl",
            inject: {
                "$scope": ".",
                "electionData": "electionData"
            },

            init: function () {
                this.electionData = this.electionData.data;
            }
        });
}(angular));
