
$(document).ready(function () {

    rd_att_type_change();

});

function rd_att_type_change()
{
    $('.gvAttendants input[type=radio]').live("change", function () {
        var p = this.closest("table").closest("tr");
        divAbsenceExcuse = $(p).find(".divAbsenceExcuse");
        txtExcuse = $(p).find("input[type=text]");
        if ($(this).val() == 3) {
            $(divAbsenceExcuse).show();
        }
        else {
            $(divAbsenceExcuse).hide();
            $(txtExcuse).val('');
        }
    });

    $('.ddlAbsenseExcuse').live("change", function () {
        var divAbsenceExcuse = this.closest(".divAbsenceExcuse");
        txtExcuse = $(divAbsenceExcuse).find("input[type=text]");
        console.log($(this).val());
        if ($(this).val() == "0") {
            $(txtExcuse).val('');
            $(txtExcuse).show();
        }
        else {
            $(txtExcuse).val('');
            $(txtExcuse).hide();
        }
    });
}

