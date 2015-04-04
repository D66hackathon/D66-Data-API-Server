/*global angular, d3, ol, $ */

(function (angular) {
    "use strict";

    angular.module("d66Hackathon")
        .directive("heatMap", function() {
            return {
                restrict: "A",
                replace: false,
                scope: {data: "=mapData"},
                link: function (scope, elements) {
                    var regions = scope.data.Regions;

                    var features = [];
                    $.each(regions, function(i, region) {
                        features.push(new ol.Feature({
                            geometry: new ol.geom.Point(
                                ol.proj.transform([region.Longitude, region.Latitude], 'EPSG:4326', 'EPSG:3857')
                            ),
                            name: region.Name,
                            votes: region.Parties[3].Votes
                        }));
                    });


                    var vectorSource = new ol.source.Vector({
                        features: features
                    });
                    var vector = new ol.layer.Heatmap({
                        source: vectorSource,
                        blur: 5,
                        radius: 15
                    });
   

                    var map = new ol.Map({
                        target: elements[0],
                        layers: [
                            new ol.layer.Tile({
                                source: new ol.source.MapQuest({layer: 'osm'})
                            }),
                            vector
                        ],
                        view: new ol.View({
                            center: ol.proj.transform([5.096, 52.089], 'EPSG:4326', 'EPSG:3857'),
                            zoom: 13
                        })
                    });
                }
            };
        });
}(angular));
