// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

$(document).ready(function () {
    $("input[type=text]").on("keyup", function () {

        if (ValidURL($(this).val()) === true) {
            $(".start article").removeClass("full-width");
            $(this).addClass("valid-background");
            $(this).removeClass("not-valid-background");
            
            $(".chibi-button").show();
        }
        else
        {
            $(".start article").addClass("full-width");
            $(this).addClass("not-valid-background");
            $(this).removeClass("valid-background");

            $(".chibi-button").hide();
        }

        if ($(this).val() === "") {
            $(this).removeClass("valid-background");
            $(this).removeClass("not-valid-background");
            $(".start article").addClass("full-width");
            $(".start article input").addClass("full-width-input");
        }

    });
});

$("input, textarea").focus(function () {
    $(this).data("placeholder", $(this).attr("placeholder"))
        .attr("placeholder", "");
}).blur(function () {
    $(this).attr("placeholder", $(this).data("placeholder"));
    });


function ValidURL(str) {
    regexp = /^(?:(?:https?|ftp):\/\/)?(?:(?!(?:10|127)(?:\.\d{1,3}){3})(?!(?:169\.254|192\.168)(?:\.\d{1,3}){2})(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))|(?:(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)(?:\.(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)*(?:\.(?:[a-z\u00a1-\uffff]{2,})))(?::\d{2,5})?(?:\/\S*)?$/;
    if (regexp.test(str)) {
        return true;
    }
    else {
        return false;
    }
}