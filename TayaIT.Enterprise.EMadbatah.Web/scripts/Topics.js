$(document).ready(function () {

    var $request;

    $("a.aPopupAddTopic").click(function (e) {
        $(".txtTpcTitle").val('');
        $(".txtTpcIDPopupEdit").val('0');
        $(".txtSID").val($(this).attr("sid"));
        $(".popupoverlay").show();
        $(".popupAddTopic").show();
        e.preventDefault();
    });

    $(document).on('click', 'a.aPopupEditTopic', function (e) {
        var tpcid = $(this).attr("tpcid");
        $(".txtTpcTitle").val($("#dicTpcTitle" + tpcid).html().trim());
        $(".txtTpcIDPopupEdit").val(tpcid);
        $(".txtSID").val($(this).attr("sid"));
        $(".popupoverlay").show();
        $(".popupAddTopic").show();
        e.preventDefault();
    });

    $(document).on('click', 'a.aPopupAddParag', function (e) {
        var tpcid = $(this).attr("tpcid");
        $(".txtTpcParag").val('');
        $(".txtTpcIDPopupParag").val(tpcid);
        $(".txtTpcParagIDPopup").val("0")
        $(".txtSID").val($(this).attr("sid"));
        $(".popupoverlay").show();
        $(".popupAddTopicParag").show();
        e.preventDefault();
    });

    $(document).on('click', 'a.aPopupEditTpcParag', function (e) {
        var tpcparagid = $(this).attr("tpcparagid");
        $(".txtTpcParag").val($("#divTpcParagtxt_" + tpcparagid).html().trim()); //$("#divTpcParagtxt_" + tpcid).html()
        $(".txtTpcParagIDPopup").val(tpcparagid);
        $(".txtSID").val($(this).attr("sid"));
        $(".popupoverlay").show();
        $(".popupAddTopicParag").show();
        e.preventDefault();
    });

    $(".btnAddTopic").click(function (e) {
        if ($(".txtTpcTitle").val() != null && $(".txtTpcTitle").val() != "") {
            tpcid = $(".txtTpcIDPopupEdit").val();
            var funcname = tpcid == "0" ? "AddTopic" : "EditTopic";
            if ($request != null) {
                $request.abort();
                $request = null;
            }

            $request = jQuery.ajax({
                cache: false,
                type: 'post',
                url: 'TopicHandler.ashx',
                data: {
                    funcname: funcname,
                    tpctitle: $(".txtTpcTitle").val(),
                    sid: $(".txtSID").val(),
                    tpcid: tpcid
                },
                dataType: 'json',
                success: function (response) {
                    if (response != "0") {
                        if (tpcid == "0") {
                            var divContent = "<div id=\"divTpc" + response + "\"  tpcid=\"" + response + "\">" +
                            "<div class=\"row tpcraw\">" +
                             "<div class=\"hoverArrow down\" tpcid=\"" + response + "\"></div>" +
                                "<div class=\"grid_12 h2 dicTpcTitle\"  id=\"dicTpcTitle" + response + "\">" + $(".txtTpcTitle").val() + "</div>" +
                                "<div class=\"grid_4 h2\"><a href=\"javascript:void(0)\" class=\"aPopupAddParag\" sid=" + $(".txtSID").val() + " tpcid=\"" + response + "\">اضافة نص الطلب</a></div>" +
                                "<div class=\"grid_2 h2\"><a href=\"javascript:void(0)\" class=\"aPopupEditTopic\" sid=" + $(".txtSID").val() + " tpcid=\"" + response + "\">تعديل</a></div>" +
                                "<div class=\"grid_2 h2\"><a href=\"javascript:void(0)\" class=\"aPopupDeleteTopic\" sid=" + $(".txtSID").val() + " tpcid=\"" + response + "\">حذف</a></div>" +
                                "<div class=\"grid_2 h2\"><a href=\"javascript:void(0)\" class=\"aPopupGetAttTopic\" sid=" + $(".txtSID").val() + " tpcid=\"" + response + "\">مقدموا الطلب</a></div>" +
                                "<div class=\"clear\"></div>" +
                            "</div> " +
                            "<div id=\"divTpcParags" + response + "\" class=\"TpcParags\">" +
                            "</div>" +
                          "</div>";

                            var x = "divTpcs";
                            $(".TpcParags").hide();
                            $(".hoverArrow").removeClass("up").addClass("down");
                            $("#" + x).append(divContent).show();
                        }
                        else {
                            $("#dicTpcTitle" + tpcid).html($(".txtTpcTitle").val());
                        }
                        $(".popupoverlay").hide();
                        $(".popupAddTopic").hide();

                        e.preventDefault();
                    }
                },
                error: function (response) {
                    alert("Error");
                }
            });
        }
    }); // End Add

    $(document).on('click', '.aPopupDeleteTopic', function (e) {
        var tpcid = $(this).attr("tpcid");

        if ($request != null) {
            $request.abort();
            $request = null;
        }

        $request = jQuery.ajax({
            cache: false,
            type: 'post',
            url: 'TopicHandler.ashx',
            data: {
                funcname: 'DeleteTopic',
                tpcid: tpcid
            },
            dataType: 'json',
            success: function (response) {
                if (response != "0") {
                    $("#divTpc" + tpcid).remove();
                }
            },
            error: function (response) {
                alert("Error");
            }
        });
    });


    $(document).on('click', 'a.aPopupGetAttTopic', function (e) {
        var sid = $(this).attr("sid");
        var tpcid = $(this).attr("tpcid");
        if ($request != null) {
            $request.abort();
            $request = null;
        }

        $request = jQuery.ajax({
            cache: false,
            type: 'post',
            url: 'TopicHandler.ashx',
            data: {
                funcname: 'GetAllAtt',
                sid: sid,
                tpcid: tpcid
            },
            dataType: 'json',
            success: function (response) {
                AttCont = $("#AttCont");
                AttCont.empty();
                chb = "<div style='padding-bottom:10px' class=\"grid_6 h2 grid_att\"> <input type=\"checkbox\" name=\"chbAttID\" value=\"AttID\"><span>AttName</span></div>";
                var tmpTr = "";
                for (i = 0; i < response.length; i++) {
                    tmpchb = chb.replace(/AttID/g, response[i].ID).replace("AttName", response[i].AttendantTitle + " " + response[i].AttendantDegree + " " + response[i].LongName);
                    AttCont.append(tmpchb);
                    if (response[i].FirstName == "1")
                        $("input:checkbox[name='chb" + response[i].ID + "']", AttCont).prop('checked', true);
                }
                $(".spnTpcTitlePopup").html($("#divTpc" + tpcid + " div.dicTpcTitle").html());
                $(".txtTpcID").val(tpcid);
                $(".popupoverlay").show();
                $(".popupAttendant").show();

                e.preventDefault();
            },
            error: function (response) { }
        });
        e.preventDefault();
    });


    $(document).on('click', '.btnAddAttToTopic', function (e) {
        var names = [];
        $('.grid_att input:checked').each(function () {
            names.push(this.value);
        });
        if ($request != null) {
            $request.abort();
            $request = null;
        }

        $request = jQuery.ajax({
            cache: false,
            type: 'post',
            url: 'TopicHandler.ashx',
            data: {
                funcname: 'AddTopicAtt',
                json_str: JSON.stringify(names),
                tpcid: $(".txtTpcID").val()
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


    $(document).on('click', '.btnAddTopicParag', function (e) {
        var sid = $(".txtSID").val();
        var tpcid = $(".txtTpcIDPopupParag").val();
        var tpcparag = $(".txtTpcParag").val();
        var tpcparagid = $(".txtTpcParagIDPopup").val();

        if ($request != null) {
            $request.abort();
            $request = null;
        }

        funcname = tpcparagid == "0" ? "AddTopicParag" : "EditTopicParag";

        $request = jQuery.ajax({
            cache: false,
            type: 'post',
            url: 'TopicHandler.ashx',
            data: {
                funcname: funcname,
                tpcparagid: tpcparagid,
                tpcparag: tpcparag,
                tpcid: tpcid
            },
            dataType: 'json',
            success: function (response) {
                if (tpcparagid == "0") {
                    var html = "<div id=\"divTpcParag_" + response + "\" class=\"grid_22 TpcParag\">" +
                                "<div style=\"margin-top: 10px\">" +
                                "</div>" +
                                "<div class=\"row\">" +
                                    "<div class=\"grid_6 h2\" id=\"divTpcParagtxt_" + response + "\">" +
                                        "" + tpcparag + "</div>" +
                                    "<div class=\"grid_4 h2\">" +
                                        "<a href=\"javascript:void(0)\" class=\"aPopupEditTpcParag\" tpcparagid=\"" + response + "\"" +
                                            "tcpid=\"" + tpcid + "\" sid=\"" + sid + "\">تعديل</a></div>" +
                                        "<div class=\"grid_4 h2\">" +
                                        "<a href=\"javascript:void(0)\" class=\"aPopupDelTpcParag\" tpcparagid=\"" + response + "\">" +
                                           " حذف</a></div>" +
                                    "<div class=\"clear\">" +
                                    "</div>" +
                                "</div>" +
                            "</div>  <div class=\"clear\">" +
                            "</div>";
                    divTpcParags = $("#divTpcParags" + tpcid);
                    divTpcParags.append(html).show();
                    $('#divTpc' + tpcid + ' .hoverArrow').removeClass('down').addClass('up');

                } else {
                    if (response != "0") {
                        $("#divTpcParagtxt_" + tpcparagid).html(tpcparag);
                    }
                }
                $(".popupoverlay").hide();
                $(".popupAddTopicParag").hide();

                e.preventDefault();
            },
            error: function (response) {
                alert("Error");
            }
        });

        e.preventDefault();
    });


    $(document).on('click', '.aPopupDelTpcParag', function (e) {
        var tpcparagid = $(this).attr("tpcparagid");
        if ($request != null) {
            $request.abort();
            $request = null;
        }

        $request = jQuery.ajax({
            cache: false,
            type: 'post',
            url: 'TopicHandler.ashx',
            data: {
                funcname: 'DeleteTopicParag',
                tpcparagid: tpcparagid
            },
            dataType: 'json',
            success: function (response) {
                if (response != "0") {
                    $("#divTpcParag_" + tpcparagid).remove();
                }
            },
            error: function (response) {
                alert("Error");
            }
        });
    });

    $(document).on('click', '.hoverArrow', function () {
        var curent = $(this);
        $('.hoverArrow').not(curent).removeClass('up').addClass('down');
        $('.TpcParags').hide();
        $(this).toggleClass('down up');
        if (curent.hasClass("up")) {
            $('#divTpcParags' + curent.attr("tpcid").trim()).show();
        }
    });

    $(".close_btn").click(function () {
        $(".popupoverlay").hide();
        $(".reviewpopup_cont").hide();
        $(".reviewpopup_cont-st1").hide();
    });
});
