/*global angular */

(function (angular) {
    "use strict";

    angular.module("d66Hackathon")
        .classy.controller({
            name: "RegionsCtrl",
            inject: {
                "$scope": ".",
                "regionalData": "regionalData",
                "electionData": "electionData"
            },

            init: function () {
                this.regionalData = this.regionalData.data;
                this.electionData = this.electionData.data;
            }
        });
}(angular));
