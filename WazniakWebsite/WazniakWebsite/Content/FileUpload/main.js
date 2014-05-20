/*
 * jQuery File Upload Plugin JS Example 6.5.1
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * Copyright 2010, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/MIT
 */

/*jslint nomen: true, unparam: true, regexp: true */
/*global $, window, document */

function initialiseFileUploadControl() {
    var url = "/Home/UploadFiles";
    $.post(url)
        .done(function (data) {
            var parsedData = JSON.parse(data);
            var $form = $('#fileupload');
            $form.fileupload('option', 'done').call($form, $.Event('done'), { result: parsedData });
        });
    
}

$(function () {
    'use strict';

    // Initialize the jQuery File Upload widget:
    $('#fileupload').fileupload();

    $('#fileupload').fileupload('option', {
            maxFileSize: 500000000,
            resizeMaxWidth: 1920,
            resizeMaxHeight: 1200
    });

    initialiseFileUploadControl();
});
