// validation
function check() {
    var x = document.forms.editSessionFileForm
    x[0].checked = true
}

function uncheck() {
    var x = document.forms.editSessionFileForm
    x[0].checked = false
}
var prevAgendaItemIndex;
var prevAgendaSubItemIndex;
var prevSpeakerIndex;
var prevSpeakerTitle;
var prevFragOrder;
var prevSpeakerJob;
var prevSpeakerImgUrl;



$(document).ready(function() {
    // select tag
    var $MainContent_ddlSpeakers = $('#MainContent_ddlSpeakers').select2().trigger("change");
    // vars
    var startTime = $('.hdstartTime');
    var endTime = $('.hdendTime');
    var currentOrder = $('.hdcurrentOrder');
    var currentSessionContID = $(".hdSessionContentItemID");
    //all choosable items
    var allInputs = $("#MainContent_ddlAgendaItems,#MainContent_ddlAgendaSubItems,#MainContent_ddlSpeakers,#MainContent_txtSpeakerOtherJob,#specialBranch,#MainContent_ddlOtherTitles,#MainContent_ddlCommittee")
    // validate form onsubmit
    var formvalidation = $("#editSessionFileForm").validate({
        onclick: false,
        errorPlacement: function(error, element) {
            if (element.attr('id') == 'txtAgendaItem') {
                element.next().next('.errorCont').html(error)
            } else if (element.attr('id') == 'MainContent_ddlSpeakers') {
                // element.after(error);
                alert(error.html());
            } else {
                element.after(error);
            }
        },

        rules: {
            addnewjobtext: "required",
            ctl00$MainContent$ddlAgendaItems: {
                required: true,
                min: 1
            },
            ctl00$MainContent$ddlSpeakers: {
                required: true,
                min: 1
            },
            txtAgendaItem: {
                required: "#specialBranch:checked"
            }
        },
        messages: {
            addnewjobtext: "أدخل وظيفة المتحدث",
            ctl00$MainContent$ddlAgendaItems: "من فضلك اختر البند",
            ctl00$MainContent$ddlSpeakers: "من فضلك اختر المتحدث",
            txtAgendaItem: "ادخل اسم البند الأستثنائى"

        }
    });
    // remove checkbox selection
    //  $(".sameAsPrevSpeaker").selected(false);
    // $(".chkIgnoredSegment").selected(false);

    //usama march
    //$(".chkGroupSubAgendaItems").selected(false);

    // for the popup window
    var resetText = $("#various1");
    resetText.fancybox({
        titlePosition: 'inside',
        transitionIn: 'none',
        transitionOut: 'none',
        autoDimensions: false,
        padding: 20,
        width: 600,
        height: 120,
        href: resetText.attr('data-div')
    });
    // popup buttons actions
    $('#yes').click(function() {
        var ed = $('#MainContent_elm1').tinymce();
        var sessionContentItemID = currentSessionContID.val(); //getParameterByName("scid");
        var sessionFileID = getParameterByName("sfid");
        // Do you ajax call here, window.setTimeout fakes ajax call
        ed.setProgressState(1); // Show progress
        jQuery.ajax({
            cache: false,
            type: 'post',
            url: 'EditSessionHandler.ashx',
            data: {
                funcname: 'BackToOriginalContent',
                sfid: sessionFileID,
                scid: sessionContentItemID
            },
            dataType: 'json',
            success: function(response) {
                if (response != '') {
                    ed.setProgressState(0); // Hide progress
                    //ed.setContent(html);
                    if (response.Message == "success") {
                        $("#MainContent_elm1").val(response.Item.Text);
                        $("#MainContent_CurrentItemID").val(response.FragOrderInXml);
                    } else {
                        alert("عفواً . غير مسموح لك بالتعديل في هذه الصفحة");

                    }
                }
            },
            error: function() {

            }
        });
        // close popup
        $.fancybox.close()
    })
    $('#no').click(function() {
        $.fancybox.close()
    })

        $(".btnAddNewTopic").click(function (e) {
            tpcid = 0;
            if ($request != null) {
                $request.abort();
                $request = null;
            }

            $request = jQuery.ajax({
                cache: false,
                type: 'post',
                url: 'TopicHandler.ashx',
                data: {
                    funcname: "AddTopic",
                    sid: $(".sessionID").val()
                },
                dataType: 'json',
                success: function (response) {
                    if (response != "0") {
                        $(".topicId").val(response) ;
                        $(".chkTopic").attr("disabled","disabled");
                        $(".divTopic").show();
                        e.preventDefault();
                        $(".btnAddNewTopic").attr("disabled","disabled");
                        $(".aPopupGetAttTopic").show();
                    }
                },
                error: function (response) {
                    alert("Error");
                }
            });
    }); // End Add


    //onchange ignored
    $('.chkIgnoredSegment').change(function() {

        if ($(this).attr('checked')) {
            var selectedSpeakerID = $("#MainContent_ddlSpeakers").val();
            if (selectedSpeakerID == 0) {
                $("#MainContent_ddlSpeakers option:contains(" + "غير محدد" + ")").attr('selected', 'selected');
                $("#select2-MainContent_ddlSpeakers-container").html($('#MainContent_ddlSpeakers :selected').text());
            }
        }
    });

    // onchange event for SameSpeaker checkbox
    $('.sameAsPrevSpeaker').change(function() {
        if ($(".sameAsPrevSpeaker").is(':checked')) {
            //$("#editSessionFileForm").resetForm()
            $("#MainContent_ddlAgendaItems").val(prevAgendaItemIndex);
            $('#MainContent_ddlAgendaItems').trigger('change');
            $("#MainContent_ddlAgendaSubItems").val(prevAgendaSubItemIndex);
            $("#MainContent_ddlSpeakers").val(prevSpeakerIndex).trigger("change");
            $("#MainContent_txtSpeakerOtherJob").val(prevSpeakerTitle);
            $("#MainContent_imgSpeakerAvatar").attr("src", prevSpeakerImgUrl);
            $("#MainContent_txtSpeakerJob").html(prevSpeakerJob);
            $("#select2-MainContent_ddlSpeakers-container").html($('#MainContent_ddlSpeakers :selected').text());
            allInputs.attr('disabled', 'disabled');
            $("#MainContent_ddlOtherTitles").attr('disabled', 'disabled');
            // remove all errors
            $('label.error').remove()
            $('.error').removeClass('error')
        } else {
            allInputs.removeAttr('disabled', 'disabled');

            if ($(".chkGroupSubAgendaItems").is(':checked')) {
                $("#MainContent_ddlAgendaSubItems").attr('disabled', 'disabled');
            } else {
                $("#MainContent_ddlAgendaSubItems").removeAttr('disabled', 'disabled');
            }
            $("#MainContent_ddlSpeakers").val(0).trigger("change");
            $("#select2-MainContent_ddlSpeakers-container").html($('#MainContent_ddlSpeakers :selected').text());
            $("#MainContent_ddlOtherTitles").val(0);
            $("#MainContent_ddlCommittee").val(0).hide();
            $("#MainContent_txtSpeakerOtherJob").val("");
            $("#MainContent_imgSpeakerAvatar").attr("src", "/images/AttendantAvatars/unknown.jpg");
        }
    });

    //usama march
    // onchange event for SameSpeaker checkbox
    $('.chkGroupSubAgendaItems').change(function() {
        if ($(".chkGroupSubAgendaItems").is(':checked')) {
            $("#MainContent_ddlAgendaSubItems").attr('disabled', 'disabled');
            $("#MainContent_ddlAgendaItems > option:selected").attr("IsGroupSubAgendaItems", "true");
        } else {
            $("#MainContent_ddlAgendaSubItems").removeAttr('disabled', 'disabled');
            $("#MainContent_ddlAgendaItems > option:selected").attr("IsGroupSubAgendaItems", "false");
        }
    });


    // cascading drop down lists, to get AgendaSubItems By AgendaItem ID
    $("#MainContent_ddlAgendaItems").change(function() {
        $("#MainContent_ddlAgendaSubItems").html("");
        var AgendaItemID = $("#MainContent_ddlAgendaItems > option:selected").attr("value");

        var IsGroupSubAgendaItems = $("#MainContent_ddlAgendaItems > option:selected").attr("IsGroupSubAgendaItems");

        if (AgendaItemID <= 0) {
            $('#specialBranch').attr('disabled', 'disabled');
            $('select#MainContent_ddlAgendaSubItems').attr('disabled', 'disabled')
        } else {
            $('#specialBranch').removeAttr('disabled');
            $('select#MainContent_ddlAgendaSubItems').removeAttr('disabled');
        }

        if (AgendaItemID != 0) {
            $('select#MainContent_ddlAgendaSubItems').attr('disabled', 'disabled')
            jQuery.ajax({
                cache: false,
                type: 'post',
                url: 'EditSessionHandler.ashx',
                data: {
                    funcname: 'GetAgendaSubItems',
                    agendaid: AgendaItemID
                },
                //contentType: "application/json; charset=utf-8",
                dataType: 'json',
                success: function(subitems) {
                    $.each(subitems, function() {
                        $("#MainContent_ddlAgendaSubItems").append($("<option></option>").val(this['ID']).html(this['Text']));
                    });
                    if (subitems.length > 0) {
                        $('select#MainContent_ddlAgendaSubItems').removeAttr('disabled')
                    }

                    if (IsGroupSubAgendaItems == 'true') {
                        $('select#MainContent_ddlAgendaSubItems').attr('disabled', 'disabled');
                        $(".chkGroupSubAgendaItems").attr("checked", "checked");
                    } else {
                        $(".chkGroupSubAgendaItems").removeAttr("checked");
                    }

                    if ($(".sameAsPrevSpeaker").is(':checked')) {
                        $('select#MainContent_ddlAgendaSubItems').attr('disabled', 'disabled').val(prevAgendaSubItemIndex);
                    }

                },
                error: function() {

                }
            });
        }
    });



    $("#MainContent_ddlSpeakers").change(function() {
        $('#divNewSpeaker').hide("");
        var attendantID = $("#MainContent_ddlSpeakers").val();
        $("#MainContent_ddlSpeakers").removeClass("error");
        if (attendantID != 0 && attendantID != "1.5") {
            // $('select#MainContent_ddlAgendaSubItems').attr('disabled', 'disabled')
            jQuery.ajax({
                cache: false,
                type: 'post',
                url: 'EditSessionHandler.ashx',
                data: {
                    funcname: 'GetSpeakerJobTitleAndAvatar',
                    attid: attendantID
                },
                //contentType: "application/json; charset=utf-8",
                dataType: 'json',
                success: function(data) {
                    if (data != 'error') {
                        var d = new Date();
                        var n = d.getTime();
                        var dataArr = data.split(",");
                        $('#MainContent_txtSpeakerJob').html(dataArr[0]);
                        $('#MainContent_imgSpeakerAvatar').attr("src", "/images/AttendantAvatars/" + dataArr[1] + "?" + n.toString());
                        $('#MainContent_ddlOtherTitles').val(0).removeAttr('disabled', 'disabled');
                        $('#MainContent_ddlCommittee').val(0).hide().removeAttr('disabled', 'disabled');
                        $('#MainContent_txtSpeakerOtherJob').val("").removeAttr('disabled', 'disabled');
                        if ($("#MainContent_ddlSpeakers > option:selected").text() == "غير محدد" || $(".sameAsPrevSpeaker").is(':checked')) {
                            $('#MainContent_ddlOtherTitles').val(0).attr('disabled', 'disabled');
                            $('#MainContent_ddlCommittee').val(0).hide().attr('disabled', 'disabled');
                            $('#MainContent_txtSpeakerOtherJob').val("").attr('disabled', 'disabled');
                        }
                    }
                }

            });
        } else {
            $('#MainContent_txtSpeakerJob').html("");
            $('#MainContent_imgSpeakerAvatar').attr("src", "/images/AttendantAvatars/unknown.jpg");
            $('#MainContent_ddlOtherTitles').val(0).removeAttr('disabled', 'disabled');
            $('#MainContent_ddlCommittee').val(0).hide().removeAttr('disabled', 'disabled');
            $('#MainContent_txtSpeakerOtherJob').val("").removeAttr('disabled', 'disabled');
            if (attendantID == "1.5") {
                $('#divNewSpeaker').show("");
            } else {
                $('#divNewSpeaker').hide("");
            }
        }
    });


    // next button onclick
    $(".next").click(function() {
        if ($("#editSessionFileForm").valid()) {
            $(".next").attr("disabled", "disabled");
            // var AgendaItemID = $("#MainContent_ddlAgendaItems > option:selected").length > 0 ? $("#MainContent_ddlAgendaItems > option:selected").attr("value") : "";
            //  var AgendaSubItemID = $("#MainContent_ddlAgendaSubItems > option:selected").length > 0 ? $("#MainContent_ddlAgendaSubItems > option:selected").attr("value") : "";
            var AgendaItemID = $('.agendaItemId').val();
            var AttachID = $('.attachId').val();
            var VoteID = $('.voteId').val();
            var TopicID = $('.topicId').val();
            var SpeakerID = $("#MainContent_ddlSpeakers").val();
            SpeakerID = SpeakerID == 1.5 ? -1 : SpeakerID;
            var SpeakerName = $(".txtNewSpeaker").val();
            var SameAsPrevSpeaker = $(".sameAsPrevSpeaker").is(':checked');
            var MergeWithPrevTopic = $(".chkTopic").is(':checked');
            var IsSessionPresident = $(".isSessionPresident").is(':checked') ? "1" : "0";
            var IsGroupSubAgendaItems = $(".chkGroupSubAgendaItems").is(':checked');
            var Ignored = $(".chkIgnoredSegment").is(':checked');
            var SpeakerJob = $("#MainContent_txtSpeakerOtherJob").val();
            var SpeakerImgUrl = $("#MainContent_imgSpeakerAvatar").attr("src");
            // editor value
            var clone = $('<div>').append($("#MainContent_elm1").val());
            clone.find('span').removeClass('highlight editable hover');
            var Text = encodeURI(clone.html())
            // comments value
            var Comments = $("#MainContent_txtComments").val();
            var Footer = $("#MainContent_txtFooter").val();
            // Show progress
            var ed = $('#MainContent_elm1').tinymce()
            ed.setProgressState(1);
            $(".addingNewAgendaItem").show();
            // pause the player
            $("#jquery_jplayer_1").jPlayer("pause");
            //
            if (currentOrder.val() - 0 > prevFragOrder - 0) {
                if (SameAsPrevSpeaker == false && prevSpeakerIndex == SpeakerID && !$(".chkIgnoredSegment").is(':checked')) {
                    $(".sameAsPrevSpeaker").attr('checked', 'checked');
                    allInputs.attr('disabled', 'disabled');
                    SameAsPrevSpeaker = true;
                }
            }
            if (AgendaItemID != 0) {
                jQuery.ajax({
                    cache: false,
                    type: 'post',
                    url: 'EditSessionHandler.ashx',
                    data: {
                        funcname: 'DoNext',
                        AgendaItemID: AgendaItemID,
                        AttachID: AttachID,
                        VoteID: VoteID,
                        tpcid: TopicID,
                        SpeakerID: SpeakerID,
                        SpeakerName: SpeakerName,
                        SameAsPrevSpeaker: SameAsPrevSpeaker,
                        MergeWithPrevTopic: MergeWithPrevTopic,
                        IsSessionPresident: IsSessionPresident,
                        IsGroupSubAgendaItems: IsGroupSubAgendaItems,
                        Ignored: Ignored,
                        SpeakerJob: SpeakerJob,
                        Text: Text,
                        Comments: Comments,
                        Footer: Footer,
                        lastItem: 0
                    },
                    dataType: 'json',
                    success: function(response) {
                        prevAgendaItemIndex = AgendaItemID;
                        prevSpeakerIndex = SpeakerID;
                        prevSpeakerTitle = SpeakerJob;
                        prevSpeakerImgUrl = SpeakerImgUrl;
                        prevSpeakerJob = $("#MainContent_txtSpeakerJob").html();
                        prevFragOrder = currentOrder.val();
                        BindData(response, 1);
                        nextAndprev({
                            ed: ed,
                            response: response
                        });
                    },
                    error: function() {
                        alert("لقد حدث خطأ");
                        $(".next").removeAttr("disabled");
                        ed.setProgressState(0);
                        allInputs.removeAttr('disabled');
                    }
                });
            }
        }
    });

    function htmlEncode(value) {
        return $('<div/>').text(value).html();
    }

    // SPLIT ACTION
    $(".split").click(function() {
        // VARS
        var ed = $('#MainContent_elm1').tinymce();
        // get Cursor Position
        getCursorPosition(ed, function(OB) {
            if (!ed.getContent().length) {
                alert('There is no text cut from');
            } else if (OB.collapsed) {
                // Restore the selection bookmark
                ed.selection.moveToBookmark(OB.bm);
                // select where the cursor is
                var range = ed.selection.getRng();
                // target element
                var $rangeStart = OB.$target;
                var rangeStart = OB.target;
                // check if the start not the body
                if (rangeStart.nodeName == 'BODY' || range.startOffset == OB.$target.text().length) {
                    if (OB.nextSibling) {
                        $rangeStart = $(OB.nextSibling);
                        rangeStart = $rangeStart[0];
                    } else {
                        alert('There is no text cut from');
                        return;
                    }
                } else if (rangeStart.nodeName == 'SPAN') { // IF tagename is SPAN
                    // Check if there is more than one word
                    var words = $rangeStart.html().split(' ').filter(function(n) {
                        return n != ''
                    });
                    var wordsLength = words.length;
                    // if there is more than word in this span
                    if (wordsLength > 1) {
                        // CHECK THE SELECTION START
                        var selectedWords = [];
                        var selectedWordsLength = 0;
                        for (var index = 0; index < wordsLength; index++) {
                            // vars
                            selectedWordsLength += words[index].length + 1;
                            // check the selected words
                            if (selectedWordsLength > range.startOffset + 1) {
                                selectedWords.push(words[index]);
                            }
                        }
                        // remove the words
                        $rangeStart.html($rangeStart.html().replace(selectedWords.join(' '), ''));
                        // add the new words in new span
                        var newSpanContent = (selectedWords.length == 1) ? ' ' + selectedWords.join(' ') : selectedWords.join(' ') + ' ';
                        var $newSpan = $rangeStart.clone(false).html(newSpanContent);
                        $rangeStart.after($newSpan);
                        // select the new start
                        $rangeStart = $newSpan;
                        rangeStart = $rangeStart[0];
                    }
                }
                // last element
                var $lastElement = $rangeStart.nextAll().andSelf().last();
                var lastElement = $lastElement[0];
                // select the html
                range.setStart(rangeStart, 0);
                range.setEnd(lastElement, 1);
                ed.selection.setRng(range);
                // get the selection and split
                var selectedContent = ed.selection.getContent();
                // check if there is any tag
                if (!$(selectedContent).length) {
                    var cloneRangeStart = $rangeStart.clone().html(selectedContent);
                    selectedContent = cloneRangeStart[0].outerHTML;
                }
                // check if the user selected the whole text or not
                if (($(selectedContent).length >= $(ed.getContent()).length) || ($(selectedContent).text().length >= $(ed.getContent()).text().length)) {
                    // alert the user
                    alert('You selected the whole text');
                    // deselect
                    ed.selection.collapse(true);
                    return;
                }
                // check if the form is valid
                if ($("#editSessionFileForm").valid()) {
                    // split action
                    splitAction(selectedContent);
                } else {
                    // deselect
                    ed.selection.collapse(true);
                    return;
                }
            } else {
                alert('Without any selection please !!')
            }
        });
    });

    $(".close_btn").click(function() {
        $(".popupoverlay").hide();
        $(".reviewpopup_cont").hide();
        $(".reviewpopup_cont-st1").hide();
    });

    $(".approve1").click(function() {
        if ($("textarea.splittinymce", '.reviewpopup_cont1').val() == '') {
            alert("لا يمكن القطع إلا بوجود نص");

            $(".popupoverlay").hide();
            $(".reviewpopup_cont1").hide();
            return;
        }
        splitAction($("textarea.splittinymce", '.reviewpopup_cont1').val());
    });

    $(".ddlOtherTitles").change(function() {
        var selectID = $(".ddlOtherTitles option:selected").attr("value");
        if (selectID == "5" || selectID == "6") {
            $(".ddlCommittee").show().val(0);
        } else {
            $(".ddlCommittee").hide();
        }

        if (selectID != "0" && selectID != "4" && selectID != "7" && selectID != "8") {
            $(".txtSpeakerOtherJob").val($(".ddlOtherTitles option:selected").text());
        } else {
            $(".txtSpeakerOtherJob").val("");
        }
    });

    $(".ddlCommittee").change(function() {
        var selecText = $("#MainContent_ddlOtherTitles  option:selected").text() + $(".ddlCommittee option:selected").text().replace(/لجنة/g, "");
        $(".txtSpeakerOtherJob").val(selecText);
    });


    // split function
    function splitAction(selectedHtml) {
        //  if (confirm("هل أنت متأكد من أنك تريد قطع النص الحالي؟ .. هذه الخطوة لا يمكن الرجوع فيها")) {

        // cut
        tinyMCE.execCommand('mceReplaceContent', false, '');
        // vars
        var currentFileID = getParameterByName("sfid");
        // call the ajax call
        jQuery.ajax({
            cache: false,
            type: 'post',
            url: 'EditSessionHandler.ashx',
            data: {
                funcname: 'SplitItem',
                FRAGORDER: $(".hdcurrentOrder").val(),
                XMLPATH: $(".hdxmlFilePath").val(),
                SPLITTEDTEXT: htmlEncode(selectedHtml),
                sfid: currentFileID,
            },
            dataType: 'json',
            success: function(response) {
                // next action
                $(".next").removeAttr("disabled").triggerHandler('click');
            },
            error: function() {
                alert("لقد حدث خطأ");

                ed.setProgressState(0);
                $(".prev").removeAttr("disabled");
            }
        });
        $(".popupoverlay").hide();
        $(".reviewpopup_cont1").hide();
        //  }
    }
    // split function
    function splitActionForManagePoint(selectedHtml) {
        // vars
        var currentFileID = getParameterByName("sfid");
        // call the ajax call
        jQuery.ajax({
            cache: false,
            type: 'post',
            url: 'EditSessionHandler.ashx',
            data: {
                funcname: 'SplitItem',
                FRAGORDER: $(".hdcurrentOrder").val(),
                XMLPATH: $(".hdxmlFilePath").val(),
                SPLITTEDTEXT: htmlEncode(selectedHtml),
                sfid: currentFileID,
            },
            dataType: 'json',
            success: function(response) {
                // next action
                $(".sameAsPrevSpeaker").removeAttr('checked');
                $(".isSessionPresident").removeAttr('checked');
				$("#MainContent_txtSpeakerOtherJob").val('');
                $(".next").removeAttr("disabled").triggerHandler('click');
            },
            error: function() {
                alert("لقد حدث خطأ");

                ed.setProgressState(0);
                $(".prev").removeAttr("disabled");
            }
        });
    }

    // previous button onclick
    $(".prev").click(function() {
        //if ($("#editSessionFileForm").valid()) {
        $(".prev").attr("disabled", "disabled");
        var PrevContentID = $("#MainContent_CurrentItemID").val() != "0" ? $("#MainContent_CurrentItemID").val() : "";
        // var AgendaItemID = $("#MainContent_ddlAgendaItems > option:selected").length > 0 ? $("#MainContent_ddlAgendaItems > option:selected").attr("value") : "";
        //var AgendaSubItemID = $("#MainContent_ddlAgendaSubItems > option:selected").length > 0 ? $("#MainContent_ddlAgendaSubItems > option:selected").attr("value") : "";
        var AgendaItemID = $('.agendaItemId').val();
        var AttachID = $('.attachId').val();
        var TopicID = $('.topicId').val();
        var VoteID = $('.voteId').val();
        var SpeakerID = $("#MainContent_ddlSpeakers").val();
        SpeakerID = SpeakerID == 0 ? $("#MainContent_ddlSpeakers option:contains(" + "غير محدد" + ")").attr('selected', 'selected').val() : SpeakerID;
        SpeakerID = SpeakerID == 1.5 ? -1 : SpeakerID;
        var SpeakerName = $("#MainContent_txtNewSpeaker").val();
        var IsSessionPresident = $(".isSessionPresident").is(':checked') ? "1" : "0";
        var SameAsPrevSpeaker = $(".sameAsPrevSpeaker").is(':checked');
        var MergeWithPrevTopic = $(".chkTopic").is(':checked');
        var IsGroupSubAgendaItems = $(".chkGroupSubAgendaItems").is(':checked');
        var Ignored = $(".chkIgnoredSegment").is(':checked');
        var SpeakerJob = $("#MainContent_txtSpeakerOtherJob").val();
        // editor value
        var clone = $('<div>').append($("#MainContent_elm1").attr("value"))
        clone.find('span').removeClass('highlight editable hover')
        var Text = encodeURI(clone.html())
        // comments value
        var Comments = $("#MainContent_txtComments").val();
        var Footer = $("#MainContent_txtFooter").val();
        $(".addingNewAgendaItem").show();

        // Show progress
        var ed = $('#MainContent_elm1').tinymce()
        ed.setProgressState(1);
        // pause the player
        $("#jquery_jplayer_1").jPlayer("pause");
        //
        jQuery.ajax({
            cache: false,
            type: 'post',
            url: 'EditSessionHandler.ashx',
            data: {
                funcname: 'DoPrevious',
                PrevContentID: PrevContentID,
                AgendaItemID: AgendaItemID,
                AttachID: AttachID,
                VoteID: VoteID,
                tpcid: TopicID,
                // AgendaSubItemID: AgendaSubItemID,
                SpeakerID: SpeakerID,
                SpeakerName: SpeakerName,
                SameAsPrevSpeaker: SameAsPrevSpeaker,
                MergeWithPrevTopic: MergeWithPrevTopic,
                IsSessionPresident: IsSessionPresident,
                IsGroupSubAgendaItems: IsGroupSubAgendaItems,
                Ignored: Ignored,
                SpeakerJob: SpeakerJob,
                Text: Text,
                Comments: Comments,
                Footer: Footer,
                lastItem: 0
            },
            dataType: 'json',
            success: function(response) {
                prevAgendaItemIndex = response.prevAgendaItemID; // Item.AgendaItemID;
                //prevAgendaSubItemIndex = response.prevAgendaSubItemID; // Item.AgendaSubItemID;
                prevSpeakerIndex = response.prevAttendantID; // Item.AttendantID;
                prevSpeakerTitle = response.prevAttendantJobTitle; // Item.CommentOnAttendant;
                prevSpeakerImgUrl = response.AttendantAvatar;
                prevSpeakerJob = response.AttendantJobTitle;
                BindData(response, 2);
                //prev clicked and no prev content item exist in db
                if (response.prevAgendaItemID == null)
                    $('.sameAsPrevSpeaker').attr('disabled', 'disabled');
                nextAndprev({
                    ed: ed,
                    response: response
                });
            },
            error: function() {
                alert("لقد حدث خطأ");

                ed.setProgressState(0);
                $(".prev").removeAttr("disabled");
            }
        });
        //  }
    });

    function nextAndprev(o) {
        // remove the loading
        o.ed.setProgressState(0)
        // remove undo level
        o.ed.undoManager.clear();
        o.ed.undoManager.add();
        var AudioPlayer = $("#jquery_jplayer_1");
        // pause player
        AudioPlayer.jPlayer("stop").jPlayer("play");
        // reset the caret position
        o.ed.selection.select(o.ed.selection.getStart());
        o.ed.selection.collapse(true);
        // $($('textarea.tinymce').html(),'span.segment').last().attr('data-stime')
        // AudioPlayer.jPlayer("pause", $($('textarea.tinymce').html(),'span.segment').last().attr('data-stime'));
        // remove the class to let the user seek the time
        // AudioPlayer.removeClass('playerStoppedBefore')
    }
    // bind data to controls
    function BindData(response, prevOrNext) {
        var isIgnored = 0
        if (prevOrNext == 1) {
            isIgnored = $(".chkIgnoredSegment").is(':checked') ? 1 : 0
        }
        if (response.ItemOrder == "last") {
            $('#MainContent_btnNext').attr('disabled', 'disabled');
        }

        $('#MainContent_ddlOtherTitles').val(0);
        $('#MainContent_ddlCommittee').val(0).hide();
        // update editor text
        if (response.Message == "success") {
            $(".btnAddProcuder").removeAttr('disabled', 'disabled');
            // update text controls value
            if (response.Item.ID != null && response.Item.ID != 0) {
                currentSessionContID.val(response.Item.ID);
            } else {
                currentSessionContID.val(-1);
            }
            $("#MainContent_elm1").val(response.Item.Text);
            $("#MainContent_CurrentItemID").val(response.FragOrderInXml);
            $("#MainContent_txtComments").val(response.Item.CommentOnText);
            $("#MainContent_txtFooter").val(response.Item.PageFooter);
            $("#MainContent_txtSpeakerJob").html(response.AttendantJobTitle);
            $("#MainContent_txtSpeakerOtherJob").val(response.Item.CommentOnAttendant);
            //$('#MainContent_ddlSpeakers-container').attr("title",response.AttendantAvatar);
            // $('#MainContent_ddlSpeakers-container').html(response.AttendantAvatar);
            $('#MainContent_imgSpeakerAvatar').attr("src",response.AttendantAvatar);
            // bind drop down lists
            var AgendaItem_SelectedID = $("#MainContent_ddlAgendaItems > option:selected").attr("value");
            var AgendaSubItem_SelectedID = $("#MainContent_ddlAgendaSubItems > option:selected").attr("value");
            var AgendaSubItem_html = $("#MainContent_ddlAgendaSubItems").html();
            var Speakers_SelectedID = $("#MainContent_ddlSpeakers").val();
            if (Speakers_SelectedID == 1.5) {
                var if_added_b4 = $("#MainContent_ddlSpeakers > option[value=" + response.SpeakerID + "]");
                if (if_added_b4.length == 0) {
                    //Add New Option
                    $('#MainContent_ddlSpeakers').append($('<option>', {
                        value: response.SpeakerID,
                        text: $('.txtNewSpeaker').val()
                    }));
                }

                //  $("#MainContent_ddlSpeakers > option:selected").removeAttr("selected");
                // $("#MainContent_ddlSpeakers > option[value=" + response.SpeakerID + "]").attr('selected', 'selected');
                //  $("#select2-MainContent_ddlSpeakers-container").html($('.txtNewSpeaker').val());
            }
            if (prevOrNext == 1 && Speakers_SelectedID == 1.5) {
                //  prevSpeakerTitle = $('.txtNewSpeaker').val();
                prevSpeakerIndex = response.Item.AttendantID;
                //        prevSpeakerTitle = SpeakerJob;
                //        prevSpeakerImgUrl = SpeakerImgUrl;
            }
            $('#MainContent_txtNewSpeaker').val('');
            $('#divNewSpeaker').hide();
            $('.agendaItemId').val(response.AgendaItemID);
            $('.attachId').val(response.AttachID);
            $('.voteId').val(response.VoteID);
            $('.topicId').val(response.TopicID);
            $('.prevTopicId').val(response.PrevTopicID);
            if(response.TopicID != "0" && response.TopicID != null)
            {
             $(".aPopupGetAttTopic").show();
            } 
            else  $(".aPopupGetAttTopic").hide();

            $('.agendaItemTxt').html(response.AgendaItemText);
            $('.agendaItemIsIndexed').val(response.AgendaItemIsIndexed);

            if (response.AgendaItemText == "غير معرف") {
                $('.divAgenda').hide();
            } else {
                $('.divAgenda').show();
            }
            if (response.AttachID == "0") {
                $('.divAttach').hide();
                $('.spanAttachTitle').html('');
            } else {
                $('.divAttach').show();
                $('.spanAttachTitle').html(response.AttachText);
            }
            if (response.VoteID == "0") {
                $('.divVote').hide();
                $('.spanVoteSubject').html('');
            } else {
                $('.divVote').show();
                $('.spanVoteSubject').html(response.VoteSubject);
            }
          //  $('.divTopic').hide();
            if (response.TopicID == "0" && response.PrevTopicID == "0") {
                $('.divTopic').hide();
                $(".chkTopic").removeAttr('checked');
                $(".chkTopic").attr('disabled','disabled');
                $(".aPopupGetAttTopic").hide();
                $(".btnAddNewTopic").removeAttr("disabled");
            } else  if (response.TopicID != "0" && response.PrevTopicID == "0")  {
                $('.divTopic').show();
                $(".chkTopic").removeAttr('checked');
                $(".chkTopic").attr('disabled','disabled');
                $(".aPopupGetAttTopic").show();
                $(".btnAddNewTopic").attr("disabled","disabled");
            }
            else  if (response.TopicID == "0" && response.PrevTopicID != "0")  {
                $('.divTopic').hide();
                $(".chkTopic").removeAttr('checked');
                $(".chkTopic").removeAttr('disabled');
                $(".aPopupGetAttTopic").hide();
                $(".btnAddNewTopic").removeAttr("disabled");
            }
            else  if (response.TopicID != "0" && response.PrevTopicID != "0")  {
                $('.divTopic').hide();
                if(!response.MergeWithPrevTopic){
                    $(".chkTopic").removeAttr('checked');
                    $(".btnAddNewTopic").attr("disabled","disabled");
                }
                else{
                    $(".chkTopic").attr('checked','checked');
                    $(".btnAddNewTopic").removeAttr("disabled");
                }
                if(response.TopicID != response.PrevTopicID )
                {
                   $('.divTopic').show();
                }
                $(".aPopupGetAttTopic").show();
                $(".chkTopic").removeAttr('disabled');
            }
            // set start and end time in hidden fields
            startTime.val(response.PargraphStartTime);
            endTime.val(response.PargraphEndTime);

            currentOrder.val(response.ItemFragOrder);

            // agenda items DDL
            $("#MainContent_ddlAgendaItems").html("");
            $("#MainContent_ddlAgendaItems").append($("<option selected='selected'></option>").val("0").html("-------- اختر البند --------"));
            $.each(response.AgendaItems, function() {

                $("#MainContent_ddlAgendaItems").append($("<option></option>").val(this['ID']).html(this['Text']).attr('IsGroupSubAgendaItems', this['IsGroupSubAgendaItems']));
            });
            //select option
            // $("#MainContent_ddlAgendaItems").val(

            if (response.Item.AgendaItemID != null && response.Item.AgendaItemID != 0) {
                $("#MainContent_ddlAgendaItems > option[value=" + response.Item.AgendaItemID + "]").attr('selected', 'selected');
            } else {
                $("#MainContent_ddlAgendaItems > option[value=" + AgendaItem_SelectedID + "]").attr('selected', 'selected');
            }
            // agenda sub items DDL
            $("#MainContent_ddlAgendaSubItems").html("");
            //$('#MainContent_ddlAgendaSubItems').removeAttr('disabled')
            if (response.Item.AgendaSubItemID != null && response.Item.AgendaSubItemID != 0) {
                $.each(response.AgendaSubItems, function() {
                    $("#MainContent_ddlAgendaSubItems").append($("<option></option>").val(this['ID']).html(this['Text']));
                });
                $("#MainContent_ddlAgendaSubItems > option[value=" + response.Item.AgendaSubItemID + "]").attr('selected', 'selected');
            } else {
                //25-03-2012 -- UN
                $("#MainContent_ddlAgendaSubItems").attr("disabled", "disabled");
                $("#MainContent_ddlAgendaSubItems").html(AgendaSubItem_html);
                $("#MainContent_ddlAgendaSubItems > option[value=" + AgendaSubItem_SelectedID + "]").attr('selected', 'selected');
            }
            // speaker DDL
            try {
                if (response.Item.AttendantID != null && response.Item.AttendantID != 0) {
                    $("#MainContent_ddlSpeakers > option:selected").removeAttr("selected");
                    $("#MainContent_ddlSpeakers > option[value=" + response.Item.AttendantID + "]").attr('selected', 'selected');
                    $("#select2-MainContent_ddlSpeakers-container").html($('#MainContent_ddlSpeakers :selected').text());
                    $("#select2-MainContent_ddlSpeakers-container").attr("title",$('#MainContent_ddlSpeakers :selected').text());
                    $('#MainContent_imgSpeakerAvatar').attr("src",response.AttendantAvatar +"?t=2");
            } else {
                    $("#MainContent_ddlSpeakers > option[value=" + Speakers_SelectedID + "]").attr('selected', 'selected');
                    $("#select2-MainContent_ddlSpeakers-container").html($('#MainContent_ddlSpeakers :selected').text());
                    $("#select2-MainContent_ddlSpeakers-container").attr("title",$('#MainContent_ddlSpeakers :selected').text());
                    $('#MainContent_imgSpeakerAvatar').attr("src",response.AttendantAvatar +"?t=3");
             }
            } catch (err) {}
            if ($("#MainContent_ddlSpeakers > option:selected").text() == "غير محدد") {
                $('#MainContent_ddlOtherTitles').val(0).attr('disabled', 'disabled');
                $('#MainContent_ddlCommittee').val(0).hide().attr('disabled', 'disabled');
                $('#MainContent_txtSpeakerOtherJob').val("").attr('disabled', 'disabled');
                $('#MainContent_imgSpeakerAvatar').attr("src","/images/AttendantAvatars/unknown.jpg?t=1");
                }

            // end binding DDL
            // to hide some controls base on current fragment order, i.e hide next when we are in the last fragment
            if (response.ItemOrder == "first") {
                $(".prev").attr("disabled", "disabled");
                allInputs.add(".next").removeAttr('disabled');
                $(".sameAsPrevSpeaker").attr("disabled", "disabled");
            } else if (response.ItemOrder == "last") {
                $(".sameAsPrevSpeaker,.prev,.finish,.btnPreview").add(allInputs).removeAttr("disabled");
                $(".next").attr("disabled", "disabled");
            } else {
                $(".next,.prev").removeAttr("disabled");
                $(".sameAsPrevSpeaker").removeAttr("disabled"); //kill same as
            }

            if (response.Ignored) {
                $(".chkIgnoredSegment").attr('checked', 'checked');
            } else {
                $('.chkIgnoredSegment').removeAttr('checked');
            }

            // alert('response.IsGroupSubAgendaItems ' + response.IsGroupSubAgendaItems);

            //usama march                
            if (response.IsGroupSubAgendaItems == true) {
                $('.chkGroupSubAgendaItems').attr('checked', 'checked');
                $("#MainContent_ddlAgendaSubItems").attr('disabled', 'disabled');

            } else if (response.IsGroupSubAgendaItems != null) {
                $('.chkGroupSubAgendaItems').removeAttr('checked');
                $("#MainContent_ddlAgendaSubItems").removeAttr('disabled');
            }
            //if not the same speaker
            if (!response.SameAsPrevSpeaker) //&& response.Item.AttendantID == null) {
            {
                allInputs.removeAttr('disabled');
                $('.sameAsPrevSpeaker').removeAttr('disabled').removeAttr('checked');
            } else { // 
                $('.sameAsPrevSpeaker').attr('checked', 'checked');
                allInputs.attr('disabled', 'disabled');
            }
            if (response.IsSessionPresident == "0") //&& response.Item.AttendantID == null) {
            {
                $('.isSessionPresident').removeAttr('checked');
            } else {
                $('.isSessionPresident').attr('checked', 'checked');
            }
            if (response.Item.AttendantID == 0) { //if data is from xml, so initialized the speaker
                $("#MainContent_ddlSpeakers").val(0).trigger("change");
                $("#select2-MainContent_ddlSpeakers-container").html($('#MainContent_ddlSpeakers :selected').text());
                allInputs.removeAttr('disabled');

                //usama march
                if ($(".chkGroupSubAgendaItems").is(':checked')) {
                    $("#MainContent_ddlAgendaSubItems").attr('disabled', 'disabled');
                }

                $('.sameAsPrevSpeaker').removeAttr('disabled');
                $('.sameAsPrevSpeaker').removeAttr('checked');
                $('.isSessionPresident').removeAttr('checked');
                $('.chkIgnoredSegment').removeAttr('checked');
            }
            mode = 1;
            if ($(".hdPageMode").val().length == 0) {
                mode = "1";
            } else {
                mode = $(".hdPageMode").val();
            }

            if (mode == 3) {
                $('.finish').removeAttr('disabled');
                $('.btnPreview').removeAttr('disabled');

                $('.btnSaveOnly').attr('disabled', 'disabled');
                $('.btnSaveAndExit').attr('disabled', 'disabled');
            }
            if (prevOrNext == 1 && isIgnored == 1) {
                $(".sameAsPrevSpeaker").attr("disabled", "disabled");
            } else {
                $(".sameAsPrevSpeaker").removeAttr("disabled");
            }

            if ($("#MainContent_chkGroupSubAgendaItems").is(':checked'))
                $("#MainContent_ddlAgendaSubItems").attr('disabled', 'disabled');
        } else {
            //alert("عفواً . غير مسموح لك بالتعديل في هذه الصفحة");

            $(".prev,.next").removeAttr("disabled");
        }
    }
    // save and exit button onclick
    $(".btnSaveAndExit").click(function() {
        if ($("#editSessionFileForm").valid()) {
            var mode = "1";
            var sessionContentItem;
            if ($(".hdPageMode").val().length == 0) {
                mode = "1";
            } else {
                mode = $(".hdPageMode").val();
                sessionContentItem = $(".hdSessionContentItemID").val();
            }
            var sessionID = $(".sessionID").val();

            //  var AgendaItemID = $("#MainContent_ddlAgendaItems > option:selected").length > 0 ? $("#MainContent_ddlAgendaItems > option:selected").attr("value") : "";
            //  var AgendaSubItemID = $("#MainContent_ddlAgendaSubItems > option:selected").length > 0 ? $("#MainContent_ddlAgendaSubItems > option:selected").attr("value") : "";
            var AgendaItemID = $('.agendaItemId').val();
            var AttachID = $('.attachId').val();
            var VoteID = $('.voteId').val();
            var TopicID = $('.topicId').val();
            var SpeakerID = $("#MainContent_ddlSpeakers > option:selected").val();
            SpeakerID = SpeakerID == 1.5 ? -1 : SpeakerID;
            var SpeakerName = $("#MainContent_txtNewSpeaker").val();
            var SameAsPrevSpeaker = $(".sameAsPrevSpeaker").is(':checked');
            var MergeWithPrevTopic = $(".chkTopic").is(':checked');
            var IsSessionPresident = $(".isSessionPresident").is(':checked') ? "1" : "0";
            var IsGroupSubAgendaItems = $(".chkGroupSubAgendaItems").is(':checked');
            var Ignored = $(".chkIgnoredSegment").is(':checked');
            var SpeakerJob = $("#MainContent_txtSpeakerOtherJob").val();
            var Text = encodeURI($("#MainContent_elm1").attr("value"));
            var Comments = $("#MainContent_txtComments").val();
            var Footer = $("#MainContent_txtFooter").val();
            var lastItem = $(".finish").is(":disabled") ? 0 : 1;
            if (SameAsPrevSpeaker == false && !$(".chkIgnoredSegment").is(':checked') && prevSpeakerIndex == SpeakerID) {
                $(".sameAsPrevSpeaker").attr('checked', 'checked');
                $("#MainContent_ddlAgendaItems,#MainContent_ddlAgendaSubItems,#MainContent_ddlSpeakers,#MainContent_txtSpeakerJob,#specialBranch").attr('disabled', 'disabled');
                SameAsPrevSpeaker = true;
            }
            $(".btnSaveAndExit").attr("disabled", "disabled");
            if (AgendaItemID != 0) {
                jQuery.ajax({
                    cache: false,
                    type: 'post',
                    url: 'EditSessionHandler.ashx',
                    data: {
                        funcname: 'SaveAndExit',
                        AgendaItemID: AgendaItemID,
                        AttachID: AttachID,
                        VoteID: VoteID,
                        tpcid: TopicID,
                        SpeakerID: SpeakerID,
                        SpeakerName: SpeakerName,
                        SameAsPrevSpeaker: SameAsPrevSpeaker,
                        MergeWithPrevTopic: MergeWithPrevTopic,
                        IsSessionPresident: IsSessionPresident,
                        IsGroupSubAgendaItems: IsGroupSubAgendaItems,
                        Ignored: Ignored,
                        SpeakerJob: SpeakerJob,
                        Text: Text,
                        Comments: Comments,
                        Footer: Footer,
                        editmode: mode,
                        scid: sessionContentItem,
                        lastItem: lastItem
                    },
                    dataType: 'json',
                    success: function(response) {
                        if (response.Message == "success") {
                            $(".btnSaveAndExit").removeAttr("disabled");
                            if (mode == "2")
                                window.location = "ReviewNotes.aspx?sid=" + sessionID;
                            else
                            if (mode == "3")
                                window.location = "Review.aspx?sid=" + sessionID + "#scid_" + sessionContentItem;
                            else
                                window.location = "default.aspx";
                        } else {
                            alert("لقد حدث خطأ");

                            allInputs.removeAttr('disabled');
                        }
                    },
                    error: function() {
                        alert("لقد حدث خطأ");

                        allInputs.removeAttr('disabled');
                    }
                });
            }
        }
    });

    // save only button onclick
    $(".btnSaveOnly").click(function() {
        // if ($("#editSessionFileForm").valid()) {
        var mode = "1";
        var sessionContentItem;
        if ($(".hdPageMode").val().length == 0) {
            mode = "1";
        } else {
            mode = $(".hdPageMode").val();
            sessionContentItem = $(".hdSessionContentItemID").val();
        }
        var sessionID = $(".sessionID").val();

        //  var AgendaItemID = $("#MainContent_ddlAgendaItems > option:selected").length > 0 ? $("#MainContent_ddlAgendaItems > option:selected").attr("value") : "";
        //  var AgendaSubItemID = $("#MainContent_ddlAgendaSubItems > option:selected").length > 0 ? $("#MainContent_ddlAgendaSubItems > option:selected").attr("value") : "";
        var AgendaItemID = $('.agendaItemId').val();
        var AttachID = $('.attachId').val();
        var VoteID = $('.voteId').val();
        var TopicID = $('.topicId').val();

        var SpeakerID = $("#MainContent_ddlSpeakers").val();
        SpeakerID = SpeakerID == 0 ? $("#MainContent_ddlSpeakers option:contains(" + "غير محدد" + ")").attr('selected', 'selected').val() : SpeakerID;
        SpeakerID = SpeakerID == 1.5 ? -1 : SpeakerID;
        // $("#MainContent_ddlSpeakers").val(SpeakerID);
        //$("#select2-MainContent_ddlSpeakers-container").html($('#MainContent_ddlSpeakers :selected').text());

        var SpeakerName = $("#MainContent_txtNewSpeaker").val();
        var SameAsPrevSpeaker = $(".sameAsPrevSpeaker").is(':checked');
        var MergeWithPrevTopic = $(".chkTopic").is(':checked');
        var IsSessionPresident = $(".isSessionPresident").is(':checked') ? "1" : "0";
        var IsGroupSubAgendaItems = $(".chkGroupSubAgendaItems").is(':checked');
        var Ignored = $(".chkIgnoredSegment").is(':checked');
        var SpeakerJob = $("#MainContent_txtSpeakerOtherJob").val();
        var Text = encodeURI($("#MainContent_elm1").attr("value"));
        var Comments = $("#MainContent_txtComments").val();
        var Footer = $("#MainContent_txtFooter").val();

        //$("#MainContent_ddlSpeakers option:contains(" + "أخرى" + ")").val() != SpeakerID &&
        if (SameAsPrevSpeaker == false && !$(".chkIgnoredSegment").is(':checked') && prevSpeakerIndex == SpeakerID) {
            $(".sameAsPrevSpeaker").attr('checked', 'checked');
            $("#MainContent_ddlAgendaItems,#MainContent_ddlAgendaSubItems,#MainContent_ddlSpeakers,#MainContent_txtSpeakerJob,#specialBranch").attr('disabled', 'disabled');
            SameAsPrevSpeaker = true;
        }

        var lastItem = $(".finish").is(":disabled") ? 0 : 1;
        $(".btnSaveOnly").attr("disabled", "disabled");
        if (AgendaItemID != 0) {
            jQuery.ajax({
                cache: false,
                type: 'post',
                url: 'EditSessionHandler.ashx',
                data: {
                    funcname: 'SaveOnly',
                    AgendaItemID: AgendaItemID,
                    AttachID: AttachID,
                    VoteID: VoteID,
                    tpcid: TopicID,
                    SpeakerID: SpeakerID,
                    SpeakerName: SpeakerName,
                    SameAsPrevSpeaker: SameAsPrevSpeaker,
                    MergeWithPrevTopic: MergeWithPrevTopic,
                    IsSessionPresident: IsSessionPresident,
                    IsGroupSubAgendaItems: IsGroupSubAgendaItems,
                    Ignored: Ignored,
                    SpeakerJob: SpeakerJob,
                    Text: Text,
                    Comments: Comments,
                    Footer: Footer,
                    editmode: mode,
                    scid: sessionContentItem,
                    lastItem: lastItem
                },
                dataType: 'json',
                success: function(response) {
                    if (response.Message == "success") {
                        $(".btnSaveOnly").removeAttr("disabled");
                        var Speakers_SelectedID = $("#MainContent_ddlSpeakers").val();
                        //alert(Speakers_SelectedID);
                        if (Speakers_SelectedID == 1.5) {
                            var if_added_b4 = $("#MainContent_ddlSpeakers > option[value=" + response.SpeakerID + "]");
                            if (if_added_b4.length == 0) {
                                //Add New Option
                                $('#MainContent_ddlSpeakers').append($('<option>', {
                                    value: response.SpeakerID,
                                    text: $('.txtNewSpeaker').val()
                                }));
                            }

                            //  $("#MainContent_ddlSpeakers > option:selected").removeAttr("selected");
                            // $("#MainContent_ddlSpeakers > option[value=" + response.SpeakerID + "]").attr('selected', 'selected');
                            //  $("#select2-MainContent_ddlSpeakers-container").html($('.txtNewSpeaker').val());
                        }
                        $('#MainContent_txtNewSpeaker').val('');
                        $('#divNewSpeaker').hide();

                        try {
                            if (response.Item.AttendantID != null && response.Item.AttendantID != 0) {
                                $("#MainContent_ddlSpeakers > option:selected").removeAttr("selected");
                                $("#MainContent_ddlSpeakers > option[value=" + response.Item.AttendantID + "]").attr('selected', 'selected');
                                $("#select2-MainContent_ddlSpeakers-container").html($('#MainContent_ddlSpeakers :selected').text());
                            } else {
                                $("#MainContent_ddlSpeakers > option[value=" + Speakers_SelectedID + "]").attr('selected', 'selected');
                                $("#select2-MainContent_ddlSpeakers-container").html($('#MainContent_ddlSpeakers :selected').text());
                            }
                        } catch (err) {}
                        if ($("#MainContent_ddlSpeakers > option:selected").text() == "غير محدد") {
                            $('#MainContent_ddlOtherTitles').val(0).attr('disabled', 'disabled');
                            $('#MainContent_ddlCommittee').val(0).hide().attr('disabled', 'disabled');
                            $('#MainContent_txtSpeakerOtherJob').val("").attr('disabled', 'disabled');
                        }




                        alert("تم الحفظ بنجاح");
                    } else {
                        alert("لقد حدث خطأ");
                        $(".btnSaveOnly").removeAttr("disabled");
                        allInputs.removeAttr('disabled');
                    }
                },
                error: function() {
                    alert("لقد حدث خطأ");
                    $(".btnSaveOnly").removeAttr("disabled");
                    allInputs.removeAttr('disabled');
                }
            });
        }
        //}
    });

    function getParameterByName(name) {
        var match = RegExp('[?&]' + name + '=([^&]*)').exec(window.location.search);
        return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
    }
    // save only button onclick
    $(".btnPreview").click(function() {
        var mode = "1";
        var sessionContentItem;
        if ($(".hdPageMode").val().length == 0) {
            mode = "1";
        } else {
            mode = $(".hdPageMode").val();
            sessionContentItem = $(".hdSessionContentItemID").val();
        }
        var sessionID = $(".sessionID").val();
        var AgendaItemID = $('.agendaItemId').val();
        var AttachID = $('.attachId').val();
        var VoteID = $('.voteId').val();
        var TopicID = $('.topicId').val();

        var SpeakerID = $("#MainContent_ddlSpeakers").val();
        SpeakerID = SpeakerID == 0 ? $("#MainContent_ddlSpeakers option:contains(" + "غير محدد" + ")").attr('selected', 'selected').val() : SpeakerID;
        SpeakerID = SpeakerID == 1.5 ? -1 : SpeakerID;

        var SpeakerName = $("#MainContent_txtNewSpeaker").val();
        var SameAsPrevSpeaker = $(".sameAsPrevSpeaker").is(':checked');
        var MergeWithPrevTopic = $(".chkTopic").is(':checked');
        var IsSessionPresident = $(".isSessionPresident").is(':checked') ? "1" : "0";
        var IsGroupSubAgendaItems = $(".chkGroupSubAgendaItems").is(':checked');
        var Ignored = $(".chkIgnoredSegment").is(':checked');
        var SpeakerJob = $("#MainContent_txtSpeakerOtherJob").val();
        var Text = encodeURI($("#MainContent_elm1").attr("value"));
        var Comments = $("#MainContent_txtComments").val();
        var Footer = $("#MainContent_txtFooter").val();

        //$("#MainContent_ddlSpeakers option:contains(" + "أخرى" + ")").val() != SpeakerID &&
        if (SameAsPrevSpeaker == false && !$(".chkIgnoredSegment").is(':checked') && prevSpeakerIndex == SpeakerID) {
            $(".sameAsPrevSpeaker").attr('checked', 'checked');
            $("#MainContent_ddlAgendaItems,#MainContent_ddlAgendaSubItems,#MainContent_ddlSpeakers,#MainContent_txtSpeakerJob,#specialBranch").attr('disabled', 'disabled');
            SameAsPrevSpeaker = true;
        }

        var lastItem = $(".finish").is(":disabled") ? 0 : 1;
        $(".btnPreview").attr("disabled", "disabled");
        if (AgendaItemID != 0) {
            jQuery.ajax({
                cache: false,
                type: 'post',
                url: 'EditSessionHandler.ashx',
                data: {
                    funcname: 'SaveOnly',
                    AgendaItemID: AgendaItemID,
                    AttachID: AttachID,
                    VoteID: VoteID,
                    tpcid: TopicID,
                    SpeakerID: SpeakerID,
                    SpeakerName: SpeakerName,
                    SameAsPrevSpeaker: SameAsPrevSpeaker,
                    MergeWithPrevTopic: MergeWithPrevTopic,
                    IsSessionPresident: IsSessionPresident,
                    IsGroupSubAgendaItems: IsGroupSubAgendaItems,
                    Ignored: Ignored,
                    SpeakerJob: SpeakerJob,
                    Text: Text,
                    Comments: Comments,
                    Footer: Footer,
                    editmode: mode,
                    scid: sessionContentItem,
                    lastItem: lastItem
                },
                dataType: 'json',
                success: function(response) {
                    if (response.Message == "success") {
                        $(".btnPreview").removeAttr("disabled");
                        window.location = "PreReview.aspx?sfid=" + getParameterByName("sfid") + "#scid_" + $(".hdSessionContentItemID").val();
                    } else {
                        alert("لقد حدث خطأ");
                        $(".btnSaveOnly").removeAttr("disabled");
                        allInputs.removeAttr('disabled');
                    }
                },
                error: function() {
                    alert("لقد حدث خطأ");
                    $(".btnSaveOnly").removeAttr("disabled");
                    allInputs.removeAttr('disabled');
                }
            });
        }
        //}
    });

    // finish: save and exit button onclick
    $(".finish").click(function() {
        if ($("#editSessionFileForm").valid()) {
            var mode = "1";
            var sessionContentItem;
            if ($(".hdPageMode").val().length == 0) {
                mode = "1";
            } else {
                mode = $(".hdPageMode").val();
                sessionContentItem = $(".hdSessionContentItemID").val();
            }
            // var AgendaItemID = $("#MainContent_ddlAgendaItems > option:selected").length > 0 ? $("#MainContent_ddlAgendaItems > option:selected").attr("value") : "";
            // var AgendaSubItemID = $("#MainContent_ddlAgendaSubItems > option:selected").length > 0 ? $("#MainContent_ddlAgendaSubItems > option:selected").attr("value") : "";
            var AgendaItemID = $('.agendaItemId').val();
            var AttachID = $('.attachId').val();
            var VoteID = $('.voteId').val();
            var TopicID = $('.topicId').val();
            var SpeakerID = $("#MainContent_ddlSpeakers > option:selected").val();
            SpeakerID = SpeakerID == 1.5 ? -1 : SpeakerID;
            var SpeakerName = $("#MainContent_txtNewSpeaker").val();
            var SameAsPrevSpeaker = $(".sameAsPrevSpeaker").is(':checked');
            var MergeWithPrevTopic = $(".chkTopic").is(':checked');
            var IsSessionPresident = $(".isSessionPresident").is(':checked') ? "1" : "0";
            var IsGroupSubAgendaItems = $(".chkGroupSubAgendaItems").is(':checked');
            var Ignored = $(".chkIgnoredSegment").is(':checked');
            var SpeakerJob = $("#MainContent_txtSpeakerOtherJob").val();
            var Text = encodeURI($("#MainContent_elm1").attr("value"));
            var Comments = $("#MainContent_txtComments").val();
            var Footer = $("#MainContent_txtFooter").val();
            var sessionID = $(".sessionID").val();
            var lastItem = $(".finish").is(":disabled") ? 0 : 1;
            if (SameAsPrevSpeaker == false && !$(".chkIgnoredSegment").is(':checked') && prevSpeakerIndex == SpeakerID) {
                $(".sameAsPrevSpeaker").attr('checked', 'checked');
                $("#MainContent_ddlAgendaItems,#MainContent_ddlAgendaSubItems,#MainContent_ddlSpeakers,#MainContent_txtSpeakerJob,#specialBranch").attr('disabled', 'disabled');
                SameAsPrevSpeaker = true;
            }
            $(".finish").attr("disabled", "disabled");
            if (AgendaItemID != 0) {
                jQuery.ajax({
                    cache: false,
                    type: 'post',
                    url: 'EditSessionHandler.ashx',
                    data: {
                        funcname: 'UpdateSessionFileStatusCompleted',
                        AgendaItemID: AgendaItemID,
                        AttachID: AttachID,
                        VoteID: VoteID,
                        tpcid: TopicID,
                        SpeakerID: SpeakerID,
                        SpeakerName: SpeakerName,
                        SameAsPrevSpeaker: SameAsPrevSpeaker,
                        MergeWithPrevTopic: MergeWithPrevTopic,
                        IsSessionPresident: IsSessionPresident,
                        IsGroupSubAgendaItems: IsGroupSubAgendaItems,
                        Ignored: Ignored,
                        SpeakerJob: SpeakerJob,
                        Text: Text,
                        Comments: Comments,
                        Footer: Footer,
                        sid: sessionID,
                        lastItem: lastItem
                    },
                    dataType: 'json',
                    success: function(response) {
                        if (response.Message == "success") {
                            if (mode == "2")
                                window.location = "ReviewNotes.aspx?sid=" + sessionID;
                            else
                            if (mode == "3")
                                window.location = "Review.aspx?sid=" + sessionID + "#scid_" + sessionContentItem;
                            else
                                window.location = "default.aspx";
                        } else {
                            alert("لقد حدث خطأ");

                            allInputs.removeAttr('disabled');
                        }
                    },
                    error: function() {
                        alert("لقد حدث خطأ");

                        allInputs.removeAttr('disabled');
                    }
                });
            }
        }
    });

    // for the small window popup
    $('#specialBranch').click(function() {
        if ($(this).is(':checked')) {
            $('#smallwindow').show()
            $('select#MainContent_ddlAgendaSubItems').attr('disabled', 'disabled')
        } else {
            $('#smallwindow').hide()
            $('select#MainContent_ddlAgendaSubItems').removeAttr('disabled')
        }
    })
    // 
    $('#btnAddCustomAgendaItem').click(function() {
        if ($("#txtAgendaItem").valid()) {
            var itemText = $('#txtAgendaItem').val();
            var parentAgendaItemID = $("#MainContent_ddlAgendaItems").val();

            var dropdown = $("#MainContent_ddlAgendaSubItems");
            var isDuplicate = false;
            $("#MainContent_ddlAgendaSubItems > option").each(function() {
                if (jQuery.trim(this.text) == jQuery.trim(itemText)) {
                    isDuplicate = true;
                    alert("هذا البند الفرعي موجود .. لا يمكنك إضافة بندين بنفس الإسم");

                    return false;
                }
            });

            if (isDuplicate == true)
                return;
            else {
                jQuery.ajax({
                    cache: false,
                    type: 'post',
                    url: 'EditSessionHandler.ashx',
                    data: {
                        funcname: 'AddAgendaItem',
                        agendaitemtext: itemText,
                        sid: sessionID
                    },
                    dataType: 'json',
                    success: function(item_id) {
                        if (item_id != 0) {

                            $("#MainContent_ddlAgendaSubItems").removeAttr('disabled');
                            $('#smallwindow').hide();
                            $("#MainContent_ddlAgendaSubItems > option:selected").removeAttr('selected')
                            $("#MainContent_ddlAgendaSubItems").append($("<option selected='selected'></option>").val(item_id).html(itemText));
                            //$('select#MainContent_ddlAgendaSubItems').html("").attr('disabled', 'disabled');
                        } else {
                            alert("عفواً ... هذا البند تم تسجيله من قبل ")

                        }
                    },
                    error: function() {

                    }
                });
            }
        }
    });
    // add new job & edit button
    $('#addnewjobbutton').click(function(e) {
        if ($("#editSessionFile").valid()) {
            $('#newjobtitle').attr('class', 'done')
            // insert text
            var value = $('input[name=addnewjobtext]').val();
            $('#newjobtitle .donemode strong').html(value)
            // send the value
        }
        e.preventDefault()
    })
    $('#editnewjobbutton').click(function(e) {
        $('#newjobtitle').attr('class', 'edit')
        e.preventDefault()
    });
    // tinymce
    $('textarea.tinymce').tinymce({
        custom_undo_redo: true,
        // General options
        theme: "advanced",
        plugins: "pagebreak,directionality,noneditable,paste",
        paste_preprocess: function(pl, o) {
            getCursorPosition(this, function(OB) {
                // Content string containing the HTML from the clipboard
                var stripedContent = o.content.replace(/&nbsp;/g, ' ').replace(/(<([^>]+)>)/ig, '');
                var cleanedHTML = cleanHTML(stripedContent);
                o.content = '<span>' + cleanedHTML + '</span>'
            });
        },
        language: "ar",
        // direction
        directionality: "rtl",
        // clean up
        cleanup: true,
        cleanup_on_startup: true,
        width: '100%',
        height: 400,
        theme_advanced_source_editor_wrap: true,
        // Theme options
        theme_advanced_buttons1: "justifycenter,justifyright,|,undo,redo",
        theme_advanced_buttons2: "",
        theme_advanced_buttons3: "",
        theme_advanced_buttons4: "",
        theme_advanced_path: false,
        theme_advanced_toolbar_location: "top",
        theme_advanced_toolbar_align: "right",
        theme_advanced_resizing: false,
        // Example content CSS (should be your site CSS)
        content_css: "styles/tinymce_content.css",
        // invalid elements
        invalid_elements: "applet,body,button,caption,fieldset ,font,form,frame,frameset,head,,html,iframe,img,input,link,meta,object,option,param,script,select,style,table,tbody,tr,td,th,tbody,textarea,xmp",
        // valid elements
        valid_elements: "@[class],span[*],p[*],strong,em,blockquote,br,i[!id]",
        force_br_newlines: true,
        force_p_newlines: false,
        forced_root_block: false,
        cleanup_callback: function(type, value) {
            switch (type) {
                case "get_from_editor":
                    break;
                case "insert_to_editor":
                    // Clear empty tags
                    value = cleanHTML(value);
                    break;
                case "submit_content":
                    break;
                case "get_from_editor_dom":
                    break;
                case "insert_to_editor_dom":
                    break;
                case "setup_content_dom":
                    break;
                case "submit_content_dom":
                    break;
            }
            return value;
        },
        setup: function(ed) {
            // function to make the span editable
            function editableSpan(ed, e, higlightonly) {
                if (e) {
                    // remove all classes from the editor
                    $('span.editable', ed.contentDocument).removeClass('editable');
                    // add class editable
                    if (e.nodeName == 'SPAN') {
                        // add class editable
                        $(e).addClass('editable');
                        // check the flag
                        if (!higlightonly) {
                            // time from the span
                            var time = parseFloat($(e).attr('data-stime'))
                            // seek
                            $("#jquery_jplayer_1").jPlayer("pause", time);
                        }
                    }
                }
            }
            // click on text tinyMCE editor
            ed.onMouseUp.add(function(ed, e) {
                editableSpan(ed, e.target)
            });
            // on keys
            editorEvents(ed);
            // oninit
            ed.onInit.add(function(ed) {
                var AudioPlayer = $("#jquery_jplayer_1");
                // all span segments
                var all_spans_segments = $('span.segment', ed.contentDocument);
                // hover effect
                all_spans_segments.live("mouseover mouseout", function(event) {
                    if (event.type == "mouseover") {
                        // remove all classes
                        $(this).toggleClass('hover');
                    } else {
                        // remove hover class
                        $(this).removeClass('hover');
                    }
                });
                // jplayer
                var playertime;
                /*AudioPlayer.bind($.jPlayer.event.seeking, function(event) {
                    // Add a listener to report the time play began
                });*/
                AudioPlayer.jPlayer({
                    swfPath: "/scripts/jPlayer/",
                    wmode: "window",
                    solution: 'html, flash',
                    supplied: "mp3",
                    preload: 'metadata',
                    volume: 1,
                    cssSelectorAncestor: '#jp_container_1',
                    errorAlerts: false,
                    warningAlerts: false,
                    ready: function() {
                        // get start and end time in hidden fields
                        var firstTime = Math.floor(startTime.val()); //parseFloat($('span.segment:first', ed.contentDocument).attr('data-stime'));
                        // alert(Math.floor(startTime.val()));
                        // alert(Math.floor($('span.segment:first', ed.contentDocument).attr('data-stime')));
                        // play the jplayer
                        $(this).jPlayer("setMedia", {
                            mp3: $(".MP3FilePath").val() // mp3 file path
                        }).jPlayer("play", firstTime);
                        // next x seconds button
                        $('.jp-audio .next-jp-xseconds').click(function(e) {
                            var lastTime = Math.ceil(endTime.val()); //parseFloat($('span.segment:last', ed.contentDocument).attr('data-stime'));
                            if (!((playertime + 5) >= lastTime)) {
                                AudioPlayer.jPlayer("play", playertime + 5);
                            }
                        })
                        // prev x seconds button
                        $('.jp-audio .prev-jp-xseconds').click(function(e) {
                            if (!((playertime - 5) <= firstTime)) {
                                AudioPlayer.jPlayer("play", playertime - 5);
                            }
                        })
                    },
                    timeupdate: function(event) {
                        if (!$(this).data("jPlayer").status.paused) {
                            // all span segments
                            var all_spans_segments = $('span.segment', ed.contentDocument);
                            var firstTime = Math.floor(startTime.val());
                            //var firstTime = parseFloat($('span.segment:first', ed.contentDocument).attr('data-stime'));
                            var lastTime = Math.ceil(endTime.val()); //Math.ceil($('span.segment:last', ed.contentDocument).attr('data-stime'));//endTime.val() from hidden field
                            // remove all classes
                            all_spans_segments.removeClass('highlight editable');
                            // highlight the word by time
                            playertime = event.jPlayer.status.currentTime;
                            if (Math.ceil(playertime) > (lastTime + 1)) {
                                AudioPlayer.jPlayer('pause', firstTime);
                            } else if (playertime < firstTime) {
                                $(this).jPlayer('play', firstTime);
                            }
                            // check the time
                            var playerfixedTime = playertime.toFixed(2);
                            var playerfixedTimeString = playerfixedTime.toString();
                            var playerfixedTimeToArray = playerfixedTimeString.split('.');
                            // highlight the span
                            var highlight = all_spans_segments.filter('span.segment[data-stime^="' + playerfixedTimeToArray[0] + '."]');
                            if (highlight.length > 1) {
                                highlight = highlight.filter(function() {
                                    // get the nearest span
                                    var spanTime = $(this).attr('data-stime')
                                    var spanTimeToArray = spanTime.split('.');
                                    var spanfragment = spanTimeToArray[1];
                                    var playerfragment = playerfixedTimeToArray[1];
                                    if (playerfragment >= spanfragment) {
                                        return true;
                                    }
                                }).filter(':last');
                            }
                            // highlight
                            highlight.addClass('highlight')
                            if ($.browser.msie && $.browser.version == '9.0') {
                                if (Math.ceil(playertime) > lastTime || Math.ceil(playertime) < firstTime) {
                                    AudioPlayer.jPlayer('stop')
                                }
                            }
                        }
                    }
                });
                // jplayer shorcuts
                $(document).add(ed.dom.doc.body).bind('keydown', function(e) {
                    var k = e.keyCode;
                    if ($(e.target).find(':input,select').length) { // not input
                        if (k == 88 || k == 67 || k == 86 || k == 66) {
                            e.preventDefault();
                            return;
                        }
                    }
                    if (k == 116) {
                        window.location.href = window.location.href;
                    }
                }).bind('keydown', 'alt+t', function() {
                    // add agenda item
                    $(".addingNewAgendaItem").trigger('click')
                }).bind('keydown', 'alt+v', function() {
                    // add procedure
                    $(".btnAddProcuder").trigger('click')
                }).bind('keydown', 'alt+l', function() {
                    // add attach
                    $(".btnAssignAttachToContentItem").trigger('click')
                }).bind('keydown', 'alt+w', function() {
                    // add vote
                    $(".btnAddNewVote").trigger('click')
                }).bind('keydown', 'alt+s', function() {
                    // previous page
                    $(".btn.prev").trigger('click')
                }).bind('keydown', "alt+r", function() {
                    // previous page
                    $(".btn.split").trigger('click')
                }).bind('keydown', "alt+p", function() {
                    // previous page
                    $(".btn.btnSaveAndExit").trigger('click')
                }).bind('keydown', 'alt+k', function() {
                    // previous page
                    $(".btn.finish").trigger('click')
                }).bind('keydown', 'alt+u', function() {
                    // previous page
                    $(".btn.btnPreview").trigger('click')
                }).bind('keydown', 'alt+a', function() {
                    // play & pause player
                    if (AudioPlayer.data("jPlayer").status.paused) {
                        AudioPlayer.jPlayer("play");
                    } else {
                        AudioPlayer.jPlayer("pause");
                    }
                }).bind('keydown', 'alt+q', function() {
                    // stop player
                    AudioPlayer.jPlayer("stop");
                }).bind('keydown', 'alt+j', function() {
                    // next page
                    $(".btn.next").trigger('click')
                }).bind('keydown', 'alt+i', function() {
                    // next x seconds
                    $('.jp-audio .next-jp-xseconds').trigger('click')
                }).bind('keydown', 'alt+o', function() {
                    // prev x seconds
                    $('.jp-audio .prev-jp-xseconds').trigger('click')
                })
            });
        }
    });

    // tinymce for the popup window
    var defaultOptions = {
        custom_undo_redo: true,
        // General options
        theme: "advanced",
        plugins: "pagebreak,directionality,noneditable,paste",
        paste_preprocess: function(pl, o) {
            getCursorPosition(this, function(OB) {
                // Content string containing the HTML from the clipboard
                var stripedContent = o.content.replace(/&nbsp;/g, ' ').replace(/(<([^>]+)>)/ig, '');
                var cleanedHTML = cleanHTML(stripedContent);
                o.content = '<span>' + cleanedHTML + '</span>'
            });
        },
        language: "ar",
        // direction
        directionality: "rtl",
        // clean up
        cleanup: true,
        cleanup_on_startup: true,
        width: '100%',
        height: 400,
        theme_advanced_source_editor_wrap: true,
        // Theme options
        theme_advanced_buttons1: "justifycenter,justifyright,|,undo,redo",
        theme_advanced_buttons2: "",
        theme_advanced_buttons3: "",
        theme_advanced_buttons4: "",
        theme_advanced_path: false,
        theme_advanced_toolbar_location: "top",
        theme_advanced_toolbar_align: "right",
        theme_advanced_resizing: false,
        // Example content CSS (should be your site CSS)
        content_css: "styles/tinymce_content.css",
        // invalid elements
        invalid_elements: "applet,body,button,caption,fieldset ,font,form,frame,frameset,head,,html,iframe,img,input,link,meta,object,option,param,script,select,style,table,tbody,tr,td,th,tbody,textarea,xmp",
        // valid elements
        valid_elements: "@[class],span[*],p[*],strong,em,blockquote,br,i[!id]",
        force_br_newlines: true,
        force_p_newlines: false,
        forced_root_block: false,
        cleanup_callback: function(type, value) {
            switch (type) {
                case "get_from_editor":
                    break;
                case "insert_to_editor":
                    // Clear empty tags
                    value = cleanHTML(value);
                    break;
                case "submit_content":
                    break;
                case "get_from_editor_dom":
                    break;
                case "insert_to_editor_dom":
                    break;
                case "setup_content_dom":
                    break;
                case "submit_content_dom":
                    break;
            }
            return value;
        },
        setup: function(ed) {
            // on keypress
            editorEvents(ed);
        }
    };

    // get where the cursor position
    function getCursorPosition(ed, callBack, mv) {
        ed.undoManager.add();
        // vars
        var target;
        var objects = {};
        // Stores a bookmark of the current selection
        var bm = ed.selection.getBookmark();
        // get the mark
        objects.bm = bm;
        objects.$mark = $(ed.getBody()).find('#' + bm.id + '_start');
        objects.mark = objects.$mark[0];
        if (mv) {
            objects.markNextSibling = (objects.mark.nextElementSibling) ? $.clone(objects.mark.nextElementSibling) : objects.mark.nextElementSibling;
            objects.markPreviousSibling = (objects.mark.previousElementSibling) ? $.clone(objects.mark.previousElementSibling) : objects.mark.previousElementSibling;
            objects.markAcNextSibling = (objects.mark.nextSibling) ? $.clone(objects.mark.nextSibling) : objects.mark.nextSibling;
            objects.markAcPreviousSibling = (objects.mark.previousSibling) ? $.clone(objects.mark.previousSibling) : objects.mark.previousSibling;
        } else {
            objects.markNextSibling = (objects.mark.nextElementSibling);
            objects.markPreviousSibling = (objects.mark.previousElementSibling);
            objects.markAcNextSibling = (objects.mark.nextSibling);
            objects.markAcPreviousSibling = (objects.mark.previousSibling);
        }
        // define the real parent target
        objects.$target = objects.$mark.parentsUntil('body').last();
        // if there is parent
        if (objects.$target[0]) {
            objects.target = objects.$target[0];
            // get next and prev sibling
            objects.nextSibling = (objects.target.nextElementSibling) ? objects.target.nextElementSibling : false;
            objects.previousSibling = (objects.target.previousElementSibling) ? objects.target.previousElementSibling : false;
        } else {
            objects.$target = $(ed.getBody());
            objects.target = ed.getBody();
            // get next and prev sibling
            objects.nextSibling = (objects.markNextSibling) ? objects.markNextSibling : false;
            objects.previousSibling = (objects.markPreviousSibling) ? objects.markPreviousSibling : false;
        }
        // get accurate next and prev sibling
        objects.acNextSibling = (objects.target.nextSibling) ? $.clone(objects.target.nextSibling) : objects.nextSibling;
        objects.acPreviousSibling = (objects.target.previousSibling) ? $.clone(objects.target.previousSibling) : objects.previousSibling;
        // add more info
        objects.collapsed = ed.selection.getRng().collapsed;
        objects.startOffset = ed.selection.getRng().startOffset;
        objects.endOffset = ed.selection.getRng().endOffset;
        // move bookmark
        if (mv) {
            // Restore the selection bookmark
            ed.selection.moveToBookmark(bm);
        }
        // check callback
        if (callBack) {
            // call the function
            var callBackData = callBack(objects);
            // move bookmark
            if (!mv) {
                // Restore the selection bookmark
                ed.selection.moveToBookmark(bm);
            }
            // return
            return callBackData;
        }
        // return
        return objects;
    }

    function cleanHTML(value) {
        var emptyTagsBr = /<[\w]*(?=\s|>)(?!(?:[^>=]|=(['"])(?:(?!\1).)*\1)*?\sdata-mce-type=['"])[^>]*>\s*<\/[\w]*>/g;
        //var emptyTagsBr = /<[\w]*(?=\s|>)(?!(?:[^>=]|=(['"])(?:(?!\1).)*\1)*?\sdata-mce-type=['"])[^>]*>\s*(<br\s*[\/]?>)?\s*<\/[\w]*>/g;
        var cleaned = value.replace(emptyTagsBr, '');
        return value;
    }

    // function for on key press
    function editorEvents(ed) {
        // on keydown
        ed.onKeyDown.add(function(ed, e) {
            // undo
            ed.undoManager.add();
            // to disable the merge of p tag with span tags
            var backSpaceKey = (e.keyCode == 8);
            var deleteKey = (e.keyCode == 46);
            // backspace
            if (backSpaceKey || deleteKey) {
                ed.undoManager.add();
                // get where the cursor position
                if (getCursorPosition(ed, function(OB) {
                        if (backSpaceKey) {
                            if (OB.markAcPreviousSibling && OB.markAcPreviousSibling.nodeName == 'BR') {
                                OB.markAcPreviousSibling.remove();
                                return true;
                            } else if (OB.target && OB.previousSibling) {
                                if (
                                    (OB.target.nodeName == 'P' && OB.previousSibling.nodeName == 'SPAN') ||
                                    (OB.target.nodeName == 'SPAN' && OB.previousSibling.nodeName == 'P')
                                ) {
                                    if (!(OB.markAcPreviousSibling && OB.markAcPreviousSibling.nodeName) || OB.markAcPreviousSibling.data == '') {
                                        return true;
                                    }
                                } else if (OB.target.nodeName == 'BODY') {
                                    if (
                                        (OB.markNextSibling.nodeName == 'P' && OB.markPreviousSibling.nodeName == 'SPAN') ||
                                        OB.markNextSibling.nodeName == 'SPAN' && OB.markPreviousSibling.nodeName == 'P'
                                    ) {
                                        if (OB.markAcNextSibling.nodeName == '#text' && (OB.markAcPreviousSibling.nodeName == 'P' || OB.markAcPreviousSibling.data == '')) {
                                            return true;
                                        }
                                    }
                                }
                            }
                        } else {
                            if (OB.target && OB.nextSibling) {

                                if (OB.markNextSibling && OB.markNextSibling.nodeName == 'BR') {
                                    OB.markNextSibling.remove();
                                    return true;
                                } else if (
                                    (OB.target.nodeName == 'P' && OB.nextSibling.nodeName == 'SPAN') ||
                                    (OB.target.nodeName == 'SPAN' && OB.nextSibling.nodeName == 'P')
                                ) {
                                    if (!(OB.markAcNextSibling && OB.markAcNextSibling.nodeName) || OB.markAcNextSibling.data == '') {
                                        return true;
                                    }
                                } else if (OB.target.nodeName == 'BODY') {
                                    if (
                                        (OB.markNextSibling.nodeName == 'P' && OB.markPreviousSibling.nodeName == 'SPAN') ||
                                        OB.markNextSibling.nodeName == 'SPAN' && OB.markPreviousSibling.nodeName == 'P'
                                    ) {
                                        if (OB.markAcPreviousSibling.nodeName == '#text' && (OB.markAcNextSibling.nodeName == 'P' || OB.markAcNextSibling.data == '')) {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    })) {
                    ed.undoManager.add();
                    e.preventDefault();
                }
            }
            // clean up
            ed.execCommand('mceCleanup');
        });

        // check if the user writes on no where
        ed.onKeyPress.add(function (ed, e) {
            ed.undoManager.add();
            getCursorPosition(ed, function(OB) {
                var currentNode = OB.target;
                if (currentNode.nodeName == 'BODY' && e.charCode != 13) {
                    // select the nearest tag
                    var nextElement = OB.nextSibling;
                    if (nextElement) {
                        ed.undoManager.add();
                        var char = (e.keyCode == 32) ?  '&nbsp;' : String.fromCharCode(e.keyCode);
                        var mark = $('<i>'+char+'</i>');
                        $(nextElement).prepend(mark);
                        ed.selection.select(mark[0]);
                        $(currentNode).find('[data-mce-type]').remove();
                        ed.execCommand('mceCleanup');
                        ed.selection.collapse(false);
                        ed.undoManager.add();
                        e.preventDefault();
                    }
                }
            });
        });

        // click on text tinyMCE editor
        ed.onKeyUp.add(function(ed, e) {
            ed.undoManager.add();
            // check if the backspace
            if (e.keyCode == 8 || e.keyCode == 46) {
                // to merge the spans together
                var selectedTag = ed.selection.getEnd();
                if (selectedTag.nodeName == 'SPAN' && selectedTag.nextSibling && selectedTag.nextSibling.nodeName == 'SPAN') {
                    // VARS
                    var $selectedTag = $(selectedTag);
                    var nextSibling = selectedTag.nextSibling;
                    var $nextSibling = $(nextSibling);
                    // add the text to the selected tag
                    $selectedTag.append($nextSibling.text());
                    // remove the old on
                    $nextSibling.remove();
                    // clean up
                    ed.execCommand('mceCleanup');
                };
                // convert all the spans in the p tags
                var $allChildSpans = $(ed.getBody()).find('p span');
                if ($allChildSpans.length) {
                    $allChildSpans.each(function() {
                        // VARS
                        var $this = $(this);
                        // REPLACE the spans with text only
                        $this.replaceWith($this.text());
                    });
                }
            };
            ed.undoManager.add();
        });
    }
    //
    $('#MainContent_Textarea1, #MainContent_Textarea2').tinymce(defaultOptions);
    // change the default options
    var newOptions = {
        custom_undo_redo: true,
        // General options
        theme: "advanced",/*
        plugins: "pagebreak,directionality,noneditable,paste",
        paste_preprocess: function(pl, o) {
            getCursorPosition(this, function(OB) {
                // Content string containing the HTML from the clipboard
                var stripedContent = o.content.replace(/&nbsp;/g, ' ').replace(/(<([^>]+)>)/ig, '');
                var cleanedHTML = cleanHTML(stripedContent);
                o.content = cleanedHTML
            });
        },*/
        language: "ar",
        // direction
        directionality: "rtl",
        // clean up
        cleanup: true,
        cleanup_on_startup: true,
        width: '100%',
        height: 200,
        theme_advanced_source_editor_wrap: true,
        // Theme options
        theme_advanced_buttons1: "bold,|,undo,redo",
        theme_advanced_buttons2: "",
        theme_advanced_buttons3: "",
        theme_advanced_buttons4: "",
        theme_advanced_path: false,
        theme_advanced_toolbar_location: "top",
        theme_advanced_toolbar_align: "right",
        theme_advanced_resizing: false,
        // Example content CSS (should be your site CSS)
        content_css: "styles/tinymce_content.css",
        // invalid elements
        invalid_elements: "applet,body,button,caption,fieldset ,font,form,frame,frameset,head,,html,iframe,img,input,link,meta,object,option,param,script,select,style,table,tbody,tr,td,th,tbody,textarea,xmp",
        // valid elements
        valid_elements: "@[class],strong,br,i[!id]",
        force_br_newlines: true,
        force_p_newlines: false,
        forced_root_block: false,
        cleanup_callback: function(type, value) {
            switch (type) {
                case "get_from_editor":
                    break;
                case "insert_to_editor":
                    // Clear empty tags
                    value = cleanHTML(value);
                    break;
                case "submit_content":
                    break;
                case "get_from_editor_dom":
                    break;
                case "insert_to_editor_dom":
                    break;
                case "setup_content_dom":
                    break;
                case "submit_content_dom":
                    break;
            }
            return value;
        }
    }
    $('#MainContent_Textarea3').tinymce(newOptions);

    // ajax load the dropdown list
    jQuery.ajax({
        cache: false,
        type: 'get',
        url: 'ProcedureHandler.ashx',
        data: {
            funcname: 'GetProcedureTypes'
        },
        dataType: 'json',
        success: function(response) {
            if (response != '') {
                // vars
                var MainContent_DropDownList1 = $('#MainContent_DropDownList1');
                var listData = $('#listData');
                // loop to create the options
                for (var i = 0; i < response.length; i++) {
                    var option = response[i];
                    // create the option in the dropdown list
                    MainContent_DropDownList1.append($('<option></option>').attr('value', option.ID).text(option.ProcedureType));
                }
                // first list
                MainContent_DropDownList1.change(function() {
                    if (MainContent_DropDownList1.val() != 0) {
                        // reset
                        listData.html('');
                        // vars
                        var selectOptionIndex = $('option:selected', MainContent_DropDownList1).index() - 1;
                        var newDropDownListValues = response[selectOptionIndex];
                        var newDropDownListValuesLength = newDropDownListValues.SessionProcedureObj.length;
                        // loop to create the options
                        for (var i = 0; i < newDropDownListValuesLength; i++) {
                            var option = newDropDownListValues.SessionProcedureObj[i];
                            // create the option in the dropdown list
                            listData.append($('<li/>').data('value', option.ID).html(option.ProcedureTitle));
                        }
                    } else {
                        // reset
                        listData.html('');
                    }
                });
                // second list
                $(listData).delegate('li', 'click', function() {
                    // vars
                    var $this = $(this);
                    var addingParText = $this.html();
                    var clone = $('<p/>').append(addingParText).attr('procedure-id', $this.data('value')).css({ "text-align": "right" });
                    var cloneHTML = clone[0].outerHTML;
                    var ed = $('#MainContent_Textarea2').tinymce();
                    // add to the undo manager
                    ed.undoManager.add();
                    // get caret position
                    getCursorPosition(ed, function(OB) {
                        if (OB.collapsed) {
                            // get current target
                            var nodeName = OB.target.nodeName;
                            // check node name
                            if (nodeName == 'BODY') {
                                ed.execCommand('mceInsertRawHTML', false, cloneHTML);
                            } else {
                                if (
                                    (OB.markAcPreviousSibling && OB.markAcPreviousSibling.data != undefined) &&
                                    (OB.markAcNextSibling && OB.markAcNextSibling.data != undefined) &&
                                    (OB.markAcPreviousSibling.data.length >= OB.markAcNextSibling.data.length)
                                ) {
                                    OB.$target.after(cloneHTML);
                                } else {
                                    OB.$target.before(cloneHTML);
                                }
                            }
                            // add to the undo manager
                            ed.undoManager.add();
                        } else {
                            alert('Without any selection please !!')
                        }
                    }, true);
                });
            }
        },
        error: function() {

        }
    });
    // add procuder button
    $(".btnAddManagePoint").click(function(e) {
        $(".sameAsPrevSpeaker").removeAttr('checked');
        $('.ddlSpeakersClone').empty();
        $('#MainContent_ddlSpeakers option').clone().appendTo('.ddlSpeakersClone');
        $(".ddlSpeakersClone").val("0");
        $(".ddlSpeakersClone option:contains(" + "غير محدد" + ")").remove();
        $(".ddlSpeakersClone option:contains(" + "أخرى" + ")").remove();

        // show the popup
        $(".popupoverlay").show();
        $(".reviewpopup_cont6").show();
        e.preventDefault();
    });
    // add procuder yes button
    $(".btnSaveManagePoint").click(function(e) {

        if ($(".ddlSpeakersClone").val() != 0) {

            //set the speaker
            $("#MainContent_ddlSpeakers option[value='" + $(".ddlSpeakersClone").val() + "']").attr("selected", "selected");
            $("#select2-MainContent_ddlSpeakers-container").html($('#MainContent_ddlSpeakers :selected').text());

            // set the editor
            /* var ed1 = $('#MainContent_elm1').tinymce();
             var htmlContent = ed1.getContent();
             var clone = $('<div>').append(htmlContent)
             clone.find('span').removeClass('highlight editable hover');


             */
            var clone = $('#MainContent_elm1').html();
            // bind the new value
            $('#MainContent_elm1').val("<span data-stime='" + startTime.val() + "'>الأخ الرئيس</span>");
            // add to the undo manager
            $('#MainContent_elm1').tinymce().undoManager.add();

            //alert(clone);
            splitActionForManagePoint(clone);
            // close the popup
            $(".popupoverlay").hide();
            $(".reviewpopup_cont6").hide();
            e.preventDefault();
        } else alert("من فضلك اختر المتحدث");
    });

    $(".btnAddProcuder").click(function(e) {
        // get the editors
        var ed1 = $('#MainContent_elm1').tinymce();
        var ed2 = $('#MainContent_Textarea2').tinymce();
        // clear the html
        var htmlContent = ed1.getContent();
        var clone = $('<div>').append(htmlContent)
        clone.find('span').removeClass('highlight editable hover');
        ed2.setContent(clone.html());
        // clear the undo manager
        ed2.undoManager.clear();
        // show the popup
        $(".popupoverlay").show();
        $(".reviewpopup_cont2").show();
        e.preventDefault();
    });
    // add procuder yes button
    $(".approve2").click(function(e) {
        // bind the new value
        $('#MainContent_elm1').val($("textarea.splittinymce", '.reviewpopup_cont2').val());
        // add to the undo manager
        $('#MainContent_elm1').tinymce().undoManager.add();
        // close the popup
        $(".popupoverlay").hide();
        $(".reviewpopup_cont2").hide();
        e.preventDefault();
    });

    // add Attach button
    $(".btnAssignAttachToContentItem").click(function(e) {
        /*var checkedRadio = $(".rdlattachments input:radio");
        var attachId = $(".attachId").val();
        if (attachId) {
            checkedRadio.filter('[value="' + attachId + '"]').prop('checked', true);
        }
        */

        loadSessionAttaches();
        $(".popupoverlay").show();
        $(".reviewpopup_cont4").show();
        // $(".rdlattachments").attr("checked", "");
        e.preventDefault();
    });

    function loadSessionAttaches() {
        var attachId = $(".attachId").val();
        $('.rdlattachments').empty();
        //Load Available Votes
        jQuery.ajax({
            cache: false,
            type: 'post',
            url: 'SessionHandler.ashx',
            data: {
                funcname: 'GetSessionAttachments',
                sid: $(".sessionID").val()
            },
            dataType: 'json',
            success: function(response) {
                var radio;
                var label;
                var div;
                for (i = 0; i < response.length; i++) {
                    radio = $('<input>').attr({
                        type: 'radio',
                        name: 'colorinput',
                        value: response[i].ID,
                        id: 'test' + response[i].ID
                    });
                    if (response[i].ID.toString() == attachId.toString()) {
                        radio.attr("checked", "checked");
                    }
                    div = $('<div class="rd">');
                    div.append(radio);
                    div.append('<label>' + response[i].Name + '</label>');
                    $('.rdlattachments').append(div);
                }
            }
        });
    }

    $(".btnAddAttach", '.reviewpopup_cont4').click(function(e) {
        var checkedRadio = $(".rdlattachments input:radio:checked");
        if (checkedRadio.length > 0) {
            $(".attachId").val(checkedRadio.val());
            $('.divAttach').show();
            $('.spanAttachTitle').html(checkedRadio.next().text());
        }
        // close the popup
        $(".popupoverlay").hide();
        $(".reviewpopup_cont4").hide();
        e.preventDefault();

    });

    $(".removeAttach").click(function(e) {
        $(".attachId").val('0');
        $('.divAttach').hide();
        $('.spanAttachTitle').html('');
        e.preventDefault();
    });

    $(".removeAgendaItem").click(function(e) {
        $(".agendaItemId").val($(".unAssignedAgendaId").val());
        $('.divAgenda').hide();
        $('.agendaItemTxt').html('');
        $('.agendaItemIsIndexed').html('0');
        e.preventDefault();
    });

$('.chkTopic').change(function() {
    // this will contain a reference to the checkbox   
    if (this.checked) {
        //Load Available Votes
      /*  jQuery.ajax({
            cache: false,
            type: 'post',
            url: 'EditSessionHandler.ashx',
            data: {
                funcname: 'GetTopicID',
                sid: $(".sessionID").val(),
                scid:  $(".hdSessionContentItemID").val()
            },
            dataType: 'json',
            success: function(response) {
              $(".topicId").val(response); 
              $(".aPopupGetAttTopic").show();
            }
        });*/
        $(".topicId").val($(".prevTopicId").val());
        $(".aPopupGetAttTopic").show();
    } else {
         $(".topicId").val("0");
         $(".aPopupGetAttTopic").hide();
    }
});
   
    var $request;
   $('.aPopupGetAttTopic').click(function(e) {
        var sid =  $(".sessionID").val();
        var tpcid =  $(".topicId").val();
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
                $(".popupoverlay").show();
                $(".popupAttendant").show();
                e.preventDefault();
            },
            error: function (response) { }
        });
        e.preventDefault();
    });
    
    $('.btnAddAttToTopic').click(function (e) {
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
                tpcid: $(".topicId").val()
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

    // add Attach button
    $(".btnAddTopic").click(function(e) {
        loadSessionTopics();
        $(".popupoverlay").show();
        $(".reviewpopup_cont7").show();
        e.preventDefault();
    });

    function loadSessionTopics() {

        var topicId = $(".topicId").val();
        $('.rdltopics').empty();
        //Load Available Votes
        jQuery.ajax({
            cache: false,
            type: 'post',
            url: 'TopicHandler.ashx',
            data: {
                funcname: 'GetSessionTopics',
                sid: $(".sessionID").val()
            },
            dataType: 'json',
            success: function(response) {
                var radio;
                var label;
                var div;
                for (i = 0; i < response.length; i++) {
                    radio = $('<input>').attr({
                        type: 'radio',
                        name: 'colorinput',
                        value: response[i].ID,
                        id: 'test' + response[i].ID
                    });
                    if (response[i].ID.toString() == topicId.toString()) {
                        radio.attr("checked", "checked");
                    }
                    div = $('<div class="rd">');
                    div.append(radio);
                    div.append('<label>' + response[i].Title + '</label>');
                    $('.rdltopics').append(div);
                }

            }
        });
    }

    $(".btnSaveTopic", '.reviewpopup_cont7').click(function(e) {
        var checkedRadio = $(".rdltopics input:radio:checked");
        if (checkedRadio.length > 0) {
            $(".topicId").val(checkedRadio.val());
            $('.spanTopicTitle').html(checkedRadio.next().text());
            //$('.divTopic').show();
            $('.divTopic').hide();
        }
        // close the popup
        $(".popupoverlay").hide();
        $(".reviewpopup_cont7").hide();
        e.preventDefault();
    });

    $(".removeTopic").click(function(e) {
        $(".topicId").val('0');
        $('.spanTopicTitle').html('');
        $('.divTopic').hide();
        $(".aPopupGetAttTopic").hide();
        e.preventDefault();
    });

    // add new agenda button
    $(".addingNewAgendaItem").click(function(e) {
        // show the popup
        $(".popupoverlay").show();
        $(".reviewpopup_cont3").show();
        // reset values
        if ($('.agendaItemIsIndexed').val() == "1")
            $('.isAgendaItemIndexed', '.reviewpopup_cont3').prop('checked', true);
        else $('.isAgendaItemIndexed', '.reviewpopup_cont3').prop('checked', false);

        if ($.trim($('.agendaItemTxt').html()) != 'غير معرف')
            $("textarea.splittinymce", '.reviewpopup_cont3').val($('.agendaItemTxt').html());
        else {
            $("textarea.splittinymce", '.reviewpopup_cont3').val('');
            // $('.isAgendaItemIndexed', '.reviewpopup_cont3').prop('checked', false);
        }


        e.preventDefault();
    });
    // add new agenda yes button
    $(".approve3").click(function(e) {
        // vars
        var checked = $('.isAgendaItemIndexed', '.reviewpopup_cont3').is(':checked');
        var htmlData = $("textarea.splittinymce", '.reviewpopup_cont3').val();
        checked = (checked) ? 1 : 0;
        if (htmlData != '') {
            // ajax load
            jQuery.ajax({
                cache: false,
                type: 'post',
                url: 'EditSessionHandler.ashx',
                data: {
                    funcname: 'AddAgendaItem',
                    agendaitemtext: encodeURIComponent(htmlData),
                    sid: $(".sessionID").val(),
                    isindexed: checked,
                    c: $(".hdSessionContentItemID").val(),
                    agendaid: $('.agendaItemId').val() == $('.unAssignedAgendaId').val() ? '' : $('.agendaItemId').val()
                },
                dataType: 'json',
                success: function(response) {
                    // save the id
                    $('.agendaItemId').val(response);
                    // close the popup
                    $(".popupoverlay").hide();
                    $(".reviewpopup_cont3").hide();
                    // repalce the button with the content
                    // $(".addingNewAgendaItem").hide();
                    $('.agendaItemTxt').html(htmlData);
                    $('.agendaItemIsIndexed').val(checked);
                    if (response != 0) {
                        $(".divAgenda").show();
                    }
                },
                error: function() {}
            });
        }
        e.preventDefault();
    });
});