// confirm button onclick

jQuery.download = function (url, data, method) {
    //url and data options required
    if (url && data) {
        //data can be string of parameters or array/object
        data = typeof data == 'string' ? data : jQuery.param(data);
        //split params into form inputs
        var inputs = '';
        jQuery.each(data.split('&'), function () {
            var pair = this.split('=');
            inputs += '<input type="hidden" name="' + pair[0] + '" value="' + pair[1] + '" />';
        });
        //send request
        jQuery('<form action="' + url + '" method="' + (method || 'post') + '">' + inputs + '</form>')
		.appendTo('body').submit().remove();
    };
};
//$.download('/export.php','filename=mySpreadsheet&format=xls&content=' + spreadsheetData );

function downloadFile(sessionID, fileType, fileVersion) { 

$('.absLoad.loading').show();
jQuery.ajax({
    cache: false,
    type: 'post',
    url: 'FileHandler.ashx',
    data: {
        funcname: 'GetFilesStatus',
        sid: sessionID
    },
    dataType: 'json',
    success: function (response) {
        if (response != "ERROR") {
            var errorMessage = "";
            switch (response) {
                case "NotCreated":
                    errorMessage = "لم يتم إنشاء ملفات الجلسة بعد .. يجب الموافقة على الجلسة أولا قبل إنشاء الملفات";
                    alert(errorMessage);
                    break;
                case "InProgress":
                    //alert('');
                    errorMessage = "مازات الملفات قيد الإنشاء .. تستغرق هذه العملية عدة دقائق .. عند الإنتهاء منها سيرسل لك التطبيق رسالة إلكترونية";
                    break;
                case "DraftCreated":
                    jQuery.download('FileHandler.ashx', 'sid=' + sessionID + '&filetype=' + fileType + '&filever=' + fileVersion + '&funcname=Download');
                    break;
                case "FinalCreated":
                    jQuery.download('FileHandler.ashx', 'sid=' + sessionID + '&filetype=' + fileType + '&filever=' + fileVersion + '&funcname=Download');
                    break;
                case "DraftFail":
                    errorMessage = "تعذر انشاء نسخة المسودة من المضبطة لسبب فني .. سيقوم التطبيق بإعادة المحاولة \n  تستغرق هذه العملية عدة دقائق .. عند الإنتهاء منها سيرسل لك التطبيق رسالة إلكترونية";
                    //ajax createMadbatah files
                    break;
                case "FinalFail":
                    errorMessage = "تعذر انشاء النسخة المصدقة من المضبطة لسبب فني .. سيقوم التطبيق بإعادة المحاولة \n  تستغرق هذه العملية عدة دقائق .. عند الإنتهاء منها سيرسل لك التطبيق رسالة إلكترونية";
                    //ajax createMadbatah files
                    break;
                default:
                    break;
            }

            if (response == "DraftFail" || response == "FinalFail") {
                alert(errorMessage);
                jQuery.ajax({
                    cache: false,
                    type: 'post',
                    url: 'SessionHandler.ashx',
                    data: {
                        funcname: 'CreateMadbatahFiles',
                        sid: sessionID,
                        filever: response == "DraftFail" ? "draft" : "final"

                    },
                    dataType: 'json'
                });
            }
            else if (response == "InProgress") {
                alert(errorMessage);
            }

        }
        else {
            alert('لقد حدث خطأ');
        }

        $('.absLoad.loading').hide();
    },
    error: function () {
        $('.absLoad.loading').hide();
    }
});



 //MadbatahFilesStatus filesStatus = EMadbatahFacade.GetSessionMadbatahFilesStatus(session.SessionID);
                                               //string hrefWord = "javascript:jQuery.download('FileHandler.ashx','sid=" + session.SessionID.ToString() + "&filetype=doc&filever=" + fileVersion + "&funcname=Download');";
                                               

}

$(document).ready(function () {
    $(".smalltable .removeFile").live('click', function () {
        //alert('');
        var sessionID = $(this).parents("tr").attr('data-sessionid');
        var sessionFileID = $(this).parents("tr").attr('data-id');
        //  alert(sessionID);
        // alert(sessionFileID);
        $.ajax({
            cache: false,
            type: 'post',
            url: 'SessionHandler.ashx',
            data: {
                funcname: 'RemoveSessionFile',
                sfid: sessionFileID
            },
            success: function (response) {
                if (response == 'true') {
                    $("tr [data-id=" + sessionFileID + "]").animate({
                        opacity: 0.25,
                        left: "+=50",
                        height: "toggle"
                    }, 2000, function () {
                        $(this).remove();
                    });



                    alert("تم الحذف بنجاح");

                }
            },
            error: function () {
                alert("لقد حدث خطأ");
            }
        });

    });
});

