/*
	Standards Compliant Rollover Script
	Author : Daniel Nolan
	http://www.bleedingego.co.uk/webdev.php
*/

var preLoadImg = new Object();

function initRollOvers(){
	jQuery("img.img_on").each(function(){
		var imgSrc = this.src;
		var sep = imgSrc.lastIndexOf('.');
		var onSrc = imgSrc.substr(0, sep) + 'On' + imgSrc.substr(sep, 4);
		preLoadImg[imgSrc] = new Image();
		preLoadImg[imgSrc].src = onSrc;
		jQuery(this).hover(
			function() { this.src = onSrc; },
			function() { this.src = imgSrc; }
		);
	});
}
jQuery(function(){
	initRollOvers();
});