<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ManageDeletedDefaultAttendance.aspx.cs"
    Title="المضبطة الإلكترونية - إدارة الأعضاء المحذوفين" Inherits="TayaIT.Enterprise.EMadbatah.Web.ManageDeletedDefaultAttendance"
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
        }
        .Gridview td
        {
            border: 1px solid #DEDEDE;
            color: black;
            background-color: white;
            padding: 10px; /* max-width: 130px;*/
        }
        .Gridview th
        {
            border: 1px solid #DEDEDE;
            color: black;
            background-color: #c1c1c1;
            padding: 10px;
        }
        .txtgrid
        {
            font-weight: bold;
            font-size: 100%;
            line-height: 25px;
        }
    </style>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="grid_24 xxlargerow">
                <div class="Ntitle">
                    <span>ادارة الأعضاء:</span>
                        <a href="ManageDefaultAttendance.aspx" style="font-size: 20px; font-weight: bold;float: left">عودة لصفحة إدارة الأعضاء</a>
                  </div>
            </div>
            <div class="clear">
            </div>
            <div class="grid_24">
                <asp:GridView ID="gvDefaultAttendants" runat="server" AutoGenerateColumns="false"
                    AllowPaging="true" OnPageIndexChanging="gvDefaultAttendants_PageIndexChanging"
                    PageSize="5" DataKeyNames="id" CssClass="Gridview h2">
                    <Columns>
                           <asp:TemplateField HeaderText="الدرجة العلمية" ItemStyle-Width="50px">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlDegree" runat="server" SelectedValue='<%# Eval("AttendantDegree") %>'
                                    Enabled="false" Style="width: 100%;">
                                     <asp:ListItem Value="" Text=""></asp:ListItem>
                                    <asp:ListItem Value="الدكتور" Text="الدكتور"></asp:ListItem>
                                    <asp:ListItem Value="الدكتورة" Text="الدكتورة"></asp:ListItem>
                                </asp:DropDownList>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlDegree" runat="server" SelectedValue='<%# Eval("AttendantDegree") %>'
                                    Style="width: 100%;">
                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                    <asp:ListItem Value="الدكتور" Text="الدكتور"></asp:ListItem>
                                    <asp:ListItem Value="الدكتورة" Text="الدكتورة"></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="اسم العضو">
                            <ItemTemplate>
                                <asp:Label ID="lblAttLongName" runat="server" Text='<%# Eval("LongName")%>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="textAttLongName" runat="server" Text='<%# Eval("LongName")%>' Style="width: 100%;
                                    height: 40px" CssClass="txtgrid"></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="اسم العضو الثلاثى فى ملف الوورد">
                            <ItemTemplate>
                                <asp:Label ID="lblAttName" runat="server" Text='<%# Eval("Name")%>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="textAttName" runat="server" Text='<%# Eval("Name")%>' Style="width: 100%;
                                    height: 40px" CssClass="txtgrid"></asp:TextBox>
                            </EditItemTemplate>
                         </asp:TemplateField>
                        <asp:TemplateField HeaderText="اسم العضو الثنائى فى ملف الوورد">
                            <ItemTemplate>
                                <asp:Label ID="lblAttShortName" runat="server" Text='<%# Eval("ShortName")%>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="textAttShortName" runat="server" Text='<%# Eval("ShortName")%>'
                                    Style="width: 100%; height: 40px" CssClass="txtgrid"></asp:TextBox>
                            </EditItemTemplate>
                         </asp:TemplateField>
                        <asp:TemplateField HeaderText="اللقب" ItemStyle-Width="75px">
                            <ItemTemplate>
                                <asp:Label ID="lblAttTitle" runat="server" Text='<%# Eval("AttendantTitle")%>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="textAttTitle" runat="server" Text='<%# Eval("AttendantTitle")%>'
                                    Style="width: 100%"></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="المسمى الوظيفى" ItemStyle-Width="130px">
                            <ItemTemplate>
                                <asp:Label ID="lblAttJobTitle" runat="server" Text='<%# Eval("JobTitle")%>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="textAttJobTitle" runat="server" Text='<%# Eval("JobTitle")%>' Style="width: 100%;
                                    height: 40px"  CssClass="txtgrid"></asp:TextBox>
                            </EditItemTemplate>
                         </asp:TemplateField>
                        <asp:TemplateField HeaderText="نوع العضو" ItemStyle-Width="130px">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlDeptCode" runat="server" SelectedValue='<%# Eval("Type") %>'
                                    Enabled="false" Style="width: 100%;">
                                    <asp:ListItem Value="1" Text="عضو من المجلس"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="زائر خارجى "></asp:ListItem>
                                    <asp:ListItem Value="3" Text="السادة الوزراء"></asp:ListItem>
                                    <asp:ListItem Value="5" Text="رئيس المجلس"></asp:ListItem>
                                    <asp:ListItem Value="9" Text="أمير البلاد"></asp:ListItem>
                                </asp:DropDownList>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlDeptCode" runat="server" SelectedValue='<%# Eval("Type") %>'
                                    Style="width: 100%;">
                                    <asp:ListItem Value="1" Text="عضو من المجلس"></asp:ListItem>
                                    <asp:ListItem Value="3" Text="السادة الوزراء"></asp:ListItem>
                                    <asp:ListItem Value="5" Text="رئيس المجلس"></asp:ListItem>
                                    <asp:ListItem Value="9" Text="أمير البلاد"></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="صورة العضو">
                            <ItemTemplate>
                                <asp:Image ID="imgAttendantAvatar" runat="server" ImageUrl='<%# String.Format("/images/AttendantAvatars/{0}", Eval("AttendantAvatar"))%>'
                                    Width="150px" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:FileUpload ID="fileUploadAttendantAvatar" runat="server" Width="150px" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkUnRemove" runat="server" CommandArgument='<%# Eval("ID")%>'
                                    OnClick="gvDefaultAttendants_RowUnDeleting"
                                    Text="الغاء الحذف"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
            <div class="clear">
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</asp:Content>
