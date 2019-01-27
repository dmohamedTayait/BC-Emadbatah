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
        select
        {
            width: 100%;
            font-size: 16px;
        }
        table.radio_list td label
        {
            display: none;
        }
        .table, .table td
        {
            border-bottom: 1px solid #DEDEDE;
        }
        .radio_list, .radio_list td
        {
            border: none;
        }
        .radio_list
        {
            width: 100%;
        }
        .table th
        {
            text-align: right;
            height: 35px;
        }
        .sessionopenintime td input
        {
            margin: 5px 15px 0px 10px;
        }
        .space-st1
        {
            margin: 0 2px;
            display: inline-block;
            width: 110px;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {


           // mp3player($(".MP3FilePath").val());
            // on change
            var MP3FilePath = $(".MP3FilePath");
            var MP3FilePathValue = MP3FilePath.val();
           // var AudioPlayer = $("#jquery_jplayer_1");
            // timer
            setInterval(function () {
                // vars
                MP3FilePath = $(".MP3FilePath");
                // cehck if the value changed
                if (MP3FilePath.val() != MP3FilePathValue) {
                    // save the last value
                    MP3FilePathValue = MP3FilePath.val();
                    // change the player file
                  /*  AudioPlayer.jPlayer("setMedia", {
                        mp3: MP3FilePathValue
                    }).jPlayer("play", 0);
                    $('.jp-audio .next-jp-xseconds').click(function (e) {
                        AudioPlayer.jPlayer("play", playertime + 5)
                    });
                    // prev x seconds button
                    $('.jp-audio .prev-jp-xseconds').click(function (e) {
                        AudioPlayer.jPlayer("play", playertime - 5)
                    });*/
                    mp3player(MP3FilePathValue);
                }
            }, 500);
        });

        function mp3player(mp3path) {     // alert($(".MP3FilePath").val());
           var AudioPlayer = $("#jquery_jplayer_1");
           var playertime;
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
               ready: function () {
                   // play the jplayer
                   $(this).jPlayer("setMedia", {
                       mp3:mp3path// $(".MP3FilePath").val() // mp3 file path//'http://localhost:12000/SessionFiles/1345/session_10-05-2016_1.mp3'//$(".MP3FilePath").val() // mp3 file path
                   }).jPlayer("play",0);
                   // next x seconds button
                   $('.jp-audio .next-jp-xseconds').click(function (e) {
                       AudioPlayer.jPlayer("play", playertime + 5)
                   });
                   // prev x seconds button
                   $('.jp-audio .prev-jp-xseconds').click(function (e) {
                       AudioPlayer.jPlayer("play", playertime - 5)
                   });
               },
               timeupdate: function (event) {
                   if (!$(this).data("jPlayer").status.paused) {
                       // highlight the word by time
                       playertime = event.jPlayer.status.currentTime;
                   }
               }
           });
       }

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
               <input id="MP3FolderPath" class="MP3FolderPath" type="hidden" runat="server" value="" />
               <input id="MP3FilePath" class="MP3FilePath" type="hidden" runat="server" value="http://localhost:12000/SessionFiles/1345/session_10-05-2016_1.mp3" name="MP3FilePath"/>
                <div class="grid_24">
                    <div>
                        <asp:Label runat="server" ID="lblInfo1" Visible="false" CssClass="lInfo"></asp:Label>
                    </div>
                    <div class="xxlargerow">
                        <div class="Ntitle">
                            حضور و غياب الأعضاء:</div>
                    </div>
                    <div class="clear">
                    </div>
                    <div class="largerow">
                        <div class="grid_2 h2">
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
                    <div id="jquery_jplayer_1" class="jp-jplayer">
                    </div>
                    <div id="jp_container_1" class="jp-audio">
                        <div class="jp-type-single">
                            <div id="jp_interface_1" class="jp-interface">
                                <ul class="jp-controls">
                                    <li><a href="#" class="jp-play" tabindex="1" title="play"></a></li>
                                    <li><a href="#" class="jp-pause" tabindex="1" title="pause"></a></li>
                                </ul>
                                <div class="jp-progress">
                                    <div class="jp-seek-bar">
                                        <div class="jp-play-bar">
                                        </div>
                                    </div>
                                </div>
                                <div class="jp-current-time">
                                </div>
                                <div class="jp-duration">
                                </div>
                                <div class="next-jp-xseconds" title="تقديم 5 ثوانى">
                                </div>
                                <div class="prev-jp-xseconds" title="تاخير 5 ثوانى">
                                </div>
                            </div>
                        </div>
                    </div>
                    <br />
                    <asp:GridView ID="GVAttendants" runat="server" CssClass="table h1 gvAttendants" BorderWidth="0"
                        CellPadding="0" AutoGenerateColumns="false" GridLines="None" OnRowDataBound="GVAttendants_RowDataBound"
                       >
                        <Columns>
                            <asp:TemplateField ItemStyle-Width="5%">
                                <ItemTemplate >
                                    <asp:HiddenField runat="server" ID="HFID" Value='<%# Bind("ID") %>'></asp:HiddenField>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="اسم العضو" ItemStyle-Width="35%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblFName" Text='<%# string.Format("{0} {1} {2}", Eval("AttendantTitle ") ,Eval("AttendantDegree"),Eval("LongName"))%>'></asp:Label></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="35%" HeaderText="<span class='space-st1' style='color:blue'>حاضر</span><span class='space-st1' style='color:red'>غائب</span><span class='space-st1' style='color:green'>غائب بعذر</span>">
                                <ItemTemplate>
                                    <asp:RadioButtonList ID="RBLAttendantStates" runat="server" RepeatDirection="Horizontal"
                                        DataSourceID="AttendantStateDS" DataTextField="ArName" DataValueField="ID" CssClass="radio_list">
                                    </asp:RadioButtonList>
                                    <asp:SqlDataSource ID="AttendantStateDS" runat="server" ConnectionString="<%$ ConnectionStrings:EMadbatahConn %>"
                                        SelectCommand="SELECT * FROM [AttendantState] where id in (1,2,3)"></asp:SqlDataSource>
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="" ItemStyle-Width="25%">
                                <ItemTemplate>
                                    <asp:TextBox runat="server" ID="txtAbsenceExcuse" Text='' style="display:none"></asp:TextBox></ItemTemplate>
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
