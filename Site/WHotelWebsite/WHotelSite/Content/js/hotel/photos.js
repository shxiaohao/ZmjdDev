$(function () {
	
	var PhotoSwipe = window.Code.PhotoSwipe

	var options = {
			preventHide: true,
			captionAndToolbarAutoHideDelay: 0,
			getImageSource: function(obj){
				return obj.url;
			},
			getImageCaption: function(obj){
				return obj.caption;
			}
		},
		images = $('#photos a').map(function() {
			return {url: this.href, caption: this.title};
		}),
		instance = PhotoSwipe.attach( 
			images,
			options 
		);

	instance.show(0);
});
