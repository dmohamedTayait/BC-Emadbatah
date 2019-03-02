<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SpeakersAttendance.aspx.cs"
    Inherits="TayaIT.Enterprise.EMadbatah.Web.SpeakersAttendance" MasterPageFile="~/Site.master"
    Title="المضبطة الإلكترونية - حضور و غياب الأعضاء" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <script src="scripts/AttendantScript.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <script src="Scripts/jquery-1.4.2.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery.dynDateTime.min.js" type="text/javascript"></script>
    <script src="Scripts/calendar-en.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="scripts/jPlayer/jquery.jplayer.min.js"></script>
    <link href="styles/jplayer.blue.monday.css" rel="stylesheet" type="text/css" />
    <link href="Styles/calendar-blue.css" rel="stylesheet" type="text/css" />
    <link href="Styles/calendar-blue.css" rel="stylesheet" type="text/css" />
    <style>
        select {
            width: 100%;
            font-size: 14px;
        }

        table.radio_list td label {
            display: none;
        }

        .table {
            font-size: 14px;
            font-weight: bold;
        }

            .table th {
                text-align: right;
                height: 35px;
                font-size: 14px;
                font-weight: bold;
                min-width: 10px;
                max-width: 360px;
                padding-left: 40px !important;
                border-bottom: 1px solid #DEDEDE;
            }

            .table td {
                min-width: 10px;
                max-width: 360px;
                padding-left: 40px !important;
                border-bottom: 1px solid #DEDEDE;
            }

        .radio_list, .radio_list td {
            border: none;
        }

        .radio_list {
            width: 100%;
        }

        .sessionopenintime td input {
            margin: 5px 15px 0px 10px;
        }

        .space-st1 {
            margin: 0 2px;
            display: inline-block;
            width: 110px;
        }

        .Gridview {
            border: 1px solid #DEDEDE;
            color: black;
            font-size: 14px;
        }

            .Gridview th {
                padding-left: 15px !important;
                padding-right: 5px !important;
                height: 36px;
                width:auto;
                font-size: 14px;
            }

            .Gridview td {
                padding-left: 30px !important;
                width:auto;
                font-size: 14px;
            }

        .txtgrid {
            font-weight: bold;
            font-size: 100%;
            line-height: 25px;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            AjaxEndMethod();
            if (!Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack()) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(AjaxEndMethod);
            }
        });
        function AjaxEndMethod() {
            $(".Calender").dynDateTime({
                showsTime: true,
                timeFormat: 12,
                ifFormat: "%m/%d/%Y %H:%M",
                daFormat: "%l;%M %p, %e %m,  %Y",
                align: "BR",
                electric: false,
                singleClick: false,
                displayArea: ".siblings('.dtcDisplayArea')",
                button: ".next()"
            });
        }

    </script>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="grid_24">
                        <div>
                            <asp:Label runat="server" ID="lblInfo1" Visible="false" CssClass="lInfo"></asp:Label>
                        </div>
                        <div class="xxlargerow">
                            <div class="Ntitle">
                                حضور و غياب الأعضاء:
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                        <div class="largerow">
                            <div class="grid_3 h2">
                                <asp:Label ID="Label1" runat="server" Text="رقم الجلسة:"></asp:Label>
                            </div>
                            <div class="grid_8">
                                <asp:DropDownList ID="ddlSessions" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSessions_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                            <div class="clear">
                            </div>
                        </div>
                        <div class="largerow">
                            <asp:RadioButtonList ID="ddlAttendantTypes" runat="server" AutoPostBack="True" Style="display: none"
                                OnSelectedIndexChanged="ddlAttendantTypes_SelectedIndexChanged" RepeatDirection="Horizontal"
                                CssClass="sessionopenintime h2">
                            </asp:RadioButtonList>
                            <div class="clear">
                            </div>
                        </div>
                        <br />

                        <asp:GridView ID="GVAttendants" runat="server" CssClass="table gvAttendants" BorderWidth="0"
                            CellPadding="0" AutoGenerateColumns="false" GridLines="None" OnRowDataBound="GVAttendants_RowDataBound">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:HiddenField runat="server" ID="HFID" Value='<%# Bind("ID") %>'></asp:HiddenField>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="اسم العضو">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblFName" Text='<%# string.Format("{0} {1} {2}", Eval("AttendantTitle ") ,Eval("AttendantDegree"),Eval("LongName"))%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<span class='space-st1' style='color:blue'>حاضر</span><span class='space-st1' style='color:red'>غائب</span><span class='space-st1' style='color:green'>غائب بعذر</span>">
                                    <ItemTemplate>
                                        <asp:RadioButtonList ID="RBLAttendantStates" runat="server" RepeatDirection="Horizontal"
                                            DataSourceID="AttendantStateDS" DataTextField="ArName" DataValueField="ID" CssClass="radio_list">
                                        </asp:RadioButtonList>
                                        <asp:SqlDataSource ID="AttendantStateDS" runat="server" ConnectionString="<%$ ConnectionStrings:EMadbatahConn %>"
                                            SelectCommand="SELECT * FROM [AttendantState] where id in (1,2,3)"></asp:SqlDataSource>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <div runat="server" id="divAbsenceExcuse" class="row divAbsenceExcuse">
                                            <div class="grid_2">
                                                <asp:DropDownList ID="ddlAbsenseExcuse" runat="server" DataSourceID="AbsenceExcuseDS" DataTextField="excuse" DataValueField="excuse" CssClass="ddlAbsenseExcuse">
                                                </asp:DropDownList>
                                                <asp:SqlDataSource ID="AbsenceExcuseDS" runat="server" ConnectionString="<%$ ConnectionStrings:EMadbatahConn %>"
                                                    SelectCommand="SELECT * FROM [AbsenceExcuse]"></asp:SqlDataSource>
                                            </div>
                                            <div class="grid_5">
                                                <asp:TextBox runat="server" ID="txtAbsenceExcuse" Text='' Style="display: none; width: 100%"></asp:TextBox>
                                            </div>

                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblJobTitle" Text='<%#  Eval("JobTitle")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>

                        <asp:GridView ID="GVOutsideAttendants" runat="server" AutoGenerateColumns="false" DataKeyNames="id" ShowFooter="true"
                            CssClass="table Gridview displaynone" BorderWidth="0" CellPadding="0" GridLines="None" OnRowEditing="GVOutsideAttendants_RowEditing"
                            OnRowUpdating="GVOutsideAttendants_RowUpdating" OnRowCancelingEdit="GVOutsideAttendants_RowCancelingEdit">
                            <Columns>
                                <asp:TemplateField HeaderText="ID" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblAttId" runat="server" Text='<%# Eval("ID")%>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:Label ID="lblAttId" runat="server" Text='<%# Eval("ID")%>'></asp:Label>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="الدرجة العلمية">
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
                                    <FooterTemplate>
                                        <asp:DropDownList ID="ddlDegree" runat="server" Style="width: 100%;">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="الدكتور" Text="الدكتور"></asp:ListItem>
                                            <asp:ListItem Value="الدكتورة" Text="الدكتورة"></asp:ListItem>
                                        </asp:DropDownList>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="اسم العضو" ItemStyle-ForeColor="#837f55" HeaderStyle-ForeColor="#837f55">
                                    <ItemTemplate>
                                        <asp:Label ID="lblAttLongName" runat="server" Text='<%# Eval("LongName")%>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="textAttLongName" runat="server" Text='<%# Eval("LongName")%>' Style="width: 100%;"
                                            CssClass="txtgrid"></asp:TextBox>
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <asp:TextBox ID="textAttLongName" runat="server" Style="width: 100%;" CssClass="txtgrid"></asp:TextBox>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="الاسم فى ملف الوورد">
                                    <ItemTemplate>
                                        <asp:Label ID="lblAttName" runat="server" Text='<%# Eval("Name")%>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="textAttName" runat="server" Text='<%# Eval("Name")%>' Style="width: 100%;"
                                            CssClass="txtgrid"></asp:TextBox>
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <asp:TextBox ID="textAttName" runat="server" Style="width: 100%;" CssClass="txtgrid"></asp:TextBox>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="اللقب" ItemStyle-ForeColor="#837f55" HeaderStyle-ForeColor="#837f55">
                                    <ItemTemplate>
                                        <asp:Label ID="lblAttTitle" runat="server" Text='<%# Eval("AttendantTitle")%>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="textAttTitle" runat="server" Text='<%# Eval("AttendantTitle")%>'
                                            Style="width: 100%"></asp:TextBox>
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <asp:TextBox ID="textAttTitle" runat="server" Style="width: 100%;" CssClass="txtgrid"></asp:TextBox>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="المسمى الوظيفى">
                                    <ItemTemplate>
                                        <asp:Label ID="lblAttJobTitle" runat="server" Text='<%# Eval("JobTitle")%>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="textAttJobTitle" runat="server" Text='<%# Eval("JobTitle")%>' Style="width: 100%;"
                                            CssClass="txtgrid"></asp:TextBox>
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <asp:TextBox ID="textAttJobTitle" runat="server" Style="width: 100%;" CssClass="txtgrid"></asp:TextBox>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="نوع العضو" Visible="false">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlAttType" runat="server" Enabled="false" Style="width: 100%;">
                                            <asp:ListItem Value="10" Text="الأمين العام"></asp:ListItem>
                                            <asp:ListItem Value="4" Text="أعضاء الأمانة"></asp:ListItem>
                                            <asp:ListItem Value="2" Text="خارج المجلس" Selected="True"></asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="ddlAttType" runat="server" Style="width: 100%;">
                                            <asp:ListItem Value="10" Text="الأمين العام"></asp:ListItem>
                                            <asp:ListItem Value="4" Text="أعضاء الأمانة"></asp:ListItem>
                                            <asp:ListItem Value="2" Text="خارج المجلس" Selected="True"></asp:ListItem>
                                        </asp:DropDownList>
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <asp:DropDownList ID="ddlAttType" runat="server" Style="width: 100%;">
                                            <asp:ListItem Value="10" Text="الأمين العام"></asp:ListItem>
                                            <asp:ListItem Value="4" Text="أعضاء الأمانة"></asp:ListItem>
                                            <asp:ListItem Value="2" Text="خارج المجلس" Selected="True"></asp:ListItem>
                                        </asp:DropDownList>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:CommandField ShowEditButton="true" CancelText="الغاء" EditText="تعديل" UpdateText="حفظ" />
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkRemove" runat="server" CommandArgument='<%# Eval("ID")%>'
                                            OnClientClick="return confirm('هل أنت متأكد أنك تريد الحذف ؟')" OnClick="GVOutsideAttendants_RowDeleting"
                                            Text="حذف"></asp:LinkButton>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Button ID="btnAdd" runat="server" Text="اضافة" CssClass="btn" OnClick="AddNewOutSideAttendat" />
                                    </FooterTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <br />
                        <br />
                        <br />
                        <asp:Button ID="btnSave" runat="server" Text="حفــظ" OnClick="btnSave_Click" CssClass="btn"
                            Style="display: none" />
                        <br />
                        <br />
                        <div>
                            <asp:Label runat="server" ID="lblInfo2" Visible="false" CssClass="lInfo"></asp:Label>
                        </div>
                        <br />
                        <br />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
</asp:Content>
