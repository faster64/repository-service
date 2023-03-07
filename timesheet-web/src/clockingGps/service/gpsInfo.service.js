function gpsInfoService($rootScope) {
    return {
        setGeoCoordinate: function () {
            if (navigator.geolocation) {

                // Use for debug
                /*
                    navigator.geolocation.getCurrentPosition(showPosition, showError);
                */
                navigator.geolocation.getCurrentPosition(showPosition);
            } else {
                alert("Trình duyệt không hỗ trợ Geolocation.");
            }

            function showPosition(position) {
                $rootScope.geoCoordinate = position.coords.latitude.toString() + "," + position.coords.longitude.toString();

                // Use for debug
                /*
                const latSeting = $rootScope.gpsInfo.coordinate.split(",")[0];
                const longSeting = $rootScope.gpsInfo.coordinate.split(",")[1];
                var val = Math.abs(getDistance(latSeting, longSeting, position.coords.latitude, position.coords.longitude)).toFixed(2);
                if (val <= $rootScope.gpsInfo.radiusLimit) {
                  alert("Tọa độ của bạn: " + $rootScope.geoCoordinate + " > Vị trí hợp lệ");
                } else {
                  alert("Tọa độ của bạn: " + position.coords.latitude + "," + position.coords.longitude + " > Vị trí vượt giới hạn (m): " + val + "/" + $rootScope.gpsInfo.radiusLimit);
                }
                */
            }

            function showError(error) {
                switch (error.code) {
                    case error.PERMISSION_DENIED:
                        alert("Vui lòng bật chia sẻ vị trí cho trình duyệt.");
                        break;
                    case error.POSITION_UNAVAILABLE:
                        alert("Location information is unavailable.");
                        break;
                    case error.TIMEOUT:
                        alert("The request to get user location timed out.");
                        break;
                    case error.UNKNOWN_ERROR:
                        alert("An unknown error occurred.");
                        break;
                }
            }

            function getDistance(lat1, long1, lat2, long2) {
                var R = 6378137; // Earth’s mean radius in meter
                var dLat = rad(lat2 - lat1);
                var dLong = rad(long2 - long1);
                var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                    Math.cos(rad(lat1)) * Math.cos(rad(lat2)) *
                    Math.sin(dLong / 2) * Math.sin(dLong / 2);
                var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
                var d = R * c;
                return d; // returns the distance in meter
            };

            function rad(x) {
                return x * Math.PI / 180;
            };
        },

    };
}

gpsInfoService.$inject = ["$rootScope"];

export default gpsInfoService;
