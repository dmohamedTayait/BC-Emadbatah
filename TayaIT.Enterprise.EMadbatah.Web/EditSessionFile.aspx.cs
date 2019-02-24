using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TayaIT.Enterprise.EMadbatah.Model.VecSys;
using TayaIT.Enterprise.EMadbatah.Vecsys;
using TayaIT.Enterprise.EMadbatah.Model;
using TayaIT.Enterprise.EMadbatah.BLL;
using TayaIT.Enterprise.EMadbatah.DAL;
using System.Collections;
using TayaIT.Enterprise.EMadbatah.Util.Web;
using TayaIT.Enterprise.EMadbatah.Config;
using System.Text;

namespace TayaIT.Enterprise.EMadbatah.Web
{
    public partial class EditSessionFile : BasePage
    {
        

        long sessionId;
        long session_file_id;
        Session current_session;
        public SessionAudioFile file;
        private EditorPageMode currentPageMode = EditorPageMode.Edit;
        protected void Page_Load(object sender, EventArgs e)
        {
            int tmpPageMode = 1;
            if(EditPageMode != null && 
                int.TryParse(EditPageMode, out tmpPageMode))
            {
                currentPageMode = (EditorPageMode)tmpPageMode;
            }

            hdPageMode.Value = ((int)currentPageMode).ToString();
                

            if (SessionFileID != null && long.TryParse(SessionFileID, out session_file_id))
            {
                // load the current session details and file details
                bool flag = false;
                file = SessionFileFacade.GetSessionFilesByID(session_file_id);
                if (file.UserID == null)
                {
                    flag = SessionFileFacade.LockSessionFile(session_file_id, CurrentUser.ID, false);
                }

                current_session = EditorFacade.GetSessionByID(file.SessionID);

                if (CurrentUser.ID == file.UserID || flag
                    || ((CurrentUser.Role != UserRole.DataEntry) && (current_session.ReviewerID == CurrentUser.ID))
                    || ((CurrentUser.Role != UserRole.DataEntry) && (file.FileReviewrID == CurrentUser.ID)))
                {
                    sessionId = file.SessionID;
                    sessionID.Value = sessionId.ToString();
                    bool outReedit = false;
                    if (ReEdit != null && bool.TryParse(ReEdit, out outReedit))
                    {
                        if (outReedit)
                        {
                            SessionFileHelper.UpdateSessionFileFragOrderAndModifiedDate(session_file_id, 0, DateTime.Now);
                            file.LastInsertedFragNumInXml = 0;
                        }
                    }
                    // bind data
                    BindData();
                }
                else
                {
                    // redirect to message notifaction page ""
                    editSessionFileForm.Visible = false;
                    ShowWarn("عفواً . غير مسموح لك بالتعديل في هذه الصفحة");
                }

            }
        }
        private string getHTML(Paragraph p)
        {
            string output = "";
            foreach (SpeechSegment segment in p.speechSegmentsList)
            {
                foreach (TayaIT.Enterprise.EMadbatah.Model.VecSys.Word word in segment.words)
                {
                   // output += "<span class='segment' data-stime='" + word.stime.ToString() + "'>" + word.value.Replace(".", "،") +"</span> ";
                    if (word.stime.ToString() == "0")
                    {
                        if (word.procedureid == "-1")
                            output += "<br />";
                        else
                            output += "<p procedure-id='" + word.procedureid.ToString() + "' style='text-align: " + word.textalign.ToString() + "'>" + word.value.Replace(".", "،").Replace("!####!", "&nbsp;") + "</p> ";
                    }
                    else
                        output += "<span class='segment' data-stime='" + word.stime.ToString() + "'>" + word.value.Replace(".", "،").Replace("!####!", "&nbsp;") + "</span> ";
                }
            }
            return output;
        }
        public string agendaItemTxt = "";
        public string agendaItemIsIndexed = "0";
        private void BindData()
        {
            string html; // editor rendered HTML text
            Hashtable current_session_info = new Hashtable(); // to store required values in user session
            //int FragOrder = Request.QueryString["action"] == "continue" ? (int)file.LastInsertedFragNumInXml : 0;
            int FragOrder = (int)file.LastInsertedFragNumInXml;


            if (currentPageMode == EditorPageMode.Review)
            {
                SessionContentItem currContItem = SessionContentItemHelper.GetSessionContentItemById(long.Parse(SessionContentItemID));
                FragOrder = currContItem.FragOrderInXml;
                btnSaveOnly.Attributes.Add("disbaled", "disbaled");
                btnSaveAndExit.Attributes.Add("disbaled", "disbaled");
            }

            if (currentPageMode == EditorPageMode.Edit && SessionContentItemID != null)
            {
                SessionContentItem currContItem = SessionContentItemHelper.GetSessionContentItemById(long.Parse(SessionContentItemID));
                FragOrder = currContItem.FragOrderInXml;
            }
            //ibrahim: need to know how many fargments are there in the file and this will be done once
            // set XML,MP3 file path
            string xmlFilePath = Context.Server.MapPath("~") + file.Name.ToLower().Replace(".mp3", ".trans.xml");
            string mp3FilePath = string.Format("{0}://{1}:{2}/", Request.Url.Scheme, Request.Url.Host, Request.Url.Port) + file.Name.Substring(1).Replace(@"\", "/");
            //AppConfig.GetInstance().RootUrl + file.Name.Substring(1).Replace(@"\", "/");
            XmlFilePath.Value = xmlFilePath;
            MP3FilePath.Value = mp3FilePath;
            // parse XML file
            TransFile tf = new TransFile();
            tf = VecsysParser.ParseTransXml(xmlFilePath);
            List<Paragraph> p = VecsysParser.combineSegments(tf.SpeechSegmentList);

            SessionContentItem lastContentItem = null;
            long tmpSessionContentItemID = 0;
            if ((currentPageMode == EditorPageMode.Correct || currentPageMode == EditorPageMode.Review)
                && SessionContentItemID != null
                && long.TryParse(SessionContentItemID, out tmpSessionContentItemID))
            {
                lastContentItem = SessionContentItemHelper.GetSessionContentItemById(tmpSessionContentItemID);
            }
            else
            {
                lastContentItem = SessionContentItemFacade.GetSessionContentItemByIdAndFragmentOrder(session_file_id, FragOrder);
            }
            //corrector

            divTopic.Style.Add("display", "none");
            if (lastContentItem == null) // load from xml
            {
                //// compose HTML spans from speech segments and set editor value
                html = getHTML(p[FragOrder]);
                current_session_info["PargraphStartTime"] = p[FragOrder].speechSegmentsList[0].stime;
                current_session_info["PargraphOriginalStartTime"] = p[FragOrder].speechSegmentsList[0].stime;
                current_session_info["PargraphEndTime"] = p[FragOrder].speechSegmentsList[p[FragOrder].speechSegmentsList.Count - 1].etime;
                startTime.Value = p[FragOrder].speechSegmentsList[0].stime.ToString();
                originalStartTime.Value = p[FragOrder].speechSegmentsList[0].stime.ToString();
                endTime.Value = p[FragOrder].speechSegmentsList[p[FragOrder].speechSegmentsList.Count - 1].etime.ToString();

                if (FragOrder != 0)
                {
                    SessionContentItem prevContentItem = SessionContentItemFacade.GetSessionContentItemByIdAndFragmentOrder(session_file_id, FragOrder - 1);
                    current_session_info["PargraphStartTime"] = prevContentItem.EndTime;
                    startTime.Value = prevContentItem.EndTime.ToString();
                }
            }
            else // load from DB
            {
                html = lastContentItem.Text;
                current_session_info["PargraphStartTime"] = lastContentItem.StartTime;
                current_session_info["PargraphEndTime"] = lastContentItem.EndTime;
                current_session_info["PargraphOriginalStartTime"] = lastContentItem.OriginalStartTime;
                hdSessionContentItemID.Value = lastContentItem.ID.ToString();//tmpSessionContentItemID.ToString(); ;
                startTime.Value = lastContentItem.StartTime.ToString();
                endTime.Value = lastContentItem.EndTime.ToString();
                originalStartTime.Value = lastContentItem.OriginalStartTime.ToString();
            }
            // set editor text
            elm1.Value = html;

            long prevTopicID = 0;
            //get prev item from db if exist
            if (FragOrder - 1 >= 0)
            {
                SessionContentItem prevContentItem = SessionContentItemHelper.GetPrevSessionContentItem(session_file_id, FragOrder);
                if (prevContentItem != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<script type=\"text/javascript\">");
                    sb.Append("prevAgendaItemIndex=").Append(prevContentItem.AgendaItemID).Append(";");
                    sb.Append("prevSpeakerIndex=").Append(prevContentItem.AttendantID).Append(";");

                    if (prevContentItem.AgendaSubItemID != null)
                        sb.Append("prevAgendaSubItemIndex=").Append(prevContentItem.AgendaSubItemID).Append(";");
                    if (prevContentItem.Attendant.JobTitle != null && !string.IsNullOrEmpty(prevContentItem.Attendant.JobTitle))
                        sb.Append("prevSpeakerTitle=\"").Append(prevContentItem.Attendant.JobTitle).Append("\";");
                    sb.Append("</script>");
                    ClientScript.RegisterClientScriptBlock(typeof(string), "abbas", sb.ToString());
                    prevTopicID = prevContentItem.TopicID != null ? (long)prevContentItem.TopicID : 0;
                    //ClientScript.RegisterClientScriptBlock(typeof(string), "abbas", "<script type=\"text/javascript\">prevAgendaItemIndex=" + prevContentItem.AgendaItemID + ";prevAgendaSubItemIndex=" + prevContentItem.AgendaSubItemID == null ? "''" : prevContentItem.AgendaSubItemID
                    //    + ";prevSpeakerIndex=" + prevContentItem.AttendantID + ";prevSpeakerTitle=" + prevContentItem.Attendant.JobTitle == null ? "''" : prevContentItem.Attendant.JobTitle + ";</script>");
                }
            }

            //bind Speakers Drop Down List
            ListItem li;
            long unAssignedId = 0;
            foreach (Attendant item in current_session.Attendants.OrderBy(s => s.LongName).ThenBy(s => s.CreatedAt))
            {
                ListItem liAttendant = new ListItem((item.AttendantDegree + " " +  item.LongName).Trim(), item.ID.ToString());
                ddlSpeakers.Items.Add(liAttendant);
                if (item.LongName == "غير محدد")
                    unAssignedId = item.ID;
            }
            unAssignedSpeakerId.Value = unAssignedId.ToString();

            li = new ListItem("-------- اختر المتحدث --------", "0");
            ddlSpeakers.Items.Insert(0, li);

            li = new ListItem("أخرى", "1.5");
            ddlSpeakers.Items.Add(li);

           //Bind Available Attachments
            string attachName = "";
            string topictitle = "";
           
            AgendaItem sessionUnknownItem = AgendaHelper.GetAgendaItemByNameAndSessionID("غير معرف", current_session.ID);
            unAssignedAgendaId.Value = sessionUnknownItem.ID.ToString();

            if (lastContentItem != null)
            {
                Attendant attendObj = AttendantHelper.GetAttendantById(lastContentItem.AttendantID);
                if (attendObj != null && attendObj.AttendantAvatar != null && attendObj.AttendantAvatar != "")
                {
                    imgSpeakerAvatar.Src = "/images/AttendantAvatars/" + attendObj.AttendantAvatar;
                }
                else
                {
                    imgSpeakerAvatar.Src = "/images/unknown.jpg";
                }

                if (attendObj != null && attendObj.JobTitle != null)
                {
                    txtSpeakerJob.InnerText = attendObj.JobTitle;
                }
                else
                {
                    txtSpeakerJob.InnerText = "";
                }
                if (lastContentItem.AgendaItemID != null)
                {
                    // btn_addNewAgendaItem.Style.Add("display", "none");
                    agendaItemTxt = lastContentItem.AgendaItem.Name;
                    agendaItemIsIndexed = lastContentItem.AgendaItem.IsIndexed.ToString();
                    agendaItemId.Value = lastContentItem.AgendaItem.ID.ToString();
                    if (lastContentItem.AgendaItem.Name == "غير معرف")
                        divAgenda.Style.Add("display", "none");
                    else
                        divAgenda.Style.Add("display", "");
                }
                else
                {
                    agendaItemId.Value = lastContentItem.AgendaItem.ID.ToString();
                    btn_addNewAgendaItem.Style.Add("display", "");
                }

                if (lastContentItem.AttachementID != null && lastContentItem.AttachementID != 0)
                {
                    Attachement attachObj = AttachmentHelper.GetAttachementByID((int)lastContentItem.AttachementID);
                    attachName = attachObj.Name;
                    attachId.Value = lastContentItem.AttachementID.ToString();
                    divAttach.Style.Add("display", "");
                    spanAttachTitle.InnerHtml = attachName;
                }
                else
                {
                    attachId.Value = "0";
                    divAttach.Style.Add("display", "none");
                    spanAttachTitle.InnerHtml = "";
                }

                if ((lastContentItem.TopicID == null || lastContentItem.TopicID == 0) && (prevTopicID == null || prevTopicID == 0))
                {
                    divTopic.Style.Add("display", "none");
                    chkTopic.Checked = false;
                    chkTopic.Disabled = true;
                   // btnAddNewTopic.Disabled = false;
                    aPopupGetAttTopic.Style.Add("display", "none");
                }
                else if ((lastContentItem.TopicID != null && lastContentItem.TopicID != 0) && (prevTopicID == null || prevTopicID == 0))
                {
                    topicId.Value = lastContentItem.TopicID.ToString();
                    divTopic.Style.Add("display", "");
                    chkTopic.Checked = false;
                    chkTopic.Disabled = true;
                   // btnAddNewTopic.Disabled = true;
                    aPopupGetAttTopic.Style.Add("display", "");
                    if (!(bool)lastContentItem.MergedTopicWithPrevious)
                    {
                        chkTopic.Checked = false;
                        divTopic.Style.Add("display", "block");
                    }
                    else
                    {
                        chkTopic.Checked = true;
                    }
                }
                else if ((lastContentItem.TopicID == null || lastContentItem.TopicID == 0) && (prevTopicID != null && prevTopicID != 0))
                {
                    topicId.Value = lastContentItem.TopicID.ToString();
                    prevTopicId.Value = prevTopicID.ToString();
                    divTopic.Style.Add("display", "none");
                    chkTopic.Checked = false;
                    chkTopic.Disabled = false;
                 //   btnAddNewTopic.Disabled = true;
                    aPopupGetAttTopic.Style.Add("display", "none");
                }
                else if ((lastContentItem.TopicID != null && lastContentItem.TopicID != 0) && (prevTopicID != null && prevTopicID != 0))
                {
                    topicId.Value = lastContentItem.TopicID.ToString();
                    prevTopicId.Value = prevTopicID.ToString();
                    divTopic.Style.Add("display", "none");
                    if (!(bool)lastContentItem.MergedTopicWithPrevious)
                    {
                        chkTopic.Checked = false;
                    }
                    else
                    {
                        chkTopic.Checked = true;
                    }
                    if (lastContentItem.TopicID != prevTopicID)
                    {
                        divTopic.Style.Add("display", "block");
                    } 
                    chkTopic.Disabled = false;
                  //  btnAddNewTopic.Disabled = false;
                    aPopupGetAttTopic.Style.Add("display", "");
                }/*
                if (lastContentItem.TopicID != null && lastContentItem.TopicID != 0)
                {
                    topicId.Value = lastContentItem.TopicID.ToString();
                    chkTopic.Checked = false;
                    chkTopic.Disabled = true;
                    btnAddNewTopic.Disabled = false;
                    aPopupGetAttTopic.Style.Add("display", "none");
                }
                else
                {
                    topicId.Value = "0";
                    spanTopicTitle.InnerHtml = "";
                    chkTopic.Checked = false;
                    chkTopic.Disabled = true;
                    btnAddNewTopic.Disabled = false;
                    aPopupGetAttTopic.Style.Add("display", "none");
                }
                */

                string voteSubject = "";
                voteId.Value = "0";
                divVote.Style.Add("display", "none");
                spanVoteSubject.InnerHtml = "";
                 if (lastContentItem.VotingID != null && lastContentItem.VotingID != 0)
                 {
                     Vote voteObj = VoteHelper.GetSessionVote((long)lastContentItem.VotingID);
                     voteSubject = voteObj.VoteSubject;
                     voteId.Value = lastContentItem.VotingID.ToString();
                     divVote.Style.Add("display", "");
                     spanVoteSubject.InnerHtml = voteSubject;
                 }
                 else
                 {
                     voteId.Value = "0";
                     divVote.Style.Add("display", "none");
                     spanVoteSubject.InnerHtml = "";
                 }

                ddlSpeakers.SelectedValue = lastContentItem.AttendantID.ToString();
                //ddlSpeakers.t
                txtSpeakerOtherJob.Value = lastContentItem.CommentOnAttendant;//usama for job title
                txtComments.InnerText = lastContentItem.CommentOnText;
                txtFooter.InnerText = lastContentItem.PageFooter;

                if (lastContentItem.AgendaItem.IsGroupSubAgendaItems)
                {
                    chkGroupSubAgendaItems.Attributes.Add("checked", "checked");
                    ddlAgendaSubItems.Attributes.Add("disabled", "disabled");
                }
                else
                    chkGroupSubAgendaItems.Attributes.Remove("checked");

                if (lastContentItem.MergedWithPrevious == null || lastContentItem.MergedWithPrevious == false)
                    sameAsPrevSpeaker.Attributes.Remove("checked");
                else
                    sameAsPrevSpeaker.Attributes.Add("checked", "checked");


                if (lastContentItem.IsSessionPresident == null || lastContentItem.IsSessionPresident == 0)
                    isSessionPresident.Attributes.Remove("checked");
                else
                    isSessionPresident.Attributes.Add("checked", "checked");

                if (lastContentItem.Ignored == null || lastContentItem.Ignored == false)
                    // chkIgnoredSegment.Attributes.Remove("checked");
                    chkIgnoredSegment.Checked = false;
                else
                    chkIgnoredSegment.Attributes.Add("checked", "checked");

                if (lastContentItem.AgendaSubItemID != null)
                {
                    // bind agenda sub items DDL
                    List<SessionAgendaItem> agendaSubItems = EditorFacade.GetAgendaSubItemsByAgendaID(lastContentItem.AgendaItemID);
                    ddlAgendaSubItems.DataSource = agendaSubItems;
                    ddlAgendaSubItems.DataValueField = "ID";
                    ddlAgendaSubItems.DataTextField = "Text";
                    ddlAgendaSubItems.DataBind();
                    ddlAgendaSubItems.SelectedValue = lastContentItem.AgendaSubItemID.ToString();
                }
                else
                    ddlAgendaSubItems.Enabled = false;
            }
            else
            {
               
                agendaItemTxt = sessionUnknownItem.Name;
                agendaItemIsIndexed = sessionUnknownItem.IsIndexed.ToString();
                agendaItemId.Value = sessionUnknownItem.ID.ToString();


                if (agendaItemTxt == "غير معرف")
                    divAgenda.Style.Add("display", "none");
                else
                    divAgenda.Style.Add("display", "");

                attachId.Value = "0";
                divAttach.Style.Add("display", "none");
                spanAttachTitle.InnerHtml = "";

                topicId.Value = "0";
                spanTopicTitle.InnerHtml = "";

                voteId.Value = "0";
                divVote.Style.Add("display", "none");
                spanVoteSubject.InnerHtml = "";

                chkTopic.Checked = false;
                aPopupGetAttTopic.Style.Add("display", "none");
            }


            // bind labels
            eparId.Value = current_session.EParliamentID.ToString();
            lblSessionName.Text = current_session.EParliamentID.ToString();
            lblSessionDate.Text = current_session.Date.ToShortDateString();
            lblMP3FileName.Text = System.IO.Path.GetFileName(file.Name);

            // save required info into the user session
            current_session_info["SessionID"] = sessionId;
            current_session_info["SessionFileID"] = session_file_id;
            current_session_info["FragOrderInXml"] = FragOrder;
            Session["current_session_info"] = current_session_info;

            currentOrder.Value = FragOrder.ToString();
            int numInserted = SessionContentItemHelper.GetItemsCountByFileID(session_file_id);

            if (currentPageMode == EditorPageMode.Edit)
            {
                if (FragOrder == 0)//FragOrder == 0 || 
                {
                    btnNext.Attributes.Remove("disabled");
                    btnPrev.Attributes.Add("disabled", "disabled");
                    sameAsPrevSpeaker.Attributes.Add("disabled", "disabled");

                }
                else
                    if (FragOrder > 0 && FragOrder <= p.Count - 1)
                    {
                        btnNext.Attributes.Remove("disabled");
                        btnPrev.Attributes.Remove("disabled");
                    }
                    else
                        if (FragOrder >= p.Count - 1)
                        {
                            btnNext.Attributes.Add("disabled", "disabled");
                            btnPrev.Attributes.Remove("disabled");

                        }


                if (FragOrder >= p.Count - 1)
                {
                    btnFinish.Attributes.Remove("disabled");
                    btnPreview.Attributes.Remove("disabled");
                    btnNext.Attributes.Add("disabled", "disabled");
                    btnFinish.Attributes.CssStyle.Add("display", "inline");
                    btnPreview.Attributes.CssStyle.Add("display", "inline");
                }
                //else
                //    if(FragOrder > 0 )
                //        btnPrev.Attributes.Remove("disabled");
                //    else

                //        if(((Model.SessionFileStatus)file.Status) == Model.SessionFileStatus.Completed)
            }
            else
            {
                /*btnNext.Attributes.CssStyle.Add("display", "none");
                btnPrev.Attributes.CssStyle.Add("display", "none");
                btnFinish.Attributes.CssStyle.Add("display", "none");
                btnSplit.Attributes.CssStyle.Add("display", "none");*/
                btnNext.Attributes.Remove("disabled");
                btnPrev.Attributes.Remove("disabled");
                btnFinish.Attributes.Remove("disabled");
                btnFinish.Attributes.CssStyle.Add("display", "inline");
                btnPreview.Attributes.Remove("disabled");
                btnPreview.Attributes.CssStyle.Add("display", "inline");
               
                if (FragOrder == 0)//FragOrder == 0 || 
                {
                    btnNext.Attributes.Remove("disabled");
                    btnPrev.Attributes.Add("disabled", "disabled");
                    sameAsPrevSpeaker.Attributes.Add("disabled", "disabled");

                }
                else
                    if (FragOrder > 0 && FragOrder <= p.Count - 1)
                    {
                        btnNext.Attributes.Remove("disabled");
                        btnPrev.Attributes.Remove("disabled");
                    }
                    else
                        if (FragOrder >= p.Count - 1)
                        {
                            btnNext.Attributes.Add("disabled", "disabled");
                            btnPrev.Attributes.Remove("disabled");

                        }


                if (FragOrder >= p.Count - 1)
                {
                    btnFinish.Attributes.Remove("disabled");
                    btnPreview.Attributes.Remove("disabled");
                    btnNext.Attributes.Add("disabled", "disabled");
                    btnFinish.Attributes.CssStyle.Add("display", "inline");
                    btnPreview.Attributes.CssStyle.Add("display", "inline");
                }

            }
            if (current_session != null && (current_session.SessionStatusID == (int)Model.SessionStatus.Approved ||
                current_session.SessionStatusID == (int)Model.SessionStatus.Completed))
            {
                btnFinish.Attributes.Remove("disabled");
                btnFinish.Attributes.CssStyle.Add("display", "inline");
                btnPreview.Attributes.Remove("disabled");
                btnPreview.Attributes.CssStyle.Add("display", "inline");
            }
            bool isReEdit=false;
            if (ReEdit != null && bool.TryParse(ReEdit, out isReEdit))
            {
                if (isReEdit )
                {
                    btnFinish.Attributes.Remove("disabled");
                    btnFinish.Attributes.CssStyle.Add("display", "inline");
                    btnPreview.Attributes.Remove("disabled");
                    btnPreview.Attributes.CssStyle.Add("display", "inline");
                }

            }
            if (numInserted >= p.Count - 1)
            {
                btnFinish.Attributes.Remove("disabled");
                btnFinish.Attributes.CssStyle.Add("display", "inline");
                btnPreview.Attributes.Remove("disabled");
                btnPreview.Attributes.CssStyle.Add("display", "inline");
            }

            /*if (currentPageMode != EditorPageMode.Edit)
                btnFinish.Attributes.CssStyle.Add("display", "none");*/

        }


        public string EditPageMode
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.EDIT_PAGE_MODE, Context);
            }
        }

        public string SessionContentItemID
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.SESSION_CONTENT_ITEM_ID, Context);
            }
        }
        public string ReEdit
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.REDIT, Context);
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

        }
}
}
