

$(document).ready(function () {

    //////////////////// Votes
    // Load votes
    $(".btnAddNewVote").click(function (e) {
        $(".txtNewVote", '.reviewpopup_cont5').val('');
        loadSessionVotes();
        $(".popupoverlay").show();
        $(".reviewpopup_cont5").show();
        e.preventDefault();
    });

    $(".ddl_votes").change(function () {
        loadVotesMembers();
    });

    function loadSessionVotes() {
        var voteId = $(".voteId").val();
        $('.ddl_votes').empty();
        //Load Available Votes
        jQuery.ajax({
            cache: false,
            type: 'post',
            url: 'VotingHandler.ashx',
            data: {
                funcname: 'GetSessionVotes',
                sid: $(".sessionID").val()
            },
            dataType: 'json',
            success: function (response) {
                var option;
                var label;
                var div;
                $('.ddl_votes').append($('<option>').attr({ value: "0" }).html("اختر من بين التصويتات"));
                for (i = 0; i < response.length; i++) {
                    option = $('<option>').attr({
                        value: response[i].ID
                    });
                    $('.ddl_votes').append(option.html(response[i].VoteSubject));
                }

                $('.ddl_votes').val(voteId);
                if ($('.ddl_votes').val() == voteId && voteId != "0") {
                    $(".chbVote").attr("checked", "true");
                }
                loadVotesMembers();
            }
        });
    }


    function loadVotesMembers() {

        $('#AttendantCont').html($('<img src="/images/loading.gif">'));

        var voteId = $(".ddl_votes").val();
        if (voteId != "0") {
            jQuery.ajax({
                cache: false,
                type: 'post',
                url: 'VotingHandler.ashx',
                data: {
                    funcname: 'GetVoteMemVal',
                    sid: $(".sessionID").val(),
                    voteid: voteId
                },
                dataType: 'json',
                success: function (response) {
                    var option;
                    var label;
                    var div;
                    for (i = 0; i < response.length; i++) {
                        option = $('<option>').attr({
                            value: response[i].ID
                        });
                        $('.ddl_votes').append(option.html(response[i].VoteSubject));
                    }

                    grid_1 = $('<div>').addClass("grid_11");
                    table1 = $('<table>').addClass("table_att_voting");
                    grid_2 = grid_1.clone();
                    table2 = table1.clone();
                    var vote_types = ["غير موجود", "موافق", "غير موافق", "ممتنع"]
                    var th_header_container = $('<div>');
                    th_header_container.append($('<span class="blueitem" style="margin: 0 2px;display: inline-block;width: 70px;">').html(vote_types[0])).append($('<span class="greenitem" style="margin: 0 2px;display: inline-block;width: 50px;">').html(vote_types[1])).append($('<span class="reditem" style="margin: 0 2px;display: inline-block;width: 80px;color:red">').html(vote_types[2])).append($('<span class="blueitem" style="margin: 0 2px;display: inline-block">').html(vote_types[3]));

                    row = $('<tr>').attr({});
                    th1 = $('<th style="width:5%">').attr({}).append($('<span>').html("ID").addClass("displaynone"));
                    th2 = $('<th style="width:45%">').attr({}).append($('<span>').html("اسم المتحدث"));
                    th3 = $('<th style="width:50%">').attr({}).html(th_header_container);
                    row = row.append(th1).append(th2).append(th3)
                    table1.append(row.clone());
                    table2.append(row.clone());

                    var first_table_len = parseInt(response.length / 2);

                    for (var i = 0; i < response.length; i++) {
                        row = $('<tr>').attr({});
                        td1 = $('<td style="width:5%">').attr({}).html($('<span>').html(response[i].AttendantID).addClass("displaynone"));
                        td2 = $('<td style="width:45%">').attr({}).html($('<span>').html(response[i].AttendantName));
                        td3 = $('<td style="width:50%">').attr({}).html(draw_radio_options_table(response[i].AttendantID.toString(), response[i].MemberVoteValue));
                        if (i < first_table_len) {
                            table1.append(row.append(td1).append(td2).append(td3));
                        }
                        else {
                            table2.append(row.append(td1).append(td2).append(td3));
                        }
                    }

                    $('#AttendantCont').html(grid_1.append(table1)).append(grid_2.append(table2));
                    $('.agreedVotesCount').html($("input:radio:checked[value='1']").length);
                    $('.disAgreedVotesCount').html($("input:radio:checked[value='2']").length);
                    $('.NoVotesCount').html($("input:radio:checked[value='3']").length);

                    if ($('.ddl_votes').val() == $(".voteId").val()) {
                        $(".chbVote").attr("checked", "true");
                    }
                    else {
                        $(".chbVote").removeAttr("checked");
                    }
                }
            });
        }
        else {
            $('.agreedVotesCount').html(0);
            $('.disAgreedVotesCount').html(0);
            $('.NoVotesCount').html(0);
            $('#AttendantCont').html("");
            $(".chbVote").removeAttr("checked");
        }
    }

    function draw_radio_options_table(attendantID, checkedOptionVal) {
        var vote_types = ["غير موجود", "موافق", "غير موافق", "ممتنع"]
        var table_options = $('<table class="tbl_voting_options">');
        var tr_options = $('<tr>');
        var th_header_container = $('<div>');
        for (var i = 0; i < vote_types.length; i++) {
            radio = $('<input>').attr({
                type: 'radio',
                value: i,
                name: 'options_' + attendantID
            });
            if (checkedOptionVal == (i)) {
                radio.attr("checked", "checked")
            }
            var td_options = $('<td>').html(radio);
            tr_options.append(td_options);
        }
        table_options.append(tr_options);
        return table_options;
    }

    $(".table_att_voting input:radio").live("change", function (e) {
        console.log("here");
        $('.agreedVotesCount').html($("input:radio:checked[value='1']").length);
        $('.disAgreedVotesCount').html($("input:radio:checked[value='2']").length);
        $('.NoVotesCount').html($("input:radio:checked[value='3']").length);
    });
    // add new vote
    $(".btnSaveVote").click(function (e) {
        // vars
        var voteSubject = $(".txtNewVote", '.reviewpopup_cont5').val();
        if (voteSubject != '') {
            // ajax load
            jQuery.ajax({
                cache: false,
                type: 'post',
                url: 'VotingHandler.ashx',
                data: {
                    funcname: 'AddVote',
                    sid: $(".sessionID").val(),
                    votesubject: voteSubject
                },
                dataType: 'json',
                success: function (response) {
                    option = $('<option>').attr({
                        value: response,
                        selected: true
                    });
                    $('.ddl_votes').append(option.html(voteSubject));
                    $(".txtNewVote", '.reviewpopup_cont5').val('');
                    loadVotesMembers();
                },
                error: function () { }
            });
        }
        e.preventDefault();
    });

    $(".btnSaveVoteMems").click(function (e) {
        // vars
        var agreedVote_lst = [];
        var disAgreedVote_lst = [];
        var NoVote_lst = [];
        var NonExist_lst = [];
        var names = [];
        $('.table_att_voting input:radio:checked').each(function () {
            if ($(this).val() == 1) {
                agreedVote_lst.push(this.name.replace("options_", ""));
            }
            else if ($(this).val() == 2) {
                disAgreedVote_lst.push(this.name.replace("options_", ""));
            }
            else if ($(this).val() == 3) {
                NoVote_lst.push(this.name.replace("options_", ""));
            }
            else {
                NonExist_lst.push(this.name.replace("options_", ""));
            }

        });

        if ($('.ddl_votes').val() != '') {
            // ajax load
            jQuery.ajax({
                cache: false,
                type: 'post',
                url: 'VotingHandler.ashx',
                data: {
                    funcname: 'SaveVoteMemVal',
                    sid: $(".sessionID").val(),
                    voteid: $('.ddl_votes').val(),
                    agreed_mem: JSON.stringify(agreedVote_lst),
                    disagreed_mem: JSON.stringify(disAgreedVote_lst),
                    novote_mem: JSON.stringify(NoVote_lst),
                    nonexist_mem: JSON.stringify(NonExist_lst)
                },
                dataType: 'json',
                success: function (response) {
                },
                error: function () { }
            });
        }
        // close the popup
        $(".popupoverlay").hide();
        $(".reviewpopup_cont5").hide();
        e.preventDefault();
    });
    $(".chbVote").change(function () {
        if ($(".chbVote").is(':checked')) {
            $(".voteId").val($('.ddl_votes').val());
            $('.spanVoteSubject').html($('.ddl_votes :selected').text());
            $('.divVote').show();
        }
        else {
            $(".voteId").val(0);
            $('.spanVoteSubject').html('');
            $('.divVote').hide();
        }
        //e.preventDefault();
    });

    $(".removeVote").click(function (e) {
        $(".voteId").val('0');
        $('.spanVoteSubject').html('');
        $('.divVote').hide();
        e.preventDefault();
    });


    ////////////// End vote

});