var map;
var x = document.getElementById('latitude');
var y = document.getElementById('longitude');


var latitude = 0;
var longitude = 0;
var accuracy = 0;
function initMap() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
            function (position) {
                while (latitude == 0 || longitude == 0) {
                    latitude = parseFloat(position.coords.latitude);
                    longitude = parseFloat(position.coords.longitude);
                    accuracy = position.coords.accuracy;
                }
                if (latitude != 0 && longitude != 0) {
                    x.value = latitude;
                    y.value = longitude;
                    // Basic options for a simple Google Map
                    // For more options see: https://developers.google.com/maps/documentation/javascript/reference#MapOptions
                    var mapOptions = {
                        // How zoomed in you want the map to start at (always required)
                        zoom: 16,

                        // The latitude and longitude to center the map (always required)
                        center: new google.maps.LatLng(latitude, longitude), // New York

                        // How you would like to style the map. 
                        // This is where you would paste any style found on Snazzy Maps.
                        styles: [{
                            "elementType": "geometry",
                            "stylers": [{
                                "hue": "#ff4400"
                            }, {
                                "saturation": -68
                            }, {
                                "lightness": -4
                            }, {
                                "gamma": 0.72
                            }]
                        }, {
                            "featureType": "road",
                            "elementType": "labels.icon"
                        }, {
                            "featureType": "landscape.man_made",
                            "elementType": "geometry",
                            "stylers": [{
                                "hue": "#0077ff"
                            }, {
                                "gamma": 3.1
                            }]
                        }, {
                            "featureType": "water",
                            "stylers": [{
                                "hue": "#00ccff"
                            }, {
                                "gamma": 0.44
                            }, {
                                "saturation": -33
                            }]
                        }, {
                            "featureType": "poi.park",
                            "stylers": [{
                                "hue": "#44ff00"
                            }, {
                                "saturation": -23
                            }]
                        }, {
                            "featureType": "water",
                            "elementType": "labels.text.fill",
                            "stylers": [{
                                "hue": "#007fff"
                            }, {
                                "gamma": 0.77
                            }, {
                                "saturation": 65
                            }, {
                                "lightness": 99
                            }]
                        }, {
                            "featureType": "water",
                            "elementType": "labels.text.stroke",
                            "stylers": [{
                                "gamma": 0.11
                            }, {
                                "weight": 5.6
                            }, {
                                "saturation": 99
                            }, {
                                "hue": "#0091ff"
                            }, {
                                "lightness": -86
                            }]
                        }, {
                            "featureType": "transit.line",
                            "elementType": "geometry",
                            "stylers": [{
                                "lightness": -48
                            }, {
                                "hue": "#ff5e00"
                            }, {
                                "gamma": 1.2
                            }, {
                                "saturation": -23
                            }]
                        }, {
                            "featureType": "transit",
                            "elementType": "labels.text.stroke",
                            "stylers": [{
                                "saturation": -64
                            }, {
                                "hue": "#ff9100"
                            }, {
                                "lightness": 16
                            }, {
                                "gamma": 0.47
                            }, {
                                "weight": 2.7
                            }]
                        }]
                    };

                    // Get the HTML DOM element that will contain your map 
                    // We are using a div with id="map" seen below in the <body>
                    // Create the Google Map using our element and options defined above                  
                    map = new google.maps.Map(document.getElementById('map1'), mapOptions);

                    // Let's also add a marker while we're at it
                    var marker = new google.maps.Marker({
                        position: new google.maps.LatLng(latitude, longitude),
                        map: map,
                        title: 'location',
                        draggable: true,
                        icon: '../Content/img/Pin.png'
                    });

                    google.maps.event.addListener(marker, 'dragend', function (a) {
                        x.value = this.getPosition().lat();
                        y.value = this.getPosition().lng();
                    });

                }
            })
    }
}