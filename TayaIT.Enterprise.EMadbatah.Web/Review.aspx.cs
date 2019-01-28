using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TayaIT.Enterprise.EMadbatah.BLL;
using TayaIT.Enterprise.EMadbatah.DAL;
using System.Text;
using TayaIT.Enterprise.EMadbatah.Config;
using TayaIT.Enterprise.EMadbatah.Model;
using System.IO;
using System.Collections;
using TayaIT.Enterprise.EMadbatah.Localization;
using TayaIT.Enterprise.EMadbatah.Util;


namespace TayaIT.Enterprise.EMadbatah.Web
{
    public partial class Review : BasePage
    {
        public Hashtable tblStats = null;
        int nRejected = -1;
        int nModefiedAfterApprove = -1;
        int nFixed = -1;
        public int bChecked = 0;
        public SessionDetails sd;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentUser.Role == Model.UserRole.DataEntry)
                Response.Redirect(Constants.PageNames.ERROR_PAGE + "?" + Constants.QSKeyNames.ERROR_TYPE + "=" + (int)ErrorType.Unauthorized);

            if (!Page.IsPostBack)
            {
                //now the dataentry-reviewr role is using filereviewer privilages instead od session reviewer
                isCurrentUserFileRev.Value = (CurrentUser.Role == UserRole.FileReviewer || CurrentUser.Role == UserRole.ReviewrDataEntry || CurrentUser.Role == UserRole.Reviewer).ToString().ToLower();
                currentUserID.Value = CurrentUser.ID.ToString();
                long sessionId;
                StringBuilder sb = new StringBuilder();
                bool disableEditforNotReviewrAdmin = false;

                if (SessionID != null && long.TryParse(SessionID, out sessionId))
                {
                    SessionIDHidden.Value = sessionId.ToString();
                    Session s = SessionHelper.GetSessionByID(sessionId);
                    tblStats = EMadbatahFacade.GetSessionStatistics(CurrentUser, s.ID);
                    sd = EMadbatahFacade.GetSessionDetailsBySessionID(sessionId);

                    if (CurrentUser.Role == UserRole.FileReviewer
                        //now the dataentry-reviewr role is using filereviewer privilages instead od session reviewer
                        || CurrentUser.Role == UserRole.ReviewrDataEntry)
                    {
                        var filesWithUser = from sfa in sd.SessionFiles
                                            where (sfa.FileReviewrID == CurrentUser.ID && sfa.IsActive == 1)
                                            select sfa;

                        var freeFiles = from sfa in sd.SessionFiles
                                        where (sfa.FileReviewrID == null && sfa.IsActive == 1)
                                        select sfa;
                        if ((freeFiles == null || freeFiles.Count<SessionAudioFile>() == 0) && filesWithUser != null && filesWithUser.Count<SessionAudioFile>() == 0)
                        {
                            ShowWarn(GetLocalizedString("strNoFreeFilesFound"));
                            disableEditforNotReviewrAdmin = true;
                        }
                        else if (freeFiles.Count<SessionAudioFile>() == 1)
                        {
                            ShowInfo(GetLocalizedString("strFileCanBeTaken"));
                        }
                        else if (freeFiles.Count<SessionAudioFile>() > 1)
                        {
                            ShowInfo(GetLocalizedString("strFilesCanBeTaken"));
                        }
                    }
                    //now the dataentry-reviewr role is using filereviewer privilages instead od session reviewer
                    if (s.ReviewerID == null && !(CurrentUser.Role == UserRole.FileReviewer) && !(CurrentUser.Role == UserRole.ReviewrDataEntry))
                        SessionHelper.UpdateSessionReviewer(s.ID, CurrentUser.ID);
                    else if (CurrentUser.Role == UserRole.Admin &&
                             s.ReviewerID != CurrentUser.ID)
                    {
                        ShowWarn(GetLocalizedString("strWrongReviewer"));
                        disableEditforNotReviewrAdmin = true;
                    }
                    else if (CurrentUser.Role != UserRole.Admin && s.ReviewerID != CurrentUser.ID && !(CurrentUser.Role == UserRole.FileReviewer || CurrentUser.Role == UserRole.ReviewrDataEntry)) // in case of reviewer or dataEntryReviewer
                    {
                        Response.Redirect(Constants.PageNames.ERROR_PAGE + "?" + Constants.QSKeyNames.ERROR_TYPE + "=" + (int)ErrorType.Unauthorized);
                    }

                    //handle sessionstart

                    SessionFile start = SessionStartFacade.GetSessionStartBySessionID(sessionId);


                    var NewSessionFiles = from sf2 in sd.SessionFiles
                                          where (sf2.Status == Model.SessionFileStatus.New && sf2.IsActive == 1)
                                          select sf2;

                    List<List<SessionContentItem>> allItems = SessionContentItemHelper.GetItemsBySessionIDGrouped(sessionId);

                    if (NewSessionFiles.ToList<SessionAudioFile>().Count == sd.SessionFiles.Where(c => c.IsActive == 1).ToList().Count &&
                        ((Model.SessionFileStatus)start.Status) == Model.SessionFileStatus.New || allItems.Count == 0)
                    {
                        ShowWarn(GetLocalizedString("strSessionStillWithNoWork"));
                        btnApproveSession.Style.Add("display", "none");
                        btnFinalApproveSession.Style.Add("display", "none");
                        madbatahContents.Style.Add("display", "none");
                    }
                    try
                    {
                        if (((Model.SessionFileStatus)start.Status) != Model.SessionFileStatus.New)
                        {
                            string reviewItem = Application[Constants.HTMLTemplateFileNames.ReviewItem].ToString()
                                                   .Replace("<%SessionContentItemID%>", start.ID.ToString())
                                                   .Replace("<%itemText%>", start.SessionStartText);
                            if (disableEditforNotReviewrAdmin)
                                reviewItem = reviewItem.Replace("<%isLocked%>", "lockeditem").Replace("<%title%>", "لا يحق لك التعديل .. يمكنك الاطلاع فقط");
                            else if (((Model.SessionFileStatus)start.Status) == Model.SessionFileStatus.InProgress)
                                reviewItem = reviewItem.Replace("<%isLocked%>", "lockeditem").Replace("<%title%>", "قيد التعديل الآن");
                            else
                                reviewItem = reviewItem.Replace("<%isLocked%>", "openeditem")
                                                       .Replace("<%title%>", "");

                            reviewItem = reviewItem.Replace("<%FileRevName%>", start.FileReviewer != null ? start.FileReviewer.FName : "لا يوجد")
                           .Replace("<%FileName%>", "بداية المضبطة")
                           .Replace("<%UserName%>", start.User.FName)
                           .Replace("<%RevName%>", sd.ReviewerName);

                            reviewItem = reviewItem.Replace("<%SessionFileID%>", start.ID.ToString()).Replace("<%RevNote%>", start.SessionStartReviewNote);
                            reviewItem = reviewItem.Replace("<%IsSessionStart%>", "1");

                            reviewItem = reviewItem.Replace("<%MP3FilePath%>", "");
                            reviewItem = reviewItem.Replace("<%MP3FileStartTime%>", "");
                            reviewItem = reviewItem.Replace("<%MP3FileEndTime%>", "");

                            switch ((Model.SessionFileStatus)start.Status)
                            {
                                case Model.SessionFileStatus.SessionStartApproved: //approved
                                    reviewItem = reviewItem.Replace("<%Color%>", "");
                                    break;
                                case Model.SessionFileStatus.SessionStartRejected: //rejected
                                    reviewItem = reviewItem.Replace("<%Color%>", "reditem");
                                    break;
                                case Model.SessionFileStatus.SessionStartFixed: //fixed
                                    reviewItem = reviewItem.Replace("<%Color%>", "greenitem");
                                    break;
                                case Model.SessionFileStatus.SessionStartModifiedAfterApprove: //modified
                                    reviewItem = reviewItem.Replace("<%Color%>", "blueitem");
                                    break;
                            }
                            sb.Append(reviewItem);

                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogException(ex, "SessionID: " + sessionId);

                    }


                    sessionDate.InnerText = s.Date.ToString();
                    sessionSerial.InnerText = "( " + s.EParliamentID + " )";



                    List<long> WrittenAgendaItemWithGroupedSubItems = new List<long>();
                    int pageNum = 0;
                    int ii = 0;
                    List<string> writtenAgendaItems = new List<string>();
                    long lastSFID = 0;
                    long currentSpeaker = 0;
                    long prevSpeaker = 0;
                    long topic_id = 0;
                    foreach (List<SessionContentItem> groupedItems in allItems)
                    {
                       foreach (SessionContentItem item in groupedItems)
                        {
                            AgendaItem curAgendaItem = item.AgendaItem;
                            string originalName = curAgendaItem.Name;
                            writtenAgendaItems.Add(curAgendaItem.Name);
                            string updatedAgendaName = "";
                            if (curAgendaItem.Name != "غير معرف")
                            {
                                updatedAgendaName = curAgendaItem.Name;
                                string agendaItem = Application[Constants.HTMLTemplateFileNames.ReviewItemAgendaItem].ToString()
                                                        .Replace("<%itemText%>", "* " + updatedAgendaName + ":");
                                sb.Append(agendaItem);
                            }

                            if (lastSFID == 0)
                                lastSFID = item.SessionFileID.Value;

                            bool insertSeparator = false;
                            if (lastSFID != item.SessionFileID.Value)
                            {
                                insertSeparator = true;
                                lastSFID = item.SessionFileID.Value;
                                sb.Append("<hr id=\"file_" + lastSFID + "\" />");
                            }
                            currentSpeaker = item.AttendantID;

                          /*  if (currentSpeaker != prevSpeaker && topic_id != 0)
                            {
                                //for Topics
                                string reviewItemTopic = write_topic_att(topic_id, item);
                                if (reviewItemTopic != "")
                                    sb.Append(reviewItemTopic);
                                topic_id = 0;
                            }*/

                            if ((item.TopicID != null && item.TopicID != 0 && item.MergedTopicWithPrevious != null && !(bool)item.MergedTopicWithPrevious))
                            {
                                if (topic_id != 0)
                                {
                                    string reviewItemTopic = write_topic_att(topic_id, item);
                                    if (reviewItemTopic != "")
                                        sb.Append(reviewItemTopic);
                                    topic_id = 0;
                                }
                                topic_id = (long)item.TopicID;
                            }

                            if (item.TopicID == null || item.TopicID == 0 && topic_id != 0)
                            {
                                string reviewItemTopic = write_topic_att(topic_id, item);
                                if (reviewItemTopic != "")
                                    sb.Append(reviewItemTopic);
                                topic_id = 0;
                            }

                            if (currentSpeaker != prevSpeaker)//if (!item.MergedWithPrevious.Value)
                            {
                                Attendant att = item.Attendant;
                                prevSpeaker = att.ID;


                                if (att.Type != (int)Model.AttendantType.UnAssigned)
                                {
                                    string attFullPresentationName = "";
                                    string name = "";
                                    string job = "";
                                    string job2 = "";
                                    if ((Model.AttendantType)att.Type == Model.AttendantType.President)
                                    {
                                        string speaker = Application[Constants.HTMLTemplateFileNames.ReviewItemSpeaker].ToString()
                                                       .Replace("<%itemText%>", "السيد الرئيـــــــــــــــــــــــــــس :")
                                                       .Replace("<%speakerJob%>", "")
                                                       .Replace("<%speakerJob2%>", "");
                                        sb.Append(speaker);
                                    }
                                    else
                                    {
                                        if (item.IsSessionPresident == 1)
                                        {
                                            name = "السيد رئيـس الجلســـــــــــــــــــــــة :";

                                            if (att.AttendantTitle == null)
                                                attFullPresentationName = "السيد " + att.LongName.Trim();
                                            else attFullPresentationName = att.AttendantTitle.Trim() + " " + att.LongName.Trim();
                                            attFullPresentationName = "( " + attFullPresentationName;
                                            if (String.IsNullOrEmpty(att.JobTitle))
                                                attFullPresentationName = attFullPresentationName + ")";

                                            if (!String.IsNullOrEmpty(att.JobTitle))
                                                job2 = "    " + att.JobTitle + ")";
                                            else job2 = " )";
                                            string speaker = Application[Constants.HTMLTemplateFileNames.ReviewItemSpeaker].ToString()
                                                            .Replace("<%itemText%>", name)
                                                            .Replace("<%speakerJob%>", attFullPresentationName)
                                                            .Replace("<%speakerJob2%>", job2);
                                            sb.Append(speaker);
                                        }
                                        else
                                        {
                                            if (att.AttendantTitle == null)
                                                attFullPresentationName = "السيد " + att.Name.Trim();
                                            else attFullPresentationName = att.AttendantTitle.Trim() + " " + att.Name.Trim();
                                            if (!String.IsNullOrEmpty(att.JobTitle))
                                                job = att.JobTitle;
                                            if (!string.IsNullOrEmpty(item.CommentOnAttendant))
                                                job2 = "    " + item.CommentOnAttendant;

                                            string speaker = Application[Constants.HTMLTemplateFileNames.ReviewItemSpeaker].ToString()
                                                     .Replace("<%itemText%>", attFullPresentationName)
                                                     .Replace("<%speakerJob%>", job)
                                                     .Replace("<%speakerJob2%>", "");
                                            sb.Append(speaker);
                                        }
                                    }
                                }
                                else
                                {
                                    string speaker = Application[Constants.HTMLTemplateFileNames.ReviewItemSpeaker].ToString()
                                                       .Replace("<%itemText%>", "تم اسناد هذه الفقرة الى :" + att.Name)
                                                       .Replace("<%speakerJob%>", "")
                                                       .Replace("<%speakerJob2%>", "");
                                    sb.Append(speaker);
                                }
                                ////////////////////////////////
                            }

                            string reviewItem = Application[Constants.HTMLTemplateFileNames.ReviewItem].ToString()
                                                    .Replace("<%SessionContentItemID%>", item.ID.ToString())
                                                    .Replace("<%itemText%>", item.Text);

                            if (disableEditforNotReviewrAdmin)
                                reviewItem = reviewItem.Replace("<%isLocked%>", "lockeditem").Replace("<%title%>", "لا يحق لك التعديل .. يمكنك الاطلاع فقط");
                            else if (((Model.SessionFileStatus)item.SessionFile.Status) == Model.SessionFileStatus.InProgress)
                                reviewItem = reviewItem.Replace("<%isLocked%>", "lockeditem")
                                    .Replace("<%title%>", "قيد التعديل الآن");

                            else if (CurrentUser.Role == UserRole.FileReviewer && item.SessionFile.FileReviewer != null && item.SessionFile.FileReviewerID != CurrentUser.ID)
                                reviewItem = reviewItem.Replace("<%isLocked%>", "lockeditem").Replace("<%title%>", "لا يحق لك التعديل .. يمكنك الاطلاع فقط");
                            else
                                reviewItem = reviewItem.Replace("<%isLocked%>", "openeditem")
                                    .Replace("<%title%>", "");


                            reviewItem = reviewItem.Replace("<%FileRevName%>", item.SessionFile.FileReviewer != null ? item.SessionFile.FileReviewer.FName : "لا يوجد")
                                .Replace("<%FileName%>", Path.GetFileName(item.SessionFile.Name))
                                .Replace("<%UserName%>", item.User == null ? "لا يوجد" : item.User.FName)
                                .Replace("<%RevName%>", sd.ReviewerName);



                            if (item.SessionFile.FileReviewerID != null)
                                reviewItem = reviewItem.Replace("<%FileRevID%>", item.SessionFile.FileReviewer.ID.ToString());
                            else
                                reviewItem = reviewItem.Replace("<%FileRevID%>", "");

                            reviewItem = reviewItem.Replace("<%SessionFileID%>", item.SessionFileID.ToString()).Replace("<%RevNote%>", item.ReviewerNote);

                            string mp3FilePath = string.Format("{0}://{1}:{2}/", Request.Url.Scheme, Request.Url.Host, Request.Url.Port) + item.SessionFile.Name.Substring(1).Replace(@"\", "/");
                            reviewItem = reviewItem.Replace("<%MP3FilePath%>", mp3FilePath);
                            reviewItem = reviewItem.Replace("<%MP3FileStartTime%>", item.StartTime.ToString());
                            reviewItem = reviewItem.Replace("<%MP3FileEndTime%>", item.EndTime.ToString());
                            reviewItem = reviewItem.Replace("<%IsSessionStart%>", "0");

                            switch ((Model.SessionContentItemStatus)item.StatusID)
                            {
                                case Model.SessionContentItemStatus.DataEntryApproved:
                                    reviewItem = reviewItem.Replace("<%Color%>", "brownitem");
                                     break;
                                case Model.SessionContentItemStatus.Approved: //approved
                                    reviewItem = reviewItem.Replace("<%Color%>", "");
                                    break;
                                case Model.SessionContentItemStatus.Rejected: //rejected
                                    reviewItem = reviewItem.Replace("<%Color%>", "reditem");
                                    break;
                                case Model.SessionContentItemStatus.Fixed: //fixed
                                    reviewItem = reviewItem.Replace("<%Color%>", "greenitem");
                                    break;
                                case Model.SessionContentItemStatus.ModefiedAfterApprove: //modified
                                    reviewItem = reviewItem.Replace("<%Color%>", "blueitem");
                                    break;
                            }
                            sb.Append(reviewItem);

                           /* if (item.TopicID != null && item.TopicID != 0)
                            {
                                topic_id = (long)item.TopicID;
                            }*/

                            if (item.ID == groupedItems[groupedItems.Count-1].ID && topic_id != 0)
                            {
                                //for Topics
                                string reviewItemTopic = write_topic_att(topic_id, item);
                                if (reviewItemTopic != "")
                                    sb.Append(reviewItemTopic);
                                topic_id = 0;
                            }

                            //for comments
                            if (!string.IsNullOrEmpty(item.CommentOnText))
                            {
                                string reviewItemComment = Application[Constants.HTMLTemplateFileNames.ReviewItem].ToString()
                                       .Replace("<%SessionContentItemID%>", item.ID.ToString())
                                       .Replace("<%itemText%>", item.CommentOnText)
                                       .Replace("<%isLocked%>", "lockeditem")
                                       .Replace("<%title%>", "هذا المقطع تعليق (للتعديل يمكنك استخدام خيارات تعديل أكثر للمقطع السابق) .. يمكنك الاطلاع فقط")
                                       .Replace("<%FileRevName%>", item.SessionFile.FileReviewer != null ? item.SessionFile.FileReviewer.FName : "لا يوجد")
                                        .Replace("<%FileName%>", Path.GetFileName(item.SessionFile.Name))
                                        .Replace("<%UserName%>", item.User.FName)
                                        .Replace("<%RevName%>", sd.ReviewerName + "\r\n<br/>(للتعديل يمكنك استخدام خيارات تعديل أكثر للمقطع السابق) هذا المقطع تعليق");
                                sb.Append(reviewItemComment);
                            }
                            //for footnote
                            if (!string.IsNullOrEmpty(item.PageFooter))
                            {
                                string reviewFootNote = Application[Constants.HTMLTemplateFileNames.ReviewItem].ToString()
                                        .Replace("<%SessionContentItemID%>", item.ID.ToString())
                                        .Replace("<%itemText%>", item.PageFooter)
                                        .Replace("<%isLocked%>", "lockeditem")
                                        .Replace("<%title%>", "هذا المقطع تذييل صفحة (للتعديل يمكنك استخدام خيارات تعديل أكثر للمقطع السابق) .. يمكنك الاطلاع فقط")
                                        .Replace("<%FileRevName%>", item.SessionFile.FileReviewer != null ? item.SessionFile.FileReviewer.FName : "لا يوجد")
                                        .Replace("<%FileName%>", Path.GetFileName(item.SessionFile.Name))
                                        .Replace("<%UserName%>", item.User.FName)
                                        .Replace("<%RevName%>", sd.ReviewerName + "\r\n<br/>(للتعديل يمكنك استخدام خيارات تعديل أكثر للمقطع السابق) هذا المقطع تذييل صفحة");
                                sb.Append(reviewFootNote);
                            }
                            /*}*/
                        }
                    }
                    madbatahContents.InnerHtml += sb.ToString();

                    if (CurrentUser.Role == UserRole.ReviewrDataEntry)
                        disableEditforNotReviewrAdmin = true;

                    SessionFile sf = SessionStartHelper.GetSessionStartBySessionId(sessionId);

                    bool IsSessionContainUndefinedAttendant = SessionContentItemHelper.DoesSessionContainsUndefinedAttendants(sessionId);
                    if (IsSessionContainUndefinedAttendant)
                    {
                        string message = "لا يمكن ظهور الموافقة على الجلسة أو التصديق عليها في وجود أي مقطع لمتحدث غير معرف";
                        message += "، لتعديلها يرجى فحص المقاطع التالية: ";
                        List<SessionContentItem> undefAttContItems = SessionContentItemHelper.GetSessionContentItemsOfUndefinedAttendants(sessionId);
                        int ctr = 1;
                        foreach (SessionContentItem item in undefAttContItems)
                        {
                            string toolTip = "الملف: " + new FileInfo(item.SessionFile.Name).Name + "<br/>";
                            if (item.User != null)
                                toolTip += "مدخل البيانات: " + item.User.FName;
                            else
                                toolTip += "مدخل البيانات: ";
                            //toolTip += "مراجع الملف: " + item.FileReviewer.FName;

                            message += "<a title=\"" + toolTip + "\" class=\"info\" href=\"EditSessionFile.aspx?scid=" + item.ID + "&sfid=" + item.SessionFileID + "&editmode=3&sid=" + sessionId + "\">" + ctr + "</a> , ";
                            ctr++;
                        }
                        ShowWarn(message);
                    }


                    Model.SessionStatus sessionStatus = (Model.SessionStatus)s.SessionStatusID;
                    if (sessionStatus == Model.SessionStatus.New ||
                        sessionStatus == Model.SessionStatus.InProgress ||
                        sessionStatus == Model.SessionStatus.FinalApproved ||
                        SessionContentItemHelper.GetSessionContentItemsBySessionIDAndNotStatusID(sessionId, (int)Model.SessionContentItemStatus.Approved).Count != 0 ||
                        IsSessionContainUndefinedAttendant)
                    {
                        btnApproveSession.Style.Add("display", "none");
                        btnFinalApproveSession.Style.Add("display", "none");
                    }
                    else if (sessionStatus == Model.SessionStatus.Completed)
                    {
                        btnApproveSession.Style.Add("display", "inline");
                        btnFinalApproveSession.Style.Add("display", "none");
                    }
                    else if (sessionStatus == Model.SessionStatus.Approved)
                    {
                        btnApproveSession.Style.Add("display", "none");
                        btnFinalApproveSession.Style.Add("display", "inline");
                    }
                }
                else
                {
                    ShowMainError(GetLocalizedString("strNoQueryStr"));
                }

                if (disableEditforNotReviewrAdmin)
                {
                    btnApproveSession.Style.Add("display", "none");
                    btnFinalApproveSession.Style.Add("display", "none");
                }
            }
        }

        public string write_topic_att(long topic_id, SessionContentItem item)
        {
        
            return "";
        }
    }
}