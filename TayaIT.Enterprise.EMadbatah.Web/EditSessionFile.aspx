<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditSessionFile.aspx.cs"
    Inherits="TayaIT.Enterprise.EMadbatah.Web.EditSessionFile" MasterPageFile="~/Site.master"
    Title="المضبطة الإلكترونية - تعديل الملف الصوتي" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript" src="scripts/jquery.hotkeys.js"></script>
    <script type="text/javascript" src="scripts/tiny_mce/tiny_mce.js"></script>
    <script type="text/javascript" src="scripts/tiny_mce/jquery.tinymce.js"></script>
    <script type="text/javascript" src="scripts/jPlayer/jquery.jplayer.min.js"></script>
    <script type="text/javascript" src="scripts/select2.full.min.js"></script>
    <script type="text/javascript" src="scripts/EditSessionScript.js"></script>
    <link href="styles/jplayer.blue.monday.css" rel="stylesheet" type="text/css" />
    <link href="styles/select2.min.css" rel="stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript">
        // <![CDATA[
        function Button1_onclick() {
            $.ajax({url: "FileError.aspx?sid=" + <%=SessionFileID %>});
          if(document.getElementById("Button1").value == "خطأ" )
              document.getElementById("Button1").value = "خطأ هنا";
          else
              document.getElementById("Button1").value = "خطأ";
          // you can add some processing to the AJAX call
          return false;
      }
      // ]]>
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <% 
        if (CurrentUser == null || file == null)
        {
            Context.Response.Redirect("~/Error.aspx");
        }
        //if (CurrentUser.ID == file.UserID || file.UserID == null
        //            || ((CurrentUser.Role != UserRole.DataEntry) && (current_session.ReviewerID == CurrentUser.ID)))
    %>
    <form id="editSessionFileForm" runat="server">
        <input id="MP3FilePath" class="MP3FilePath" type="hidden" runat="server" value="" />
        <input id="sessionID" type="hidden" value="" runat="server" class="sessionID" />
        <input id="eparId" type="hidden" value="" runat="server" class="eparId" />
        <input id="XmlFilePath" type="hidden" value="" runat="server" class="hdxmlFilePath" />
        <input type="hidden" id="hdPageMode" runat="server" value="1" class="hdPageMode" />
        <input type="hidden" id="hdSessionContentItemID" runat="server" value="1" class="hdSessionContentItemID" />
        <input type="hidden" id="startTime" runat="server" value="" class="hdstartTime" />
        <input type="hidden" id="originalStartTime" runat="server" value="" class="hdoriginalStartTime" />
        <input type="hidden" id="endTime" runat="server" value="" class="hdendTime" />
        <input type="hidden" id="currentOrder" runat="server" value="" class="hdcurrentOrder" />
        <input type="hidden" name="agendaItemId" id="agendaItemId" value="" runat="server"
            class="agendaItemId" />
        <input type="hidden" name="attachId" id="attachId" value="0" runat="server" class="attachId" />
        <input type="hidden" name="voteId" id="voteId" value="0" runat="server" class="voteId" />
        <input type="hidden" name="topicId" id="topicId" value="0" runat="server" class="topicId" />
        <input type="hidden" name="prevTopicId" id="prevTopicId" value="0" runat="server"
            class="prevTopicId" />
        <input type="hidden" name="unAssignedSpeakerId" id="unAssignedSpeakerId" value="0"
            runat="server" class="unAssignedSpeakerId" />
        <input type="hidden" name="unAssignedAgendaId" id="unAssignedAgendaId" value="0"
            runat="server" class="unAssignedAgendaId" />
        <div id="editSessionFile" class="container_24">
            <div class="row">
                <div class="grid_3">
                    <div class="nav">
                        <input name="" runat="server" disabled="disabled" id="btnNext" type="button" class="btn inputBlock mb-5 next btn_editsession"
                            value="التالى" />
                        <input name="" runat="server" disabled="disabled" id="btnPrev" type="button" class="btn inputBlock mb-5 prev btn_editsession"
                            value="السابق" />
                        <input name="" runat="server" disabled="disabled" id="btnFinish" type="button" class="btn inputBlock mb-5 finish btn_editsession"
                            value="انهاء" />
                        <input name="" runat="server" id="btn_addNewAgendaItem" type="button" class="btn inputBlock mb-5 addingNewAgendaItem btn_editsession"
                            value="اضافة /  تعديل فهرس" />
                        <input name="" runat="server" id="btnAddProcuder" type="button" class="btn inputBlock mb-5 btnAddProcuder btn_editsession"
                            value="اضافة اجراء" />
                        <%--  <input name="" runat="server" id="btnAssignAttachToContentItem" type="button" class="btn inputBlock mb-5 btnAssignAttachToContentItem btn_editsession"
                        value="اضافة مرفق" />--%>
                        <input name="" runat="server" id="btnAddNewVote" type="button" class="btn inputBlock mb-5 btnAddNewVote btn_editsession"
                            value="اضافة تصويت" />
                        <%--  <input name="" runat="server" id="btnAddManagePoint" type="button" class="btn inputBlock mb-5 btnAddManagePoint btn_editsession"
                        value="نقطة نظام" />--%>
                        <%-- <input name="" runat="server" id="btnAddNewTopic" type="button" class="btn inputBlock mb-5 btnAddNewTopic btn_editsession"
                        value="اضافة مقترح / توصية" />--%>
                        <input name="" id="btnSplit" runat="server" type="button" class="btn inputBlock mb-5 split btn_editsession"
                            value="اقطع" data-clipboard-action="cut" data-clipboard-target="#MainContent_txtFooter" />
                        <input name="" type="button" id="btnSaveOnly" runat="server" class="btn inputBlock mb-5 btnSaveOnly btn_editsession"
                            value="حفظ" />
                        <input name="" type="button" id="btnSaveAndExit" runat="server" class="btn inputBlock mb-5 btnSaveAndExit btn_editsession"
                            value="حفظ و خروج" />
                        <input name="" type="button" id="various1" data-div="#inline1" class="btn inputBlock mb-5 btn_editsession"
                            value="عودة للنص الأصلى" />
                        <input name="" runat="server" disabled="disabled" id="btnPreview" type="button" class="btn inputBlock mb-5 btnPreview btn_editsession"
                            value="عرض الملف كاملا" />
                        <div class="h2" style="margin-top: 15px;">
                            <input name="chkIgnoredSegment" id="chkIgnoredSegment" runat="server" class="chkIgnoredSegment"
                                type="checkbox" value="" />
                            <label for="chkIgnoredSegment">
                                تجاهل هذا المقطع</label>
                        </div>
                        <%-- <input id="Button1" class="btn inputBlock mb-5" type="button" value="خطأ" onclick="return Button1_onclick()" />--%>
                    </div>
                </div>
                <div class="grid_21">
                    <div class="borderBD row h2">
                        <div class="fr">
                            الملف: <strong>
                                <asp:Label ID="lblMP3FileName" runat="server"></asp:Label></strong>
                        </div>
                        <div class="fl spaceR">
                            بتاريخ: <strong>
                                <asp:Label ID="lblSessionDate" runat="server"></asp:Label></strong>
                        </div>
                        <div class="fl">
                            الجلسة: <strong>(<asp:Label ID="lblSessionName" runat="server"></asp:Label>)</strong>
                        </div>
                        <div class="clear">
                        </div>
                    </div>
                    <div id="divAgenda" name="divAgenda" runat="server" class="divAgenda mb-50 row">
                        <table>
                            <tr>
                                <td>
                                    <div class="h2" style="margin-right: 20px; width: 80px;">
                                        <span class="red">*</span> البند:
                                    </div>
                                </td>
                                <td>
                                    <p class="agendaItemTxt" style="margin-right: 20px; width: 650px">
                                        <%= agendaItemTxt%>
                                    </p>
                                </td>
                                <td>
                                    <a href="javascript:void(0)" class="removeAgendaItem h2" style="margin-right: 30px;">حذف الفهرس</a>
                                </td>
                                <td>
                                    <span class="agendaItemIsIndexed" style="display: none">
                                        <%= agendaItemIsIndexed%>
                                    </span>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <!--<div class="row">
            <div class="grid_3">
            <span class="red">*</span> البند:
            </div>
            <div class="grid_7">
            <div class="marginBS">
            <asp:DropDownList ID="ddlAgendaItems" AutoPostBack="false" runat="server">
            </asp:DropDownList>
            </div>
            <div>
            <input type="checkbox" class="chkGroupSubAgendaItems" id="chkGroupSubAgendaItems"
            runat="server" />
            <label>
            دمج البنود الفرعية</label>
            </div>
            </div>
            <div class="grid_3 prefix_1">
            البند الفرعى:</div>
            <div class="grid_7">
            <div class="marginBS">
            <asp:DropDownList ID="ddlAgendaSubItems" AutoPostBack="false" runat="server">
            </asp:DropDownList>
            </div>
            <div>
            <input name="" type="checkbox" id="specialBranch" value="" />
            <label for="specialBranch">
            بند استثنائى</label>
            <div id="smallwindow" class="smallwindow">
            <div class="smallwindowCont">
            <div class="smallwindowArrow">
            </div>
            <div class="marginBS title">
            ادخل اسم البند الأستثنائى</div>
            <div class="inputs">
            <input name="txtAgendaItem" type="text" id="txtAgendaItem" class="textfield">
            <input name="" type="button" id="btnAddCustomAgendaItem" class="smallbtn" value="إضافة">
            <div class="errorCont">
            </div>
            </div>
            </div>
            </div>
            </div>
            </div>
            <div class="clear">
            </div>
            </div>-->
                    <div class="row">
                        <div class="grid_15">
                            <div class="row">
                                <div class="grid_2 h2">
                                    <span class="red">*</span> المتحدث:
                                </div>
                                <div class="grid_11">
                                    <asp:DropDownList ID="ddlSpeakers" AutoPostBack="false" runat="server" CssClass="inputBlock"
                                        data-live-search="true">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="grid_11" style="margin-right: 110px">
                                    <div id="newjobtitle" class="edit">
                                        <div class="editmode">
                                            <label name="addnewjobtext" id="txtSpeakerJob" runat="server" class="textfield inputBlock"
                                                style="font-weight: bold; font-size: 16px; border: 0 !important; box-shadow: 0 0 !important"
                                                size="26">
                                            </label>
                                        </div>
                                        <div class="donemode">
                                            <a id="editnewjobbutton" href="#">[تعديل]</a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row displaynone" id="divNewSpeaker" name="divNewSpeaker">
                                <div class="grid_11" style="margin-right: 110px">
                                    <div>
                                        <input type="text" name="txtNewSpeaker" id="txtNewSpeaker" runat="server" class="textfield inputBlock txtNewSpeaker"
                                            style="font-weight: bold; font-size: 16px;" size="26" />
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="grid_2 h2">
                                    <span class="red">*</span> لقب أخر:
                                </div>
                                <div class="grid_11">
                                    <asp:DropDownList ID="ddlOtherTitles" AutoPostBack="false" runat="server" CssClass="inputBlock ddlOtherTitles">
                                        <asp:ListItem Text="-- أختر --" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="المقرر" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="عن المقرر" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="-------------------" Value="4"></asp:ListItem>
                                        <asp:ListItem Text="رئيس لجنة" Value="5"></asp:ListItem>
                                        <asp:ListItem Text="عضو لجنة" Value="6"></asp:ListItem>
                                        <asp:ListItem Text="-------------------" Value="7"></asp:ListItem>
                                        <asp:ListItem Text="أخرى" Value="8"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row  divddlCommittee">
                                <div class="grid_11" style="margin-right: 110px">
                                    <asp:DropDownList ID="ddlCommittee" AutoPostBack="false" runat="server" CssClass="inputBlock ddlCommittee"
                                        Style="display: none !important">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="grid_2 h2">
                                    &nbsp;
                                </div>
                                <div class="grid_11">
                                    <input type="text" name="txtSpeakerOtherJob" id="txtSpeakerOtherJob" runat="server"
                                        class="textfield inputBlock txtSpeakerOtherJob" style="font-weight: bold; font-size: 16px;"
                                        size="26" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="grid_2 h2">
                                    &nbsp;
                                </div>
                                <div class="grid_11 h2">
                                    <input name="" id="sameAsPrevSpeaker" runat="server" class="sameAsPrevSpeaker" type="checkbox"
                                        value="" />
                                    <label class="ml-20">
                                        نفس المتحدث السابق</label>
                                    <input name="isSessionPresident" id="isSessionPresident" runat="server" class="isSessionPresident"
                                        type="checkbox" value="" />
                                    <label>
                                        رئيس الجلسة</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="grid_2 h2">
                                    &nbsp;
                                </div>
                                <div class="grid_11 h2">
                                    <input name="chkTopic" id="chkTopic" runat="server" class="chkTopic" type="checkbox"
                                        value="" />
                                    <label for="chkTopic" style="margin-left: 110px;display:none">
                                        تابع التوصية السابقه
                                    </label>
                                    <a href="javascript:void(0)" class="aPopupGetAttTopic" style="text-decoration: underline"
                                        runat="server" name="aPopupGetAttTopic" id="aPopupGetAttTopic">مقدمو الطلب</a>
                                </div>
                            </div>
                            <div class="row divTopic" id="divTopic" runat="server" name="divTopic">
                                <span style="color: Red">*</span> <span style="color: green">بداية التوصية / الاجراء</span>
                                <span id="spanTopicTitle" runat="server" name="spanTopicTitle" class="spanTopicTitle"></span>&nbsp;&nbsp;&nbsp;&nbsp; <a href="javascript:void(0)" class="removeTopic">حذف
                                التوصية</a>
                            </div>
                        </div>
                        <div class="grid_5">
                            <div class="user-image">
                                <img style="width: 200px" src="/images/unknown.jpg" id="imgSpeakerAvatar" name="imgSpeakerAvatar"
                                    runat="server" alt="" />
                            </div>
                        </div>
                    </div>
                    <div class="player_conatiner mb-20">
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
                        <textarea id="elm1" runat="server" name="elm1" rows="3" style="width: 100%" class="tinymce"></textarea>
                    </div>
                    <div class="row divAttach" id="divAttach" runat="server" name="divAttach">
                        <span style="color: Red">*</span> <span style="color: green">اسم المرفق: </span>
                        <span id="spanAttachTitle" runat="server" name="spanAttachTitle" class="spanAttachTitle"></span>&nbsp;&nbsp;&nbsp;&nbsp; <a href="javascript:void(0)" class="removeAttach">حذف
                        المرفق</a>
                    </div>
                    <div class="row divVote" id="divVote" runat="server" name="divVote">
                        <span style="color: Red">*</span> <span style="color: green;">اسم التصويت: </span>
                        <span id="spanVoteSubject" runat="server" name="spanVoteSubject" class="spanVoteSubject"></span>&nbsp;&nbsp;&nbsp;&nbsp; <a href="javascript:void(0)" class="removeVote">حذف
                        التصويت</a>
                    </div>
                    <div class="row">
                        <div class="grid_10">
                            <div class="h2">
                                تعليق على الفقرة
                            </div>
                            <textarea rows="3" id="txtComments" runat="server" class="textfield fitAll"></textarea>
                        </div>
                        <div class="grid_10">
                            <div class="h2">
                                تذيل الصفحة
                            </div>
                            <textarea rows="3" id="txtFooter" runat="server" class="textfield fitAll"></textarea>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="grid_21 suffix_3">
                </div>
            </div>
        </div>
        <!--Begin popup container div-->
        <div class="main_popup_container displaynone">
            <div id="inline1" class="popup_container_content tex_align_center">
                <h5 class="row">هل أنت موافق على العودة للنص الأصلي؟
                </h5>
                <div class="row">
                    سوف يتم إستبدال كل التعديلات التي قمت بها بالنص الأصلي الموجود، يرجي العلم بأنه
                في حال موافقتك علي إستبدال التعديلات بالنص الأصلي، فإنه لا يمكن العودة لتعديلاتك
                مرة أخري.
                </div>
                <div class="actions_btns">
                    <input type="button" class="smallbtn" id="yes" value="نعم">
                    <input type="button" class="smallbtn" id="no" value="لا">
                </div>
            </div>
        </div>
        <div class="popupoverlay">
        </div>
        <div class="reviewpopup_cont-st1 reviewpopup_cont1 graybg">
            <div class="close_btn">
            </div>
            <div class="clear">
            </div>
            <div class="datacontainer inputcont">
                <textarea id="Textarea1" runat="server" name="elm1" rows="3" style="width: 100%"
                    class="splittinymce"></textarea>
            </div>
            <div class="poppbtnscont fl">
                <div class="fl">
                    <input type="button" id="approve" class="approve1" value="موافق" />
                    <div class="clear">
                    </div>
                </div>
                <div class="clear">
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="reviewpopup_cont-st1 reviewpopup_cont2 graybg">
            <div class="close_btn">
            </div>
            <div class="clear">
            </div>
            <div class="row">
                <div class="grid_3 h2">
                    الاجراءات:
                </div>
                <div class="grid_7">
                    <asp:DropDownList ID="DropDownList1" AutoPostBack="false" runat="server">
                        <asp:ListItem Text="-------- اختر الاجراء --------" Value="0"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="clear">
                </div>
            </div>
            <div class="row">
                <div class="grid_6">
                    <ul id="listData" class="listData-st1">
                    </ul>
                </div>
                <div class="grid_14">
                    <div class="datacontainer inputcont">
                        <textarea id="Textarea2" runat="server" name="elm1" rows="3" style="width: 100%"
                            class="splittinymce"></textarea>
                    </div>
                </div>
            </div>
            <div class="poppbtnscont fl">
                <div class="fl">
                    <input type="button" id="Button2" class="approve2" value="موافق" />
                    <div class="clear">
                    </div>
                </div>
                <div class="clear">
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="reviewpopup_cont reviewpopup_cont3 graybg">
            <div class="close_btn">
            </div>
            <div class="clear">
            </div>
            <div>
                <h2 class="borderBD">
                    <span class="red">*</span> البند :
                </h2>
            </div>
            <div class="datacontainer inputcont datacontainer1">
                <textarea id="Textarea3" runat="server" name="elm1" rows="3" style="width: 100%"
                    class="splittinymce"></textarea>
            </div>
            <div>
                <input type="checkbox" class="isAgendaItemIndexed" id="isAgendaItemIndexed" runat="server"
                    value="1" />
                <label for="MainContent_isAgendaItemIndexed">
                    <strong style="font-size: 12pt;">بند رئيسى</strong></label>
            </div>
            <div class="poppbtnscont fl">
                <div class="fl">
                    <input type="button" id="Button3" class="approve3 btn " value="حفظ" />
                    <div class="clear">
                    </div>
                </div>
                <div class="clear">
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="reviewpopup_cont reviewpopup_cont4 graybg">
            <div class="close_btn">
            </div>
            <div class="clear">
            </div>
            <div class="borderBD">
                <h2>
                    <span class="red">*</span> المرفقات :
                </h2>
            </div>
            <div class="datacontainer inputcont datacontainer3">
                <div class="rdlattachments">
                </div>
            </div>
            <div class="poppbtnscont fl">
                <div class="fl" style="margin: 10px;">
                    <input type="button" id="btnAddAttach" class="btnAddAttach btn" value="أضف" />
                    <div class="clear">
                    </div>
                </div>
                <div class="clear">
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="reviewpopup_cont reviewpopup_cont5 graybg">
            <div class="close_btn">
            </div>
            <div class="clear">
            </div>
            <div class="borderBD row">
                <h2>
                    <span class="red">*</span> اضافة تصويت برفع الأيدى :
                </h2>
            </div>
             <div class="clear">
            </div>
            <div class="row">
                <div class="grid_4 h4"><span>اضافة تصويت جديد :</span></div>

                <div class="grid_8 ">
                    <input type="text" name="txtNewVote" id="txtNewVote" runat="server"
                        class="textfield inputBlock txtNewVote" style="font-weight: bold; font-size: 16px;"
                        size="26" />
                </div>
                <div class="grid_2 h2">
                    <input type="button" id="btnSaveVote" class="btnSaveVote btn" value="اضافة" /></div>
            </div>
            <div class="row">
                <div class="grid_4 h4"><span>او اختر من التصويتات :</span></div>
                <div class="grid_8">
                    <asp:DropDownList ID="ddl_votes" AutoPostBack="false" runat="server" CssClass="inputBlock ddl_votes">
                    </asp:DropDownList>
                </div>
            </div>
            <div class="datacontainer inputcont datacontainer3 attendantCont">
                <div id="AttendantCont" class="AttendantCont">
                </div>
            </div>
            <div class="poppbtnscont fl">
                <div class="fl" style="margin: 10px">
                    <input type="hidden" name="txtTpcID" class="txtTpcID" />
                    <input type="button" id="btnSaveVoteMems" class="btnSaveVoteMems btn" value="حفظ" />
                    <div class="clear">
                    </div>
                </div>
                <div class="clear">
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="reviewpopup_cont-st1 reviewpopup_cont6 graybg">
        <div class="close_btn">
        </div>
        <div class="clear">
        </div>
        <div class="borderBD">
            <h2>
                <span class="red">*</span> اضافة نقطة نظام :
            </h2>
        </div>
        <div class="row displaynone">
            <div class="grid_14 ">
                <div class="datacontainer inputcont">
                    <textarea id="elmManagePoint" runat="server" name="elmManagePoint" rows="3" style="width: 100%"
                        class="splittinymce"></textarea>
                </div>
            </div>
        </div>
        <div class="clear">
        </div>
        <div class="row" style="padding-top: 30px;">
            <div class="grid_16">
                <div class="clear">
                </div>
                <div class="grid_2 h2">
                    <span class="red"></span>المتحدث:
                </div>
                <div class="grid_11">
                    <asp:DropDownList ID="ddlSpeakersClone" AutoPostBack="false" runat="server" CssClass="inputBlock ddlSpeakersClone"
                        data-live-search="true">
                    </asp:DropDownList>
                </div>
            </div>
        </div>
        <div class="clear">
        </div>
        <div class="poppbtnscont fl">
            <div class="fl">
                <input type="button" id="btnSaveManagePoint" class="btnSaveManagePoint" value="أضف" />
                <div class="clear">
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="clear">
        </div>
    </div>
    <div class="reviewpopup_cont reviewpopup_cont7 graybg">
        <div class="close_btn">
        </div>
        <div class="clear">
        </div>
        <div class="borderBD">
            <h2>
                <span class="red">*</span> المقترحات / التوصيات :
            </h2>
        </div>
        <div class="datacontainer inputcont datacontainer3">
            <div class="rdltopics">
            </div>
        </div>
        <div class="poppbtnscont fl">
            <div class="fl" style="margin: 10px">
                <input type="button" id="btnSaveTopic" class="btnSaveTopic btn" value="أضف" />
                <div class="clear">
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="clear">
        </div>
    </div>
    <div class="reviewpopup_cont popupAttendant graybg" style="width: 1260px !important;
        left: 31% !important; height: auto; top: 450px;">
        <div class="close_btn">
        </div>
        <div class="clear">
        </div>
        <div class="borderBD">
            <h2>
                <span class="red">*</span> مقدموا الطلب :<span> (</span><span class="spnTpcTitlePopup"></span><span>
                    )</span>
            </h2>
        </div>
        <div class="datacontainer inputcont datacontainer3 attendantCont">
            <div id="AttCont" class="AttCont">
            </div>
        </div>
        <div class="poppbtnscont fl">
            <div class="fl" style="margin: 10px">
                <input type="hidden" name="txtTpcID" class="txtTpcID" />
                <input type="button" id="btnAddAttToTopic" class="btnAddAttToTopic btn" value="حفظ" />
                <div class="clear">
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="clear">
        </div>
    </div>
    </form>
</asp:Content>
