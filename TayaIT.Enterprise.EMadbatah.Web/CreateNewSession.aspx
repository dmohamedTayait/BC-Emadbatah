<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CreateNewSession.aspx.cs"
    Title="المضبطة الإلكترونية - إضافة مضبطة جديدة" Inherits="TayaIT.Enterprise.EMadbatah.Web.CreateNewSession"
    MasterPageFile="~/Site.master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <script src="scripts/jquery-3.0.0.min.js" type="text/javascript"></script>
    <script src="scripts/jquery.datetimepicker.full.min.js" type="text/javascript"></script>
    <link href="styles/jquery.datetimepicker.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(document).ready(function () {
            AjaxEndMethod();
            if (!Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack()) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(AjaxEndMethod);
            }
        });
        function AjaxEndMethod() {
            // change the date language
            $.datetimepicker.setLocale('ar');
            // date picker
            $(".Calender").datetimepicker({
                timepicker: false,
                defaultDate: new Date(),
                format:'m/d/Y',
            });
            // time picker
            $(".timePicker").datetimepicker({
                datepicker: false,
                defaultDate: new Date(),
                format: 'H:i'
            });
        }
    </script>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <div class="grid_24 xxlargerow">
        <div class="Ntitle" runat="server" id="divPageTitle">
            اضافة مضبطة جديدة:</div>
    </div>
    <div class="clear">
    </div>
    <div class="grid_22">
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtDate"
                    ErrorMessage="*" ForeColor="Red" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
                <asp:Label ID="lblDate" runat="server" Text="الميعاد المقرر لبدء الجلسة ( التاريخ -الوقت )"></asp:Label>
            </div>
            <div class="grid_8">
                <asp:TextBox ID="txtDate" runat="server" class="textfield inputBlock Calender" />
            </div>
            <div class="grid_3">
              <asp:TextBox ID="txtTime" runat="server" class="textfield inputBlock timePicker"></asp:TextBox>
            </div>
               <div class="grid_3 h2">
                  <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="txtTime"
                    ErrorMessage="*" ForeColor="Red" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
                    ForeColor="Red" ControlToValidate="txtEParliamentID" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
                <asp:Label ID="lblEParliamentID" runat="server" Text="رقم الجلسة منذ بدء الحياة النيابية"></asp:Label>
            </div>
            <div class="grid_8">
                <asp:TextBox ID="txtEParliamentID" runat="server" CssClass="textfield inputBlock"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtEParliamentID"
                    ErrorMessage="يمكنك ادخال أرقام فقط" ForeColor="Red" ValidationExpression="^[0-9]*$"
                    ValidationGroup="VGSession"> </asp:RegularExpressionValidator>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" ControlToValidate="txtSeason" ErrorMessage="*" ForeColor="Red" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
                <asp:Label ID="lblSeason" runat="server" Text="الفصل التشريعى"></asp:Label>
            </div>
            <div class="grid_8">
               <asp:TextBox ID="txtSeason" runat="server" CssClass="textfield inputBlock"></asp:TextBox>
                 <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="txtSeason"
                    ErrorMessage="يمكنك ادخال أرقام فقط" ForeColor="Red" ValidationExpression="^[0-9]*$"
                    ValidationGroup="VGSession"> </asp:RegularExpressionValidator>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" ControlToValidate="txtStage" ErrorMessage="*" ForeColor="Red" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
                <asp:Label ID="lblStage" runat="server" Text="دور الانعقاد"></asp:Label>
            </div>
            <div class="grid_8">
                <asp:TextBox ID="txtStage" runat="server" CssClass="textfield inputBlock"></asp:TextBox>
                 <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="txtStage"
                    ErrorMessage="يمكنك ادخال أرقام فقط" ForeColor="Red" ValidationExpression="^[0-9]*$"
                    ValidationGroup="VGSession"> </asp:RegularExpressionValidator>
            </div>
               <div class="grid_3">
                 <asp:TextBox ID="txtStageType" runat="server" CssClass="textfield inputBlock"></asp:TextBox>
                 </div>
               <div class="grid_3 h2">
                   <asp:RequiredFieldValidator ID="RequiredFieldValidator16" runat="server" ControlToValidate="txtStageType" ErrorMessage="*" ForeColor="Red" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator15" runat="server" ControlToValidate="ddlPresident" ErrorMessage="*" ForeColor="Red" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
                <asp:Label ID="Label1" runat="server" Text="رئيس الجلسة"></asp:Label>
            </div>
            <div class="grid_8">
                <asp:DropDownList ID="ddlPresident" runat="server" CssClass="inputBlock">
                </asp:DropDownList>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="*"
                    ControlToValidate="txtSubject" ValidationGroup="VGSession" ForeColor="Red"></asp:RequiredFieldValidator>
                <asp:Label ID="lblSubject" runat="server" Text="الجلسة"></asp:Label>
               
            </div>
            <div class="grid_8">
                <asp:TextBox ID="txtSubject" runat="server" CssClass="textfield inputBlock"></asp:TextBox>
                 <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtSubject"
                    ErrorMessage="يمكنك ادخال أرقام فقط" ForeColor="Red" ValidationExpression="^[0-9]*$"
                    ValidationGroup="VGSession"> </asp:RegularExpressionValidator>
            </div>
               <div class="grid_3">
                <asp:TextBox ID="txtType" runat="server" CssClass="textfield inputBlock"></asp:TextBox>
                 </div>
               <div class="grid_3 h2">
                   <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" ControlToValidate="txtType" ErrorMessage="*" ForeColor="Red" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow">
            <div class="grid_6 h2">
                &nbsp;
            </div>
            <div class="grid_8 h2">
                <asp:CheckBox ID="CBSessionStart" runat="server" Text="اكتمال النصاب القانونى فى الموعد الأول"
                    Checked Style="font-size: 15px;" class="chk" />
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="prefix_5 addnewusercont">
            <asp:Button ID="btnCreateNewSession" runat="server" Text="حفظ بيانات المضبطة" OnClick="btnCreateNewSession_Click"
                ValidationGroup="VGSession" CssClass="btn" />
        </div>
    </div>
    <div class="clear">
    </div>
    </form>
</asp:Content>
