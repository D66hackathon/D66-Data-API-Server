/*global angular */

(function (angular) {
    "use strict";

    angular.module("d66Hackathon")
        .classy.controller({
            name: "MapCtrl",
            inject: {
                "$scope": ".",
                "regionalData": "regionalData",
                "electionData": "electionData"
            },

            init: function () {
                this.regionalData = this.regionalData.data;
                this.electionData = this.electionData.data;

                console.log(this.regionalData);
            }
        });
}(angular));
