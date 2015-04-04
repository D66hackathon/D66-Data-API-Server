/*global angular, d3, d3pie, $ */

(function (angular) {
    "use strict";

    angular.module("d66Hackathon")
        .directive("pieChart", function() {
            return {
                restrict: "E",
                replace: false,
                scope: {data: "=chartData", title: "=chartTitle"},
                link: function (scope, elements) {
                    var parties = scope.data;
                    var title = scope.title;

                    var contents = [];
                    $.each(parties, function(i, party) {
                        contents.push(
                            { label: party.Name, value: party.Percentage }
                        );
                    });

                    var pie = new d3pie(elements[0], {
                        header: {
                            title: {
                                text: title
                            }
                        },
                        data: {
                            content: contents
                        }
                    });
                }
            };
        });
}(angular));
