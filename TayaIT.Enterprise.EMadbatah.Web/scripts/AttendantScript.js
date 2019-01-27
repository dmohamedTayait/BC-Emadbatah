
$(document).ready(function () {

    rd_att_type_change();

});

function rd_att_type_change()
{
    $('.gvAttendants input[type=radio]').live("change", function () {
        var p = this.closest("table").closest("tr");
        txtExcuse = $(p).find("input[type=text]");
        if ($(this).val() == 3) {
            txtExcuse.show();
        }
        else {
            txtExcuse.hide();
            txtExcuse.val('');
        }
    });
}

