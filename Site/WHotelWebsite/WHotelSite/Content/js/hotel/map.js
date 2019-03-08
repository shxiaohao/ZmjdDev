$(function () {
	var point = new google.maps.LatLng(hotel.lat, hotel.lng);
	
	var map = new google.maps.Map(document.getElementById("map_canvas"), {
		center: point,
		zoom: 14
	});

	MarkerWithLabel.add(map, point, hotel.name, hotel.url, hotel.address);
});
