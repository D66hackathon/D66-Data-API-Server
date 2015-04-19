/*global angular, d3 */

(function (angular) {
    "use strict";

    angular.module("d66Hackathon")
        .service("electionData", ["$http", function($http) {
            this.data = {};

            var _this = this;
            this.resolve = function(electionId) {
                if (!$.isEmptyObject(this.data)) {
                    return;
                }

                return $http.get('/API/GetElection/'+electionId)
                    .success(function(data) {
                        _this.data = data;
                    });
            };

            return this;
        }])
        .service("regionalData", ["$http", function($http) {
            this.data = {};

            var _this = this;
            this.resolve = function(electionId) {
                if (!$.isEmptyObject(this.data)) {
                    return;
                }

                return $http.get('/API/GetRegions/'+electionId)
                    .success(function(data) {
                        console.log(data);
                        _this.data = data;
                    });
            };

            return this;
        }]);
}(angular));
