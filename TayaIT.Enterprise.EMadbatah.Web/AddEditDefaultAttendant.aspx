<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddEditDefaultAttendant.aspx.cs"
    Title="المضبطة الإلكترونية - إضافة عضو جديد" Inherits="TayaIT.Enterprise.EMadbatah.Web.AddEditDefaultAttendant"
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
        <div>
            <asp:Label runat="server" ID="lblInfo1" Visible="false" CssClass="lInfo"></asp:Label>
        </div>
        <div class="Ntitle">
            <span>
                <%= pageTitle %></span><a href="ManageDefaultAttendance.aspx" style="float: left">عودة
                    لصفحة ادارة الأعضاء</a>
        </div>
    </div>
    <div class="clear">
    </div>
    <div class="grid_22">
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="ddlAttType"
                    InitialValue="0" ErrorMessage="*" ValidationGroup="VGSession" ForeColor="Red" />
                <asp:Label ID="lblAttDegree" runat="server" Text="الدرجة العلمية"></asp:Label>
            </div>
            <div class="grid_10">
                <asp:DropDownList ID="ddlDegree" runat="server" Style="width: 100%;">
                    <asp:ListItem Value="" Text=""></asp:ListItem>
                    <asp:ListItem Value="الدكتور" Text="الدكتور"></asp:ListItem>
                    <asp:ListItem Value="الدكتورة" Text="الدكتورة"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="textAttLongName"
                    ErrorMessage="*" ForeColor="Red" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
                <asp:Label ID="lblAttLongName" runat="server" Text="اسم العضو"></asp:Label>
            </div>
            <div class="grid_10">
                <asp:TextBox ID="textAttLongName" runat="server" Style="width: 100%; height: 30px"
                    CssClass="textfield inputBlock"></asp:TextBox>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="textAttName"
                    ErrorMessage="*" ForeColor="Red" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
                <asp:Label ID="lblAttName" runat="server" Text="اسم العضو الثلاثى فى ملف الوورد"></asp:Label>
            </div>
            <div class="grid_10">
                <asp:TextBox ID="textAttName" runat="server" Style="width: 100%; height: 30px" CssClass="textfield inputBlock"></asp:TextBox>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow displaynone">
            <div class="grid_6 h2">
                <asp:Label ID="lblAttShortName" runat="server" Text="اسم العضو الثنائى فى ملف الوورد"></asp:Label>
            </div>
            <div class="grid_10">
                <asp:TextBox ID="textAttShortName" runat="server" Style="width: 100%; height: 30px"
                    CssClass="textfield inputBlock"></asp:TextBox>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:RequiredFieldValidator ID="rfv1" runat="server" ControlToValidate="textAttTitle"
                    InitialValue="0" ErrorMessage="*" ValidationGroup="VGSession" ForeColor="Red" />
                <asp:Label ID="lblAttTitle" runat="server" Text="اللقب"></asp:Label>
            </div>
            <div class="grid_10">
                <asp:TextBox ID="textAttTitle" runat="server" Style="width: 100%; height: 30px" CssClass="textfield inputBlock"
                    Text="السيد"></asp:TextBox>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:Label ID="lblAttJobTitle" runat="server" Text="المسمى الوظيفى"></asp:Label>
            </div>
            <div class="grid_10">
                <asp:TextBox ID="textAttJobTitle" runat="server" Style="width: 100%; height: 50px"
                    CssClass="textfield inputBlock" TextMode="MultiLine"></asp:TextBox>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="ddlAttType"
                    InitialValue="0" ErrorMessage="*" ValidationGroup="VGSession" ForeColor="Red" />
                <asp:Label ID="lblAttType" runat="server" Text="نوع العضو"></asp:Label>
            </div>
            <div class="grid_10">
                <asp:DropDownList ID="ddlAttType" runat="server" Style="width: 100%;">
                     <asp:ListItem Value="1" Text="عضو من المجلس"></asp:ListItem>
                     <asp:ListItem Value="4" Text="الأمانة العامة "></asp:ListItem>
                     <asp:ListItem Value="3" Text="السادة الوزراء"></asp:ListItem>
                     <asp:ListItem Value="5" Text="رئيس المجلس"></asp:ListItem>
                     <asp:ListItem Value="10" Text="الأمين العام"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:Label ID="lblttAvatar" runat="server" Text="صورة العضو"></asp:Label>
            </div>
            <div class="grid_10">
                <asp:FileUpload ID="fuAttAvatar" runat="server" />
                <asp:Image ID="imgAttendantAvatar" runat="server" Width="150px" />
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="prefix_5 addnewusercont" style="float: left">
            <asp:Button ID="btnAddEditDefAtt" runat="server" Text="حفظ بيانات العضو" OnClick="btnAddEditDefAtt_Click"
                ValidationGroup="VGSession" CssClass="btn" />
        </div>
        <br />
        <br />
        <div>
            <asp:Label runat="server" ID="lblInfo2" Visible="false" CssClass="lInfo"></asp:Label>
        </div>
        <br />
        <br />
    </div>
    <div class="clear">
    </div>
    </form>
</asp:Content>
