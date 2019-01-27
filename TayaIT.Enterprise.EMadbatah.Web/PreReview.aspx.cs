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
    public partial class PreReview : BasePage
    {
        public Hashtable tblStats = null;
        int nRejected = -1;
        int nModefiedAfterApprove = -1;
        int nFixed = -1;
        public int bChecked = 0;
        public SessionDetails sd;
        protected void Page_Load(object sender, EventArgs e)
        {
           
            if (!Page.IsPostBack)
            {
                //now the dataentry-reviewr role is using filereviewer privilages instead od session reviewer
                isCurrentUserFileRev.Value = (CurrentUser.Role == UserRole.FileReviewer || CurrentUser.Role == UserRole.ReviewrDataEntry || CurrentUser.Role == UserRole.Reviewer).ToString().ToLower();
                currentUserID.Value = CurrentUser.ID.ToString();
                long sessionFileID;
                StringBuilder sb = new StringBuilder();

                if (SessionFileID != null && long.TryParse(SessionFileID, out sessionFileID))
                {
                    SessionFile sf = SessionFileHelper.GetSessionFileByID(sessionFileID);
                    SessionIDHidden.Value = sf.SessionID.ToString();

                    if (CurrentUser.ID != sf.UserID)
                    {
                        Response.Redirect(Constants.PageNames.ERROR_PAGE + "?" + Constants.QSKeyNames.ERROR_TYPE + "=" + (int)ErrorType.Unauthorized);
                        return;
                    }

                    sessionSerial.InnerText = System.IO.Path.GetFileName(sf.Name);

                    List<long> WrittenAgendaItemWithGroupedSubItems = new List<long>();
                    List<string> writtenAgendaItems = new List<string>();

                    List<SessionContentItem> lsCntItems = SessionContentItemHelper.GetSessionContentItemsBySessionFileID(sessionFileID);
                    long currentSpeaker = 0;
                    long prevSpeaker = 0;
                    long topic_id = 0;
                    foreach (SessionContentItem item in lsCntItems)
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

                        //if (!item.MergedWithPrevious.Value)
                        currentSpeaker = item.AttendantID;

                      /*  if (currentSpeaker != prevSpeaker && topic_id != 0)
                        {
                            //for Topics
                            string reviewItemTopic = write_topic_att(topic_id, item);
                            if (reviewItemTopic != "")
                                sb.Append(reviewItemTopic);
                            topic_id = 0;
                        }*/
                        
                        if ((item.TopicID != null && item.TopicID != 0 && !(bool)item.MergedTopicWithPrevious))
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
                                            attFullPresentationName = "السيد " + att.Name.Trim();
                                        else attFullPresentationName = att.AttendantTitle.Trim() + " " + att.Name.Trim();
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
                                                 .Replace("<%speakerJob2%>", job2);
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
                        }

                        string reviewItem = Application[Constants.HTMLTemplateFileNames.ReviewItem].ToString()
                                                .Replace("<%SessionContentItemID%>", item.ID.ToString())
                                                .Replace("<%itemText%>", item.Text);

                        reviewItem = reviewItem.Replace("<%isLocked%>", "openeditem")
                                .Replace("<%title%>", "");



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


                        sb.Append(reviewItem);

                      /*  if (item.TopicID != null && item.TopicID != 0)
                        {
                            topic_id = (long)item.TopicID;
                        }*/

                        if (item.ID == lsCntItems[lsCntItems.Count - 1].ID && topic_id != 0)
                        {
                            //for Topics
                            string reviewItemTopic = write_topic_att(topic_id, item);
                            if (reviewItemTopic != "")
                                sb.Append(reviewItemTopic);
                            topic_id = 0;
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
                                    .Replace("<%UserName%>", item.User.FName);
                                  //  .Replace("<%RevName%>", sd.ReviewerName + "\r\n<br/>(للتعديل يمكنك استخدام خيارات تعديل أكثر للمقطع السابق) هذا المقطع تذييل صفحة");
                            sb.Append(reviewFootNote);
                        }
                    }
                    madbatahContents.InnerHtml += sb.ToString();
                }
                else
                {
                    ShowMainError(GetLocalizedString("strNoQueryStr"));
                }
            }
        }

        public string write_topic_att(long topic_id, SessionContentItem item)
        {
   
            return "";
        }
    }
}