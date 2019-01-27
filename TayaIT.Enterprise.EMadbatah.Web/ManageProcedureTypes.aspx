<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ManageProcedureTypes.aspx.cs"
    Title="المضبطة الإلكترونية - إدارة الإجراءات" Inherits="TayaIT.Enterprise.EMadbatah.Web.ManageProcedureTypes"
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
    <style type="text/css">
        .Gridview
        {
            border: 1px solid #DEDEDE;
            color: black;
            font-size: 160%;
            font-weight:bold;
        }
        .Gridview td
        {
            border: 1px solid #DEDEDE;
            color: black;
            background-color: white;
            padding: 10px;
        }
    </style>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="grid_24 xxlargerow">
                <div class="Ntitle">
                    قائمة الاجراءات:</div>
            </div>
            <div class="clear">
            </div>
            <div class="grid_22">
                <div>
                    <asp:GridView ID="gvProcedures" runat="server" AutoGenerateColumns="false" DataKeyNames="id"
                        CssClass="Gridview" ShowFooter="true" ShowHeader="false" OnRowEditing="gvProcedures_RowEditing"
                        OnRowUpdating="gvProcedures_RowUpdating" OnRowCancelingEdit="gvProcedures_RowCancelingEdit">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lblProcedureTitle" runat="server" Text='<%# Eval("ProcedureTypeStr")%>'
                                        PostBackUrl='<%# String.Format("~/ManageProcedures.aspx?proctypeid={0}", Eval("ID"))%>'></asp:LinkButton>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="textProcedureTitle" runat="server" Text='<%# Eval("ProcedureTypeStr")%>'></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="textProcedureTitle" runat="server" Style="width: 100%" TextMode="MultiLine"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:CommandField ShowEditButton="true" CancelText="الغاء" EditText="تعديل" UpdateText="حفظ" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkRemove" runat="server" CommandArgument='<%# Eval("ID")%>'
                                        OnClick="gvProcedures_RowDeleting" OnClientClick="return confirm('هل أنت متأكد أنك تريد الحذف ؟ -- سيتم حذف الاجراءات الخاصة بتلك القائمة ')"
                                        Text="حذف"></asp:LinkButton>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Button ID="btnAdd" runat="server" Text="اضافة" CssClass="btn" OnClick="AddNewProcedure" />
                                </FooterTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
                <div>
                    <asp:Label ID="lblresult" runat="server"></asp:Label>
                </div>
            </div>
            <div class="clear">
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</asp:Content>
