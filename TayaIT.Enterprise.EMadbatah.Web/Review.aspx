<%@ Page Title="المضبطة الإلكترونية - صفحة المراجعه" Language="C#" MasterPageFile="~/Site.master"
    AutoEventWireup="true" CodeFile="Review.aspx.cs" Inherits="TayaIT.Enterprise.EMadbatah.Web.Review" %>

<%@ Import Namespace="TayaIT.Enterprise.EMadbatah.Model" %>
<%@ Import Namespace="TayaIT.Enterprise.EMadbatah.DAL" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <script type="text/javascript" src="scripts/jquery.hotkeys.js"></script>
    <script type="text/javascript" src="scripts/tiny_mce/jquery.tinymce.js"></script>
    <script type="text/javascript" src="scripts/jPlayer/jquery.jplayer.min.js"></script>
    <script type="text/javascript" src="scripts/jquery.elastic.source.js"></script>
    <script type="text/javascript" src="scripts/jquery.hoverIntent.minified.js"></script>
    <script type="text/javascript" src="scripts/ReviewScript.js"></script>
    <link href="styles/jplayer.blue.monday.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <input id="SessionContentItemIDHidden" type="hidden" value="" />
    <input id="SessionIDHidden" type="hidden" runat="server" />
    <input id="IsSessionStartHidden" type="hidden" value="" />
    <input id="isSessionCompleted" class="isSessionCompleted" type="hidden" runat="server" />
    <input id="isCurrentUserFileRev" class="isCurrentUserFileRev" type="hidden" runat="server" />
    <input id="currentUserID" class="currentUserID" type="hidden" runat="server" />
    <script>
    function Button1_onclick() {
                                    
                                    $.ajax({ url: "Check.aspx?sid=" + <%=SessionID %>});
                                    if(document.getElementById("Button1").value == "نشر" ){
                                        document.getElementById("Button1").value = "الغاء النشر";
                                        console.log($("#chk_session_checker"));
                                         $("#chk_session_checker").attr("checked",true);
                                         }
                                    else
                                    {
                                        document.getElementById("Button1").value = "نشر";
                                         console.log($("#chk_session_checker"));
                                          $("#chk_session_checker").attr("checked",false);
                                        }
                                    
                                    // you can add some processing to the AJAX call
                                    return false;
                                }
    </script>
    <div class="reviewcontainer" id="maintable1">
        <div class="row">
            <div class="grid_4 h2">
                للذهاب / الموافقة على الملف:</div>
            <table class="table deftable h2" border="0" cellspacing="0" cellpadding="0">
                <thead>
                    <tr>
                        <th class="column column4">
                            اسم الملف
                        </th>
                        <th class="column column5">
                            الحالة
                        </th>
                        <th class="column column5">
                            حالة المراجعة
                        </th>
                        <th class="column column4">
                            اسم الملف
                        </th>
                        <th class="column column5">
                            الحالة
                        </th>
                        <th class="column column5">
                            حالة المراجعة
                        </th>
                        <th class="column column4">
                            اسم الملف
                        </th>
                        <th class="column column5">
                            الحالة
                        </th>
                        <th class="column column5">
                            حالة المراجعة
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <% int loop = 0;
                       int filesLen = sd.SessionFiles.Where(c => c.IsSessionStart != true && c.IsActive == 1).Count();
                       int div = filesLen / 3;
                       int assumedFilesLen = 3 * div;
                       int rem = filesLen % 3;
                       List<SessionAudioFile> firstArr = sd.SessionFiles.Where(c => c.IsSessionStart != true && c.IsActive == 1).ToList();
                       for (int i = 0; i < filesLen - rem; i = i + 3)
                       //     foreach (SessionAudioFile s in sd.SessionFiles)
                       {%>
                    <tr class="tbrow tbrowNew tbrowID1">
                        <% SessionAudioFile s = firstArr[i];%>
                        <td class="column column5">
                            <a class="gotofile" href="#file_<%=s.ID%>">
                                <%=System.IO.Path.GetFileName(s.Name)%></a>
                        </td>
                        <td class="column column5">
                            <%= GetLocalizedString("strSessionFileStatus" + s.Status.ToString())%>
                        </td>
                        <% if (s.Status == TayaIT.Enterprise.EMadbatah.Model.SessionFileStatus.Completed)
                           {
                               if (SessionContentItemHelper.GetFileSessionContentItemsStatusNotByStatusID(s.ID, 1) > 0)
                               {   %>
                        <td class="column column5">
                            <a class="approveSessionFile" data-fileid="<%=s.ID%>" href="#file_<%=s.ID%>">موافقة</a>
                            <br />
                        </td>
                        <% }
                           else
                           {%>
                        <td class="column column5">
                            <span style="color: Green">موافق عليه</span><br />
                        </td>
                        <%}
                       }
                           else
                           {%>
                        <td class="column column5">
                        </td>
                        <%} %>
                        <% s = firstArr[i + 1];%>
                        <td class="column column5">
                            <a class="gotofile" href="#file_<%=s.ID%>">
                                <%=System.IO.Path.GetFileName(s.Name)%></a>
                        </td>
                        <td class="column column5">
                            <%= GetLocalizedString("strSessionFileStatus" + s.Status.ToString())%>
                        </td>
                        <% if (s.Status == TayaIT.Enterprise.EMadbatah.Model.SessionFileStatus.Completed)
                           {
                               if (SessionContentItemHelper.GetFileSessionContentItemsStatusNotByStatusID(s.ID, 1) > 0)
                               {   %>
                        <td class="column column5">
                            <a class="approveSessionFile" data-fileid="<%=s.ID%>" href="#file_<%=s.ID%>">موافقة</a>
                            <br />
                        </td>
                        <% }
                            else
                            {%>
                        <td class="column column5">
                            <span style="color: Green">موافق عليه</span><br />
                        </td>
                        <%}
                        }
                           else
                           {%>
                        <td class="column column5">
                        </td>
                        <%} %>
                        <% s = firstArr[i + 2];%>
                        <td class="column column5">
                            <a class="gotofile" href="#file_<%=s.ID%>">
                                <%=System.IO.Path.GetFileName(s.Name)%></a>
                        </td>
                        <td class="column column5">
                            <%= GetLocalizedString("strSessionFileStatus" + s.Status.ToString())%>
                        </td>
                        <% if (s.Status == TayaIT.Enterprise.EMadbatah.Model.SessionFileStatus.Completed)
                           {
                               if (SessionContentItemHelper.GetFileSessionContentItemsStatusNotByStatusID(s.ID, 1) > 0)
                               {   %>
                        <td class="column column5">
                            <a class="approveSessionFile" data-fileid="<%=s.ID%>" href="#file_<%=s.ID%>">موافقة</a>
                            <br />
                        </td>
                        <% }
                          else
                          {%>
                        <td class="column column5">
                            <span style="color: Green">موافق عليه</span><br />
                        </td>
                        <%}
                      }
                           else
                           {%>
                        <td class="column column5">
                        </td>
                        <%} %>
                    </tr>
                    <% }%>
                    <% if (rem > 0)
                       {%>
                    <tr class="tbrow tbrowNew tbrowID1">
                        <% for (int i = 0; i < filesLen - assumedFilesLen; i++)
                           {
                               SessionAudioFile s = firstArr[assumedFilesLen + i];%>
                        <td class="column column5">
                            <a class="gotofile" href="#file_<%=s.ID%>">
                                <%=System.IO.Path.GetFileName(s.Name)%></a>
                        </td>
                        <td class="column column5">
                            <%= GetLocalizedString("strSessionFileStatus" + s.Status.ToString())%>
                        </td>
                        <% if (s.Status == TayaIT.Enterprise.EMadbatah.Model.SessionFileStatus.Completed)
                           {
                               if (SessionContentItemHelper.GetFileSessionContentItemsStatusNotByStatusID(s.ID, 1) > 0)
                               {   %>
                        <td class="column column5">
                            <a class="approveSessionFile" data-fileid="<%=s.ID%>" href="#file_<%=s.ID%>">موافقة</a>
                            <br />
                        </td>
                        <% }
                          else
                          {%>
                        <td class="column column5">
                            <span style="color: Green">موافق عليه</span><br />
                        </td>
                        <%}
                      }
                           else
                           {%>
                        <td class="column column5">
                        </td>
                        <%} %>
                    </tr>
                    <% }%>
                    <% }%>
                </tbody>
            </table>
        </div>
        <!--reviewmaincontent start-->
        <div class="reviewmaincontent">
            <asp:Panel ID="pnlContent" runat="server">
                <div class="basic_info">
                    <span class="info_cont prefix_1"><span class="title">الجلسة:</span> <span class="Bold_title"
                        id="sessionSerial" runat="server">[464/14ف]</span> </span><span class="info_cont prefix_1">
                            <span class="title">بتاريخ:</span> <span class="Bold_title" id="sessionDate" runat="server">
                                23-6-2010</span> </span>
                </div>
                <div class="graybg relpos">
                    <!--popupdiv start-->
                    <div class="reviewpopup_cont graybg" style="height: 400px ! important">
                        <div class="close_btn">
                        </div>
                        <div class="clear">
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
                                </div>
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                        <div class="datacontainer inputcont">
                            <textarea id="elm1" name="elm1" rows="15" style="width: 100%; height: 300px !important"
                                runat="server" class="tinymce"></textarea>
                            <script type="text/javascript">
                                try {
                                    // tinymce
                                    $('textarea.tinymce').tinymce({
                                        custom_undo_redo: false,
                                        // Location of TinyMCE script
                                        script_url: 'scripts/tiny_mce/tiny_mce.js',
                                        // General options
                                        theme: "advanced",
                                        plugins: "pagebreak,directionality,noneditable",
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
                                        valid_elements: "@[class],span[*],p[*],strong,em,blockquote,br",
                                        force_br_newlines: true,
                                        force_p_newlines: false,
                                        forced_root_block: false,
                                        setup: function (ed) {
                                            // function to make the span editable
                                            function editableSpan(ed, e) {
                                                if (e) {
                                                    // remove all classes from the editor
                                                    $('span.editable', ed.contentDocument).removeClass('editable');
                                                    // add class editable
                                                    if (e.nodeName == 'SPAN') {
                                                        // add class editable
                                                        $(e).addClass('editable');
                                                        // time from the span
                                                        var time = Math.floor($(e).attr('data-stime'))
                                                        // seek
                                                        $("#jquery_jplayer_1").jPlayer("pause", time);
                                                    }
                                                }
                                            }
                                            // click on text tinyMCE editor
                                            ed.onMouseUp.add(function (ed, e) {
                                                editableSpan(ed, e.target)
                                            });
                                            // check if the user writes on no where
                                            ed.onKeyDown.add(function (ed, l) {
                                                var dom = ed.dom;
                                                var currentNode = ed.selection.getNode();
                                                var keycode = l.keyCode;
                                                var valid =
                    (keycode > 47 && keycode < 58) || // number keys
                    keycode == 32 || keycode == 13 || // spacebar & return key(s) (if you want to allow carriage returns)
                    (keycode > 64 && keycode < 91) || // letter keys
                    (keycode > 95 && keycode < 112) || // numpad keys
                    (keycode > 185 && keycode < 193) || // ;=,-./` (in order)
                    (keycode > 218 && keycode < 227);   // [\]' (in order)
                                                if (currentNode.nodeName == 'BODY' && valid) {
                                                    // select the nearest tag
                                                    var nextElement = ed.selection.getRng().startContainer.nextSibling;
                                                    if (nextElement) {
                                                        var mark = $('<i>.</i>');
                                                        $(nextElement).prepend(mark);
                                                        ed.selection.select(mark[0]);
                                                        ed.execCommand('mceCleanup');
                                                    }
                                                }
                                            });
                                            // oninit
                                            ed.onInit.add(function (ed) {
                                                var AudioPlayer = $("#jquery_jplayer_1");
                                                // all span segments
                                                var all_spans_segments = $('span.segment', ed.contentDocument);
                                                // hover effect
                                                all_spans_segments.live("mouseover mouseout", function (event) {
                                                    if (event.type == "mouseover") {
                                                        // remove all classes
                                                        $(this).toggleClass('hover');
                                                    } else {
                                                        // remove hover class
                                                        $(this).removeClass('hover');
                                                    }
                                                });

                                                // jplayer shorcuts
                                                $(document).add(ed.dom.doc.body).bind('keydown', function (e) {
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
                                                }).bind('keydown', 'alt+w', function () {
                                                    // play & pause player
                                                    if (AudioPlayer.data("jPlayer").status.paused) {
                                                        AudioPlayer.jPlayer("play");
                                                    } else {
                                                        AudioPlayer.jPlayer("pause");
                                                    }
                                                }).bind('keydown', 'alt+q', function () {
                                                    // stop player
                                                    AudioPlayer.jPlayer("stop");
                                                })
                                            });
                                        }
                                    });

                                } catch (e) {
                                    alert(e)
                                }
                                

                            </script>
                        </div>
                        <div class="inputcont">
                            <textarea id="note" placeholder="إضافة ملحوظة"></textarea>
                        </div>
                        <div class="poppbtnscont fl">
                            <div class="fl">
                                <%--<ul>
                                    <li><a id="approve" href="#">
                                        <img border="0" src="images/arrow_orange.gif">موافق عليه</a></li>
                                    <li>
                                        <img src="images/angle_line.gif"></li>
                                    <li><a id="reject" href="#">
                                        <img border="0" src="images/arrow_orange.gif">مرفوض</a></li>
                                    <li>
                                        <img src="images/angle_line.gif"></li>
                                    <li><a id="save" href="#">
                                        <img border="0" src="images/arrow_orange.gif">حفظ</a></li>
                                </ul>--%>
                                <div class="fl">
                                    <input type="button" id="approve" class="def_btn" value="موافق عليه" />
                                </div>
                                <div class="fl">
                                    <input type="button" id="reject" class="def_btn" value="مرفوض" />
                                </div>
                                <div class="fl">
                                    <input type="button" id="save" class="def_btn" value="حفظ" />
                                </div>
                                <div class="clear">
                                </div>
                            </div>
                            <div class="fl">
                                <a id="lnkMoreEditOptions" href="#">[خيارات تعديل أكثر]</a>
                            </div>
                            <div class="clear">
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                    </div>
                    <!--divcontent start-->
                    <div class="divcontent" id="madbatahContents" runat="server">
                        <div class="popupoverlay">
                        </div>
                    </div>
                    <!--divcontent end-->
                    <!--topheader_cont start-->
                    <div class="topheader_cont">
                        <ul>
                            <li>
                                <div class="black_box">
                                </div>
                                <div class="desc">
                                    النصوص <strong>الموافق عليها</strong></div>
                            </li>
                            <li>
                                <div class="red_box">
                                </div>
                                <div class="desc">
                                    النصوص <strong>المرفوضة</strong> <span>(</span> <span id="spnRejectCount">
                                        <%=tblStats != null ? tblStats[(int)TayaIT.Enterprise.EMadbatah.Model.SessionContentItemStatus.Rejected].ToString() : ""%></span>
                                    <span>)</span>
                                </div>
                            </li>
                            <li>
                                <div class="green_box">
                                </div>
                                <div class="desc">
                                    النصوص <strong>المعدلة</strong> <span>(</span> <span id="spnFixCount">
                                        <%=tblStats != null ? tblStats[(int)TayaIT.Enterprise.EMadbatah.Model.SessionContentItemStatus.Fixed].ToString() : ""%></span><span>)</span>
                                </div>
                            </li>
                            <li>
                                <div class="blue_box">
                                </div>
                                <div class="desc">
                                    النصوص <strong>المعدلة بعد الموافقة</strong> <span>(</span> <span id="spnModAfterApprove">
                                        <%=tblStats != null ? tblStats[(int)TayaIT.Enterprise.EMadbatah.Model.SessionContentItemStatus.ModefiedAfterApprove].ToString() : ""%></span><span>)</span>
                                </div>
                            </li>
                        </ul>
                        <div class="clear">
                        </div>
                    </div>
                    <!--topheader_cont end-->
                    <div class="review_actionbtns prefix_15">
                        <form id="Form1" runat="server">
                        <div id="divSessionActionButtons">
                            <div class="SessionActionButtonsCont">
                                <div class="fl">
                                    <input type="button" runat="server" id="btnApproveSession" class="btnApproveSession def_btn"
                                        value="موافقة" />
                                </div>
                                <div class="fl">
                                    <input type="button" runat="server" id="btnFinalApproveSession" class="btnFinalApproveSession def_btn"
                                        value="تصديق" />
                                </div>
                                <div class="clear">
                                </div>
                            </div>
                        </div>
                        </form>
                    </div>
                </div>
            </asp:Panel>
            <span id="spnMsg" runat="server"></span>
        </div>
        <!--reviewmaincontent end-->
        <div id="divToolTip" class="divToolTip">
            الملف : <span id="spnToolTipFileName"></span>
            <br />
            مدخل البيانات : <span id="spnToolTipUserName"></span>
            <br />
            مراجع الملف : <span id="spnToolTipFileRevName"></span>
            <br />
            مراجع الجلسة : <span id="spnToolTipRevName"></span>
        </div>
    </div>
    <div class="clear">
    </div>
</asp:Content>
