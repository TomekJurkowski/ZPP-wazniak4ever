﻿$(document).ready(function () {
    var c = false,
		b = false;
    $(".slide").css("opacity", 1);
    $(".main").onepage_scroll({
        sectionContainer: "section"
    });
    var a = new $.BigVideo({
        useFlashForFirefox: false
    });
    a.init();
    if (Modernizr.touch) {
        a.show("img/background1.jpg");
    } else {
        a.show("video/timelapse.mp4", {
            ambient: true,
            doLoop: true,
            altSource: "video/timelapse.ogv"
        });
    }
    setTimeout(function () {
        $("#big-video-wrap").show().animate({
            opacity: 1
        });
    }, 2500);
    $(".demo1 .rotate").textrotator({
        animation: "fade",
        speed: 2000
    });
    $(".take-a-look").on("click", function () {
        $(".onepage-pagination li:nth-child(2)").children().trigger("click");
    });
    $(".onepage-pagination").addClass("animated fadeInRight delay-3");
    $(".onepage-pagination").children().each(function () {
        switch ($(this).children().data("index")) {
            case 1:
                $(this).children().append("<i class='entypo-home'></i>");
                break;
            case 2:
                $(this).children().append("<i class='entypo-dot'></i>");
                break;
            case 3:
                $(this).children().append("<i class='entypo-dot-2'></i>");
                break;
            case 4:
                $(this).children().append("<i class='entypo-dot-3'></i>");
                break;
            case 5:
                $(this).children().append("<i class='entypo-mail'></i>");
                break;
        }
    });
    setInterval(function () {
        if ($("body").hasClass("viewing-page-2")) {
            $(".slide-1-title").removeClass("hide").addClass("animated fadeInRight delay-1");
            $(".slide-1-left").removeClass("hide").addClass("animated fadeInLeft delay-2");
            $(".iphone-1").removeClass("hide").addClass("animated fadeInUp delay-3");
            $(".slide-1-right").removeClass("hide").addClass("animated fadeInRight delay-2");
            $(".icon span").css("color", "#fff");
            console.log(c);
            if (c == true) {
                $(".slide-1").css("background-color", "#0085e4");
            }
        }
        if ($("body").hasClass("viewing-page-3")) {
            $(".slide-2-title").removeClass("hide").addClass("animated fadeInRight delay-1");
            $(".slide-2-left").removeClass("hide").addClass("animated fadeInLeft delay-2");
            $(".slide-2-right").removeClass("hide").addClass("animated fadeInRight delay-2");
            $(".icon span").css("color", "#fff");
            if (c == true) {
                $(".slide-2").css("background-color", "#FFCC00");
            }
        }
        if ($("body").hasClass("viewing-page-4")) {
            $(".icon span").css("color", "#fff");
        }
    }, 300);
    $(".settings-icon").on("click", function () {
        $(this).parent().toggleClass("open");
    });
    $(".toggle-video").on("click", function () {
        $("#big-video-wrap").toggleClass("hide-video");
    });
    $(".toggle-opacity-first").on("click", function () {
        $(".home").toggleClass("full-color");
    });
    $(".toggle-opacity-all").on("click", function () {
        $(".home").toggleClass("full-color");
        $(".slide-1").toggleClass("full-color");
        $(".slide-2").toggleClass("full-color");
        $(".contact").toggleClass("full-color");
    });
    $(".toggle-phone").on("click", function () {
        $(".home-phone").toggleClass("hide animated fadeIn");
        $(".home-copy").toggleClass("col-sm-8 col-sm-offset-2 col-sm-6");
    });
    $(".toggle-slide-fade").on("click", function () {
        if (c === false) {
            c = true;
            console.log(c);
        } else {
            if (c === true) {
                c = false;
                console.log(c);
            }
        }
        $(".slide-1").toggleClass("fade-background");
        $(".slide-2").toggleClass("fade-background");
    });
});