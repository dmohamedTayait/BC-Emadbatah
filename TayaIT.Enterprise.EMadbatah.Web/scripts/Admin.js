
function AjaxEndMethod() {
    // change the date language
    $.datetimepicker.setLocale('ar');
    // date picker
    $(".Calender").datetimepicker({
        timepicker: false,
        defaultDate: new Date(),
        format: 'm/d/Y'
    });
    // time picker
    $(".timePicker").datetimepicker({
        datepicker: false,
        defaultDate: new Date(),
        format: 'H:i'
    });
}

$(document).ready(function () {

    AjaxEndMethod();
    del_init();
    att_init();
    function htmlEncode(value) {
        return $('<div/>').text(value).html();
    }


    $("a.aPopupAddSessionComm").click(function (e) {
        $(".txtAddDetails").val('');
        $(".txtCreatedAt").val('');
        $(".lblCommName").html($(this).attr("commname"));
        $(".txtCommName").val('');
        $(".txtSID").val($(this).attr("sid"));
        $(".txtCommID").val($(this).attr("commid"));
        $(".popupoverlay").show();
        $(".popupAddSessionComm").show();
        AjaxEndMethod();
        e.preventDefault();
    });


    $(".btnAddSessionComm").click(function (e) {
        if ($(".txtCreatedAt").val() != null && $(".txtCreatedAt").val() != "") {
            jQuery.ajax({
                cache: false,
                type: 'post',
                url: 'CommitteeHandler.ashx',
                data: {
                    funcname: 'AddSessionCommittee',
                    commname: $(".txtCommName").val(),
                    scommdetails: $(".txtAddDetails").val(),
                    scommcreatedat: $(".txtCreatedAt").val(),
                    sid: $(".txtSID").val(),
                    commid: $(".txtCommID").val()
                },
                dataType: 'json',
                success: function (response) {
                    if (response != "0") {

                        var divContent = " <div id=\"div_" + response + "\" class=\"grid_20 sessioncomm\">" +
                                     " <div style=\"margin-top:10px\"></div>" +
                                       "<div class=\"row\">" +
                                            "<div class=\"grid_3 h2\" style=\"color:#0134cb\"><span>اسم الجلسة :</span></div>" +
                                            "<div class=\"grid_6 h2\">" + $(".txtCommName").val() + "</div>" +
                                            " <div class=\"grid_4 h2\"><a href='javascript:void(0)' class=\"aPopupDelSessionComm\" scommid=\"" + response + "\" >حذف</a></div>" +
                                            " <div class=\"grid_4 h2\"><a href='javascript:void(0)' class=\"aPopupTakeSessionCommAtt\" scommid=\"" + response + "\" commid=\"" + $(".txtCommID").val() + "\" sid=\"" + $(".txtSID").val() + "\" >أخذ الغياب</a></div>" +
                                            " <div class=\"clear\"></div>" +
                                       " </div>" +
                                        "<div class=\"row\">" +
                                           "<div class=\"grid_3 h2\" style=\"color:#0134cb\"><span>تاريخ الانشاء :</span></div>" +
                                            "<div class=\"grid_6 h2\">" + $(".txtCreatedAt").val() + "</div>" +
                                             "<div class=\"clear\"></div>" +
                                        "</div>" +
                                        "<div class=\"row\">" +
                                            "<div class=\"grid_3 h2\" style=\"color:#0134cb\"><span>التفاصيل :</span></div>" +
                                            "<div class=\"grid_6 h2\">" + $(".txtAddDetails").val() + "</div>" +
                                            "<div class=\"clear\"></div>" +
                                        "</div>" +
                                     "</div>  <div class=\"clear\"></div>";
                        var x = "divContent" + $(".txtCommID").val().trim();
                        $("#" + x).append(divContent).show();
                        $(".popupoverlay").hide();
                        $(".popupAddSessionComm").hide();
                        del_init();
                        att_init();
                        e.preventDefault();
                    }
                },
                error: function (response) {
                    alert("Error");
                }
            });
        }
    }); // End Add

    function del_init() {
        $(".aPopupDelSessionComm").on("click", function (e) {
            var scommid = $(this).attr("scommid");
            jQuery.ajax({
                cache: false,
                type: 'post',
                url: 'CommitteeHandler.ashx',
                data: {
                    funcname: 'DeleteSessionCommittee',
                    scommid: scommid
                },
                dataType: 'json',
                success: function (response) {
                    if (response != "0") {
                        var x = "div_" + scommid;
                        console.log(x);
                        $("#" + x).remove();
                    }
                },
                error: function (response) {
                    alert("Error");
                }
            });
        });
    }

    function att_init() {
        $("a.aPopupTakeSessionCommAtt").click(function (e) {
            var scommid = $(this).attr("scommid");
            $(".txtSCommID").val(scommid);
            jQuery.ajax({
                cache: false,
                type: 'post',
                url: 'CommitteeHandler.ashx',
                data: {
                    funcname: 'GetSessionCommAtt',
                    sid: $(this).attr("sid"),
                    commid: $(this).attr("commid"),
                    scommid: scommid
                },
                dataType: 'json',
                success: function (response) {

                    tbl = $("#tbl_Att_Status");
                    $("#tbl_Att_Status .dataTr").remove();
                    tr = "<tr id=\"trAttID\" class='dataTr'>" +
                            "<td style=\"padding-right: 10px;\"><span>AttName</span></td>" +
                            "<td><input type=\"radio\" id=\"rdAttID\" name=\"rdAttID\" value='0' class='radio_list'/></td>" +
                            "<td><input type=\"radio\" id=\"rdAttID\" name=\"rdAttID\" value='2' class='radio_list'/></td>" +
                            "<td><input type=\"radio\" id=\"rdAttID\" name=\"rdAttID\" value='3' class='radio_list'/></td>" +
                            "<td><input type=\"radio\" id=\"rdAttID\" name=\"rdAttID\" value='4' class='radio_list'/></td>" +
                         "</tr>";
                    var tmpTr = "";
                    for (i = 0; i < response.length; i++) {
                        tmpTr = tr.replace(/AttID/g, response[i].ID).replace("AttName", response[i].AttendantTitle + " " + response[i].AttendantDegree + " " + response[i].LongName);
                        tbl.append(tmpTr);
                        $("input:radio[name='rd" + response[i].ID + "']", tbl).filter('[value="' + response[i].Status + '"]').prop('checked', true);
                    }

                    $(".popupoverlay").show();
                    $(".popupAttendant").show();
                    e.preventDefault();
                },
                error: function (response) { }
            });
            e.preventDefault();
        });
    }

    $(".btnAddSessionCommAtt").click(function (e) {
        tbl = $("#tbl_Att_Status");
        var rows = $('tr.dataTr', tbl);
        jsonObj = [];

        rows.each(function () {
            var id = $(this).attr("id").replace("tr", "");
            var status = 0;
            if ($("input:radio[name='rd" + id + "']:checked").val()) {
                status = $("input:radio[name='rd" + id + "']:checked").val();
            }

            item = {}
            item["DefaultAttendantID"] = id;
            item["status"] = status;

            jsonObj.push(item);
        });

        console.log(jsonObj);

        jQuery.ajax({
            cache: false,
            type: 'post',
            url: 'CommitteeHandler.ashx',
            data: {
                funcname: 'TakeSessionCommAttendance',
                json_str: JSON.stringify(jsonObj),
                scommid: $(".txtSCommID").val()
            },
            dataType: 'json',
            success: function (response) {

                $(".popupoverlay").hide();
                $(".popupAttendant").hide();
                e.preventDefault();
            },
            error: function (response) { }
        });
        e.preventDefault();
    });



    // on click on the arrows
    $('.hoverArrow').click(function () {
        var curent = $(this);
        $('.hoverArrow').not(curent).removeClass('up').addClass('down');
        $('.sessioncomms').hide();
        $(this).toggleClass('down up');
        if (curent.hasClass("up")) {
            $('#divContent' + curent.attr("commid").trim()).show();
        }
    })

    $(".close_btn").click(function () {
        $(".popupoverlay").hide();
        $(".reviewpopup_cont").hide();
        $(".reviewpopup_cont-st1").hide();
    });


});
