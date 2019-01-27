using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.Script.Serialization;
using TayaIT.Enterprise.EMadbatah.Model;
using TayaIT.Enterprise.EMadbatah.BLL;
using TayaIT.Enterprise.EMadbatah.Config;
using TayaIT.Enterprise.EMadbatah.Util.Web;
using System.Collections;
using TayaIT.Enterprise.EMadbatah.DAL;
using TayaIT.Enterprise.EMadbatah.Model.VecSys;
using TayaIT.Enterprise.EMadbatah.Vecsys;
using System.Xml;
using TayaIT.Enterprise.EMadbatah.Util;

namespace TayaIT.Enterprise.EMadbatah.Web.Framework
{
    public class EditSessionHandler : BaseHandler
    {
        public string AgendaID
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.AGENDA_ID, _context);
            }
        }
        public string AttachID
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.ATTACHMENT_ID, _context);
            }
        }
        public string TopicID
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.TOPIC_ID, _context);
            }
        }
        public string VoteID
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.Vote_ID, _context);
            }
        }
        public string AgendaItemText
        {
            get
            {
                return HttpUtility.UrlDecode(WebHelper.GetQSValue(Constants.QSKeyNames.AGENDA_ITEM_TEXT, _context));
            }
        }
        // variables to load stored session values in
        int FragOrder ;
        //bool firstFrag = false;
        //bool lastFrag = false;
        double stime, soriginaltime, etime, duration;
        long Session_ID ,Session_File_ID;
        Hashtable current_session_info;
        protected override void HandleRequest()
        {
            WebFunctions.EditWizard function;
            if (AjaxFunctionName != null && Enum.TryParse<WebFunctions.EditWizard>(AjaxFunctionName, true, out function))
            {
                string jsonStringOut = null;
                long agendItemID;
                long attendantID;
                switch (function)
                {
                    case WebFunctions.EditWizard.GetSpeakerJobTitle:
                        {
                            if (AttendantID != null && long.TryParse(AttendantID, out attendantID))
                            {
                                Attendant att = AttendantHelper.GetAttendantById(attendantID);
                                if(att != null)
                                    jsonStringOut = SerializeObjectInJSON(att.JobTitle);
                                else
                                    jsonStringOut = SerializeObjectInJSON("error");
                                //List<SessionAgendaItem> items = EditorFacade.GetAgendaSubItemsByAgendaID(agendItemID);
                                //jsonStringOut = SerializeObjectInJSON(items);
                                break;
                            }
                            else
                                break;
                        }
                    case WebFunctions.EditWizard.GetSpeakerJobTitleAndAvatar:
                        {
                            if (AttendantID != null && long.TryParse(AttendantID, out attendantID))
                            {
                                Attendant att = AttendantHelper.GetAttendantById(attendantID);
                                if (att != null)
                                    jsonStringOut = SerializeObjectInJSON(att.JobTitle + "," + att.AttendantAvatar);
                                else
                                    jsonStringOut = SerializeObjectInJSON("error");
                                //List<SessionAgendaItem> items = EditorFacade.GetAgendaSubItemsByAgendaID(agendItemID);
                                //jsonStringOut = SerializeObjectInJSON(items);
                                break;
                            }
                            else
                                break;
                        }
                    case WebFunctions.EditWizard.GetAgendaSubItems:
                        {
                            if (AgendaID != null && long.TryParse(AgendaID, out agendItemID))
                            {
                                List<SessionAgendaItem> items = EditorFacade.GetAgendaSubItemsByAgendaID(agendItemID);
                                jsonStringOut = SerializeObjectInJSON(items);
                                break;
                            }
                            else
                                break;
                        }
                    case WebFunctions.EditWizard.AddAgendaItem:
                        {
                            if (AgendaItemText != null)
                            {
                                // collect sessionContentItem attributes/values
                                //loadSessionValues();
                                int agendaItemIsIndexed = int.Parse(WebHelper.GetQSValue(Constants.QSKeyNames.AGENDA_IS_INDEXED, _context));
                                string agendaitemID = WebHelper.GetQSValue(Constants.QSKeyNames.AGENDA_ID, _context);
                                if (string.IsNullOrEmpty(agendaitemID))
                                {
                                    long id = EditorFacade.AddNewAgendaItem(HttpUtility.HtmlDecode(AgendaItemText), agendaItemIsIndexed, long.Parse(SessionID));
                                    jsonStringOut = SerializeObjectInJSON(id);
                                }
                                else
                                {
                                    EditorFacade.UpdateAgendaItem(int.Parse(agendaitemID), HttpUtility.HtmlDecode(AgendaItemText), agendaItemIsIndexed);
                                    jsonStringOut = SerializeObjectInJSON(AgendaID);
                                }
                                break;
                            }
                            else
                                break;
                        }
                    case WebFunctions.EditWizard.UpdateAgendaItem:
                        {
                            if (AgendaItemText != null && long.TryParse(AgendaID, out agendItemID))
                            {
                                string res = EditorFacade.UpdateAgendaItem(agendItemID, AgendaItemText);
                                jsonStringOut = SerializeObjectInJSON(res);
                                break;
                            }
                            else
                                break;
                        }
                    case WebFunctions.EditWizard.AssignAttachToSessionContentItem:
                        {
                            if (SessionContentItemID != null && AttachmentID !=null)
                            {
                                int res = EditorFacade.AssignAttachmentToSessionContentItem(long.Parse(AttachmentID), long.Parse(SessionContentItemID));
                                jsonStringOut = SerializeObjectInJSON(res.ToString());
                                break;
                            }
                            else
                                break;
                        }
                    case WebFunctions.EditWizard.GetTopicID:
                        {
                            jsonStringOut = SerializeObjectInJSON(getSegmentTopicID());
                            break;
                        }
                    case WebFunctions.EditWizard.DoNext:
                        {
                            Session currentSession = null;
                                Hashtable result = SaveAndExit(out currentSession);; 
                                if(result["Message"] == "fail")
                                {
                                    jsonStringOut = SerializeObjectInJSON(result);
                                    break;
                                }
                                else{
                                    try
                                    {
                                        SessionAudioFile file = SessionFileFacade.GetSessionFilesByID(Session_File_ID);
                                        int NextItemOrderID = FragOrder + 1;
                                        SessionContentItem nextContentItem = SessionContentItemFacade.GetSessionContentItemByIdAndFragmentOrder(Session_File_ID, NextItemOrderID);
                                        if (nextContentItem == null)
                                        {
                                            nextContentItem = GetContentFromXML(file.Name.ToLower().Replace(".mp3", ".trans.xml"), NextItemOrderID);
                                            current_session_info["IsGroupSubAgendaItems"] = bool.Parse(WebHelper.GetQSValue(Constants.QSKeyNames.IsGroupSubAgendaItems, _context));
                                            result["MergeWithPrevTopic"] = false;
                                        }
                                        else
                                        {
                                            current_session_info["PargraphStartTime"] = nextContentItem.StartTime;
                                            current_session_info["PargraphEndTime"] = nextContentItem.EndTime;
                                            current_session_info["SameAsPrevSpeaker"] = nextContentItem.MergedWithPrevious;
                                            current_session_info["IsGroupSubAgendaItems"] = nextContentItem.AgendaItem.IsGroupSubAgendaItems;
                                            current_session_info["Ignored"] = nextContentItem.Ignored;
                                            current_session_info["IsSessionPresident"] = nextContentItem.IsSessionPresident.ToString();
                                            result["MergeWithPrevTopic"] = nextContentItem.MergedTopicWithPrevious;
                                        }
                                        result["PargraphStartTime"] = current_session_info["PargraphStartTime"];
                                        result["PargraphEndTime"] = current_session_info["PargraphEndTime"];
                                        result["SameAsPrevSpeaker"] = current_session_info["SameAsPrevSpeaker"];
                                        result["IsSessionPresident"] = current_session_info["IsSessionPresident"];
                                        result["IsGroupSubAgendaItems"] = current_session_info["IsGroupSubAgendaItems"];
                                        result["Ignored"] = current_session_info["Ignored"];

                                        // compose response[json object] 
                                        SessionContentItem retItem = getReturnedContentItemObject(nextContentItem);
                                        result["Item"] = retItem;
                                        result["ItemFragOrder"] = NextItemOrderID.ToString();
                                        AgendaItem selectedAgendaItem = AgendaHelper.GetAgendaItemById(retItem.AgendaItemID);
                                        if (selectedAgendaItem == null)
                                            selectedAgendaItem = AgendaHelper.GetAgendaItemByNameAndSessionID("غير معرف", currentSession.ID);
                                        result["AgendaItemText"] = selectedAgendaItem != null ? selectedAgendaItem.Name : "";
                                        result["AgendaItemIsIndexed"] = selectedAgendaItem != null ? selectedAgendaItem.IsIndexed.ToString() : "0";
                                        result["AgendaItemID"] = selectedAgendaItem != null ? selectedAgendaItem.ID.ToString() : "";

                                        //For Attachment
                                        if (retItem.AttachementID != null && retItem.AttachementID != 0)
                                        {
                                            Attachement attach = AttachmentHelper.GetAttachementByID((int)retItem.AttachementID);
                                            if (attach != null)
                                            {
                                                result["AttachText"] = attach.Name;
                                                result["AttachID"] = attach.ID.ToString();
                                            }
                                            else
                                            {
                                                result["AttachText"] = "";
                                                result["AttachID"] = "0";
                                            }
                                        }
                                        else
                                        {
                                            result["AttachText"] = "";
                                            result["AttachID"] = "0";
                                        }

                                    //For Voting
                                    /*  if (retItem.VotingID != null && retItem.VotingID != 0)
                                      {
                                         TBLNonSecretVoteSubject vote = NonSecretVoteSubjectHelper.GetSessionVoteByVoteID((int)retItem.VotingID);
                                          if (vote != null)
                                          {
                                              result["VoteSubject"] = vote.NonSecretVoteSubject;
                                              result["VoteID"] = vote.NonSecretVoteSubjectID.ToString();
                                          }
                                          else
                                          {
                                              result["VoteSubject"] = "";
                                              result["VoteID"] = "0";
                                          }
                                      }
                                      else
                                      {
                                          result["VoteSubject"] = "";
                                          result["VoteID"] = "0";
                                      }*/
                                    /* //For Topics
                                     if (retItem.TopicID != null && retItem.TopicID != 0)
                                     {
                                         long prevTpcID = getPrevSegmentTopicID(FragOrder);
                                         //Topic tpc = TopicHelper.GetTopicByID((int)retItem.TopicID);
                                         if (prevTpcID == 0 || prevTpcID == retItem.TopicID)//if (tpc != null)
                                         {
                                             result["TopicTitle"] = "";//tpc.Title;
                                             result["TopicID"] = retItem.TopicID;// tpc.ID.ToString();
                                         }
                                         else
                                         {
                                             result["TopicTitle"] = "";
                                             result["TopicID"] = "0";
                                         }
                                     }
                                     else
                                     {
                                         result["TopicTitle"] = "";
                                         result["TopicID"] = "0";
                                     }
                                     */
                                    result["VoteSubject"] = "";
                                    result["VoteID"] = "0";
                                    //For Topics
                                    if (retItem.TopicID != null && retItem.TopicID != 0)
                                        {
                                            result["TopicTitle"] = "";//tpc.Title;
                                            result["TopicID"] = retItem.TopicID;// tpc.ID.ToString();
                                        }
                                        else
                                        {
                                            result["TopicTitle"] = "";
                                            result["TopicID"] = "0";
                                        }

                                        long prevTpcID = getPrevSegmentTopicID(FragOrder);
                                        result["PrevTopicID"] = prevTpcID.ToString();
                                        //userAvatar
                                        Attendant attendObj = AttendantHelper.GetAttendantById(retItem.AttendantID);
                                        if (attendObj != null && attendObj.AttendantAvatar != null && attendObj.AttendantAvatar != "")
                                        {
                                            result["AttendantAvatar"] = "/images/AttendantAvatars/" + attendObj.AttendantAvatar + "?t=" + DateTime.Now.ToString();
                                        }
                                        else
                                        {
                                            result["AttendantAvatar"] = "/images/AttendantAvatars/unknown.jpg?t=" + DateTime.Now.ToString();
                                        }

                                        if (attendObj != null && attendObj.JobTitle != null )
                                        {
                                            result["AttendantJobTitle"] = attendObj.JobTitle;
                                        }
                                        else
                                        {
                                            result["AttendantJobTitle"] = "";
                                        }

                                        List<AgendaItem> AgendaItems = currentSession.AgendaItems.ToList<AgendaItem>();
                                        List<SessionAgendaItem> retAgendaItems = new List<SessionAgendaItem>();
                                        foreach (AgendaItem agendaItem in AgendaItems)
                                        {
                                            SessionAgendaItem newSessionAgendaItem = new SessionAgendaItem(agendaItem.ID, agendaItem.Name);
                                            newSessionAgendaItem.IsGroupSubAgendaItems = agendaItem.IsGroupSubAgendaItems;
                                            retAgendaItems.Add(newSessionAgendaItem);
                                        }

                                        result["AgendaItems"] = retAgendaItems;
                                        List<SessionAgendaItem> retAgendaSubItems = null;
                                        if (retItem.AgendaItemID != 0)
                                            retAgendaSubItems = EditorFacade.GetAgendaSubItemsByAgendaID(retItem.AgendaItemID);
                                        result["AgendaSubItems"] = retAgendaSubItems;
                                        result["ItemOrder"] = checkItemPosition(file.Name.ToLower().Replace(".mp3", ".trans.xml"), NextItemOrderID);
                                        // save last FragOrder in user session
                                        current_session_info["FragOrderInXml"] = NextItemOrderID;
                                        if (FragOrder == 0)
                                            SessionFileHelper.UpdateSessionFileStatus(Session_File_ID, (int)Model.SessionFileStatus.InProgress,null);
                                    }
                                    catch (Exception ex)
                                    {
                                        result["Message"] = "fail";
                                       
                                    }
                                }
                                
                                jsonStringOut = SerializeObjectInJSON(result);
                                break;
                            
                        }
                    case WebFunctions.EditWizard.DoPrevious:
                        {
                            // collect sessionContentItem attributes/values
                            Session currentSession = null;
                            Hashtable result = SaveAndExit(out currentSession); ;
                              if (result["Message"] == "fail")
                              {
                                  jsonStringOut = SerializeObjectInJSON(result);
                                  break;
                              }
                              else
                              {

                                  //loadSessionValues();
                                  //Hashtable result = new Hashtable();
                                  int prevItemOrderID = FragOrder - 1;
                                  // check current user privellge
                                  SessionAudioFile file = SessionFileFacade.GetSessionFilesByID(Session_File_ID);
                                  if (CurrentUser.ID == file.UserID || (CurrentUser.ID != file.UserID && CurrentUser.Role != UserRole.DataEntry))//CurrentUser.ID == file.FileReviewrID)
                                  {
                                      result["Message"] = "success";
                                      // load item from DB or xml
                                      SessionContentItem prevContentItem = SessionContentItemFacade.GetSessionContentItemByIdAndFragmentOrder(Session_File_ID, prevItemOrderID);
                                      if (prevContentItem == null)
                                      {
                                          prevContentItem = GetContentFromXML(file.Name.ToLower().Replace(".mp3", ".trans.xml"), prevItemOrderID);
                                          current_session_info["IsGroupSubAgendaItems"] = bool.Parse(WebHelper.GetQSValue(Constants.QSKeyNames.IsGroupSubAgendaItems, _context));
                                          result["MergeWithPrevTopic"] = false;
                                      }
                                      else
                                      {
                                          current_session_info["PargraphStartTime"] = prevContentItem.StartTime;
                                          current_session_info["PargraphEndTime"] = prevContentItem.EndTime;
                                          current_session_info["SameAsPrevSpeaker"] = prevContentItem.MergedWithPrevious;
                                          current_session_info["IsSessionPresident"] = prevContentItem.IsSessionPresident.ToString();
                                          current_session_info["IsGroupSubAgendaItems"] = prevContentItem.AgendaItem.IsGroupSubAgendaItems;
                                          current_session_info["Ignored"] = prevContentItem.Ignored;
                                          result["MergeWithPrevTopic"] = prevContentItem.MergedTopicWithPrevious;
                                      }
                                      //get prev item from db if exist, meshmesh
                                      if (prevItemOrderID - 1 >= 0)
                                      {
                                          SessionContentItem prevPrevContentItem = SessionContentItemHelper.GetPrevSessionContentItem(Session_File_ID, prevItemOrderID);
                                          if (prevPrevContentItem != null)
                                          {
                                              result["prevAgendaItemID"] = prevPrevContentItem.AgendaItemID;
                                              result["prevAttendantID"] = prevPrevContentItem.AttendantID;
                                              if (prevPrevContentItem.AgendaSubItemID != null)
                                                  result["prevAgendaSubItemID"] = prevPrevContentItem.AgendaSubItemID;
                                              if (prevPrevContentItem.Attendant.JobTitle != null && !string.IsNullOrEmpty(prevPrevContentItem.Attendant.JobTitle))
                                                  result["prevAttendantJobTitle"] = prevPrevContentItem.Attendant.JobTitle;

                                          }
                                      }

                                      result["PargraphStartTime"] = current_session_info["PargraphStartTime"];
                                      result["PargraphEndTime"] = current_session_info["PargraphEndTime"];
                                      result["SameAsPrevSpeaker"] = current_session_info["SameAsPrevSpeaker"];
                                      result["IsSessionPresident"] = current_session_info["IsSessionPresident"];
                                      result["IsGroupSubAgendaItems"] = current_session_info["IsGroupSubAgendaItems"];
                                      result["Ignored"] = current_session_info["Ignored"];

                                      SessionContentItem retItem = getReturnedContentItemObject(prevContentItem);
                                      result["Item"] = retItem;
                                      AgendaItem selectedAgendaItem = AgendaHelper.GetAgendaItemById(retItem.AgendaItemID);
                                      if (selectedAgendaItem == null)
                                          selectedAgendaItem = AgendaHelper.GetAgendaItemByNameAndSessionID("غير معرف", currentSession.ID);
                                      result["AgendaItemText"] = selectedAgendaItem != null ? selectedAgendaItem.Name : "";
                                      result["AgendaItemIsIndexed"] = selectedAgendaItem != null ? selectedAgendaItem.IsIndexed.ToString() : "0";
                                      result["AgendaItemID"] = selectedAgendaItem != null ? selectedAgendaItem.ID.ToString() : "";

                                      if (retItem.AttachementID != null && retItem.AttachementID != 0)
                                      {
                                          Attachement attach = AttachmentHelper.GetAttachementByID((int)retItem.AttachementID);
                                          if (attach != null)
                                          {
                                              result["AttachText"] = attach.Name;
                                              result["AttachID"] = attach.ID.ToString();
                                          }
                                          else
                                          {
                                              result["AttachText"] = "";
                                              result["AttachID"] = "0";
                                          }
                                      }
                                      else
                                      {
                                          result["AttachText"] = "";
                                          result["AttachID"] = "0";
                                      }

                                      //For Voting
                                   /*   if (retItem.VotingID != null && retItem.VotingID != 0)
                                      {
                                          TBLNonSecretVoteSubject vote = NonSecretVoteSubjectHelper.GetSessionVoteByVoteID((int)retItem.VotingID);
                                          if (vote != null)
                                          {
                                              result["VoteSubject"] = vote.NonSecretVoteSubject;
                                              result["VoteID"] = vote.NonSecretVoteSubjectID.ToString();
                                          }
                                          else
                                          {
                                              result["VoteSubject"] = "";
                                              result["VoteID"] = "0";
                                          }
                                      }
                                      else
                                      {
                                          result["VoteSubject"] = "";
                                          result["VoteID"] = "0";
                                      }*/

                                      //For Topics
                                      if (retItem.TopicID != null && retItem.TopicID != 0)
                                      {
                                          /*Topic tpc = TopicHelper.GetTopicByID((int)retItem.TopicID);
                                          if (tpc != null)
                                          {
                                              result["TopicTitle"] = tpc.Title;
                                              result["TopicID"] = tpc.ID.ToString();
                                          }
                                          else
                                          {
                                              result["TopicTitle"] = "";
                                              result["TopicID"] = "0";
                                          }*/
                                          result["TopicTitle"] = "";
                                          result["TopicID"] = retItem.TopicID;
                                      }
                                      else
                                      {
                                          result["TopicTitle"] = "";
                                          result["TopicID"] = "0";
                                      }
                                      long prevTpcID = getPrevSegmentTopicID(prevItemOrderID - 1);
                                      result["PrevTopicID"] = prevTpcID.ToString();
                                      //userAvatar
                                      Attendant attendObj = AttendantHelper.GetAttendantById(retItem.AttendantID);
                                      if (attendObj != null && attendObj.AttendantAvatar != null && attendObj.AttendantAvatar != "")
                                      {
                                          result["AttendantAvatar"] = "/images/AttendantAvatars/" + attendObj.AttendantAvatar;
                                      }
                                      else
                                      {
                                          result["AttendantAvatar"] = "/images/unknown.jpg?t=" + DateTime.Now.ToString();
                                      }

                                      if (attendObj != null && attendObj.JobTitle != null)
                                      {
                                          result["AttendantJobTitle"] = attendObj.JobTitle;
                                      }
                                      else
                                      {
                                          result["AttendantJobTitle"] = "";
                                      }
                                      result["ItemFragOrder"] = prevItemOrderID.ToString();
                                      List<AgendaItem> AgendaItems = currentSession.AgendaItems.ToList<AgendaItem>();
                                      List<SessionAgendaItem> retAgendaItems = new List<SessionAgendaItem>();
                                      foreach (AgendaItem agendaItem in AgendaItems)
                                      {
                                          SessionAgendaItem newSessionAgendaItem = new SessionAgendaItem(agendaItem.ID, agendaItem.Name);
                                          retAgendaItems.Add(newSessionAgendaItem);
                                      }
                                      result["AgendaItems"] = retAgendaItems;
                                      List<SessionAgendaItem> retAgendaSubItems = null;
                                      if (retItem.AgendaItemID != 0)
                                          retAgendaSubItems = EditorFacade.GetAgendaSubItemsByAgendaID(retItem.AgendaItemID);
                                      result["AgendaSubItems"] = retAgendaSubItems;
                                      result["ItemOrder"] = checkItemPosition(file.Name.ToLower().Replace(".mp3", ".trans.xml"), prevItemOrderID);
                                      // save last FragOrder in user session
                                      current_session_info["FragOrderInXml"] = FragOrder - 1;
                                  }
                                  else
                                  {
                                      result["Message"] = "fail";
                                  }
                              }
                            jsonStringOut = SerializeObjectInJSON(result);
                            break;
                        }
                    case WebFunctions.EditWizard.BackToOriginalContent:
                        {                            
                            Hashtable result = new Hashtable();
                            // collect sessionContentItem attributes/values


                            long sfid = -1;
                            long scid = -1;
                            if (SessionContentItemID != null && long.TryParse(SessionContentItemID, out scid) &&
                                SessionFileID != null && long.TryParse(SessionFileID, out sfid))
                            {
                                SessionContentItem sci = SessionContentItemHelper.GetSessionContentItemById(scid);
                                if (sci != null)
                                {
                                    FragOrder = sci.FragOrderInXml;
                                    Session_File_ID = sfid;
                                }

                                //FragOrder = 
                            }
                            else
                            {
                                loadSessionValues();
                            }


                            
                            SessionAudioFile file = SessionFileFacade.GetSessionFilesByID(Session_File_ID);
                            SessionContentItem ContentItem = null;
                            // check current user privellge
                            //if (CurrentUser.ID == file.UserID || CurrentUser.Role == UserRole.Admin)
                            //{
                                result["Message"] = "success";
                                ContentItem = GetContentFromXML(file.Name.ToLower().Replace(".mp3", ".trans.xml"), FragOrder);
                                result["Item"] = ContentItem;
                                result["ItemOrder"] = checkItemPosition(file.Name.ToLower().Replace(".mp3", ".trans.xml"), FragOrder);
                                // save last FragOrder in user session
                                //current_session_info["FragOrderInXml"] = FragOrder;
                            //}
                            //else
                            //{
                            //    result["Message"] = "fail";
                            //}
                            jsonStringOut = SerializeObjectInJSON(result);
                            break;
                        }
                    case WebFunctions.EditWizard.SaveAndExit:
                        {
                            Session currentSession = null;
                            jsonStringOut = SerializeObjectInJSON(SaveAndExit(out currentSession));
                            break;
                        }
                    case WebFunctions.EditWizard.SaveOnly:
                        {
                             Session currentSession = null;
                            Hashtable result = SaveAndExit(out currentSession); ;
                            if (result["Message"] == "fail")
                            {
                                jsonStringOut = SerializeObjectInJSON(result);
                                break;
                            }
                            else
                            {
                                int currentItemOrderID = FragOrder;
                                // check current user privellge
                                SessionAudioFile file = SessionFileFacade.GetSessionFilesByID(Session_File_ID);
                                if (CurrentUser.ID == file.UserID || (CurrentUser.ID != file.UserID && CurrentUser.Role != UserRole.DataEntry))//CurrentUser.ID == file.FileReviewrID)
                                {
                                    result["Message"] = "success";
                                    // load item from DB
                                    SessionContentItem currentContentItem = SessionContentItemFacade.GetSessionContentItemByIdAndFragmentOrder(Session_File_ID, currentItemOrderID);
                                    if (currentContentItem == null)
                                    {
                                        result = new Hashtable();
                                        result["Message"] = "fail";

                                    }
                                    else
                                    {
                                        SessionContentItem retItem = getReturnedContentItemObject(currentContentItem);
                                        result["Item"] = retItem;
                                    }
                                    jsonStringOut = SerializeObjectInJSON(result);
                                    break;
                                }
                            }
                            jsonStringOut = SerializeObjectInJSON(jsonStringOut);
                            break;
                        }
                    case WebFunctions.EditWizard.UpdateSessionFileStatusCompleted:
                        {
                            long sessionID;
                            Session currentSession = null;
                            jsonStringOut = SerializeObjectInJSON(SaveAndExit(out currentSession));
                            SessionFileHelper.UpdateSessionFileStatus(Session_File_ID, (int)Model.SessionFileStatus.Completed,null);
                           
                            if(SessionID != null && long.TryParse(SessionID,out sessionID))
                            {
                                SessionDetails sd = EMadbatahFacade.GetSessionDetailsBySessionID(sessionID);
                                var notCompletedSessionFiles = from sf in sd.SessionFiles
                                                        where ((sf.ID != Session_File_ID) && 
                                                               (sf.Status != Model.SessionFileStatus.Completed))                                                        
                                                        select sf;

                                if (notCompletedSessionFiles.ToList<SessionAudioFile>().Count == 0)
                                {
                                    EMadbatahFacade.UpdateSessionStatus(sessionID, Model.SessionStatus.Completed);
                                }

                               
                                //sd.SessionFiles
                            }
                            break;
                        }
                    case WebFunctions.EditWizard.UpdateCompletedSessionFileStatusCompleted:
                        {
                            long sessionID, sessionFileID;
                          
                            if (SessionID != null && long.TryParse(SessionID, out sessionID) && SessionFileID != null && long.TryParse(SessionFileID, out sessionFileID))
                            {
                                SessionFileHelper.UpdateSessionFileStatus(sessionFileID, (int)Model.SessionFileStatus.Completed, null);
                                SessionDetails sd = EMadbatahFacade.GetSessionDetailsBySessionID(sessionID);
                                var notCompletedSessionFiles = from sf in sd.SessionFiles
                                                               where ((sf.ID != sessionFileID) &&
                                                                      (sf.Status != Model.SessionFileStatus.Completed))
                                                               select sf;

                                if (notCompletedSessionFiles.ToList<SessionAudioFile>().Count == 0)
                                {
                                    EMadbatahFacade.UpdateSessionStatus(sessionID, Model.SessionStatus.Completed);
                                }
                            }
                            jsonStringOut = SerializeObjectInJSON("true");//SerializeObjectInJSON(SaveAndExit(out currentSession));
                            break;
                        }
                    case WebFunctions.EditWizard.SplitItem:
                        {
                            long currentFragOrder = -1;

                            if (CurrentFragOrder != null && long.TryParse(CurrentFragOrder, out currentFragOrder) &&
                                XmlFilePath != null && SplittedText != null &&
                                SessionFileID != null && long.TryParse(SessionFileID, out Session_File_ID))
                            {
                                try
                                {
                                    XmlDocument doc = new XmlDocument();
                                    doc.Load(XmlFilePath);

                                    StringBuilder sb = new StringBuilder();
                                    XmlDocument spansDoc = new XmlDocument();//nbsp
                                    spansDoc.LoadXml("<doc>" + HttpUtility.HtmlDecode(SplittedText).Replace("&nbsp;", "!####!").Replace("<br/>", "#!#!#!").Replace("<br>", "#!#!#!").Replace("<br >", "#!#!#!").Replace("<br />", "#!#!#!") + "</doc>");

                                    //XmlNodeList spanNodes = spansDoc.SelectNodes("//doc/span"); //modefied by usama and saber for text when splitted all spaces are removed
                                    XmlNodeList spanNodes = spansDoc.SelectNodes("//span[@data-stime]|//p");

                                    if (spanNodes.Count == 0)
                                    {
                                        jsonStringOut = SerializeObjectInJSON("false");
                                        break;
                                    }
                                    string stime = "";
                                    string etime = "";
                                    int loop = 0;
                                    foreach (XmlNode node in spanNodes)
                                    {
                                        //span means the words translated from vocapia
                                        if (node.Name == "span")
                                        {
                                            if (node.Attributes["data-stime"] == null)
                                                continue;
                                            if (loop == 0)
                                            {
                                                stime = node.Attributes["data-stime"].Value;
                                                loop++;
                                            }
                                            sb.Append("<Word stime=\"" + node.Attributes["data-stime"].Value + "\" dur=\"0.25\" conf=\"0.886\">" + node.InnerText + "</Word>");
                                            etime = node.Attributes["data-stime"].Value;
                                        }

                                        else if (node.Name == "p")//الاجراءات التى اضافها المستخدم عند  القطع يجب اضافتها مع النص التالى 
                                        {
                                            string procedure_id = "0";
                                            string text_align = "right";
                                            if (node.Attributes["procedure-id"] != null)
                                                procedure_id = node.Attributes["procedure-id"].Value;
                                            if (node.Attributes["style"] != null)
                                                text_align = node.Attributes["style"].Value.Replace("text-align", "").Replace(":", "").Replace(";", "").Trim();
                                            sb.Append("<Word procedure-id=\"" + procedure_id + "\" text-align=\"" + text_align + "\">" + node.InnerText + "</Word>");
                                        }

                                        else//next line 
                                        {
                                            sb.Append("<Word procedure-id='-1' text-align='space'>space</Word>");
                                        }
                                        if (node.NextSibling != null)
                                        {
                                            if (node.NextSibling.InnerText.Contains("#!#!#!"))
                                            {
                                                string[] sep = new string[1] { "#!#!#!" };
                                                string[] newlines = node.NextSibling.InnerText.Split(sep, StringSplitOptions.None);
                                                for (int g = 0; g < newlines.Count()-1; g++)
                                                {
                                                    sb.Append("<Word procedure-id='-1' text-align='space'>space</Word>");
                                                }
                                            }
                                        }
                                    }

                                    if (stime == "")
                                    {
                                        SessionContentItem scitemObj = SessionContentItemHelper.GetSessionContentItemByFragOrder(Session_File_ID, currentFragOrder);
                                        if (scitemObj != null)
                                        {
                                            stime = scitemObj.EndTime.ToString();
                                            etime = scitemObj.EndTime.ToString();
                                        }
                                    }

                                    etime = etime == "" ? stime : etime;
                                    //<Word stime="4.89" dur="0.25" conf="0.886"> لأ </Word>
                                    sb.Append("</SpeechSegment>");
                                    //XmlNode refNode = doc.SelectSingleNode("/AudioDoc/SegmentList/SpeechSegment[2]");

                                    // get 1st random string 
                                    string Rand1 = TextHelper.RandomString(4);
                                    string Rand2 = TextHelper.RandomString(4);
                                    string speakerID = "TT_" + Rand1 + "-" + Rand2;


                                    string header = "<SpeechSegment ch=\"1\" sconf=\"1.00\" stime=\"" + stime + "\" etime=\"" + etime + "\" spkid=\"" + speakerID + "\" lang=\"ara-fnc\" lconf=\"1.00\" trs=\"1\">";
                                    XmlDocumentFragment xmlDocFrag = doc.CreateDocumentFragment();
                                    xmlDocFrag.InnerXml = header + sb.ToString();


                                    //as currentfragorder is out of combine so we have to calculate its relative pos in xml without combine
                                    TransFile tf = new TransFile();
                                    tf = VecsysParser.ParseTransXml(XmlFilePath);
                                    List<Paragraph> p = VecsysParser.combineSegments(tf.SpeechSegmentList);
                                    int curPos = 0;
                                    for (int i = 0; i <= currentFragOrder; i++)
                                        curPos += p[i].speechSegmentsList.Count;
                                    if (curPos == 0)
                                        curPos = 1;//for below curPos - 1
                                    //doc.DocumentElement.ChildNodes[3].ChildNodes.Item(2).AppendChild(xmlDocFrag);
                                    doc.DocumentElement.ChildNodes[3].InsertAfter(xmlDocFrag, doc.DocumentElement.ChildNodes[3].ChildNodes.Item(curPos - 1));

                                    //doc.InsertAfter(xmlDocFrag, doc.DocumentElement.ChildNodes[3].ChildNodes[2]);

                                    doc.Save(XmlFilePath);

                                    SessionContentItemHelper.UpdateSessionContentItemOrders(Session_File_ID, currentFragOrder);
                                    SessionContentItemHelper.UpdateSessionContentItemEndTime(Session_File_ID, currentFragOrder, double.Parse(stime));

                                    current_session_info = (Hashtable)_context.Session["current_session_info"];
                                    current_session_info["PargraphEndTime"] = double.Parse(stime);

                                    jsonStringOut = SerializeObjectInJSON("true");
                                }
                                catch (Exception ex)
                                {
                                    jsonStringOut = SerializeObjectInJSON("false");
                                }
                            }
                            break;
                        }
                    default:
                        { break; }
                }
                if (jsonStringOut != null)
                {
                    _context.Response.AddHeader("Encoding", "UTF-8");
                    _context.Response.Write(jsonStringOut);
                }
            }
            else
                return;
        }
        public Hashtable SaveAndExit(out Session session)
        {
            // collect sessionContentItem attributes/values

            //edit mode
            //editmode

            

            loadSessionValues();
            long? AgendaSubItemID = null;
            if (WebHelper.GetQSValue(Constants.QSKeyNames.AgendaSubItemID, _context) != null)
                AgendaSubItemID = long.Parse(WebHelper.GetQSValue(Constants.QSKeyNames.AgendaSubItemID, _context));
            int AgendaItemID;
            long SpeakerID;
            bool SameAsPrevSpeaker = bool.Parse(WebHelper.GetQSValue(Constants.QSKeyNames.SameAsPrevSpeaker, _context));
            bool MergeWithPrevTopic = bool.Parse(WebHelper.GetQSValue(Constants.QSKeyNames.MergeWithPrevTopic, _context));
            int IsSessionPresident = int.Parse(WebHelper.GetQSValue(Constants.QSKeyNames.IsSessionPresident, _context));
            bool IsGroupSubAgendaItems = bool.Parse(WebHelper.GetQSValue(Constants.QSKeyNames.IsGroupSubAgendaItems, _context));
            bool Ignored = bool.Parse(WebHelper.GetQSValue(Constants.QSKeyNames.Ignored, _context));
            string SpeakerJob = WebHelper.GetQSValue(Constants.QSKeyNames.SpeakerJob, _context);
            string SpeakerName = WebHelper.GetQSValue(Constants.QSKeyNames.SpeakerName, _context);
            string Text = HttpUtility.UrlDecode(WebHelper.GetQSValue(Constants.QSKeyNames.Text, _context));
            string Comments = WebHelper.GetQSValue(Constants.QSKeyNames.Comments, _context);
            string Footer = WebHelper.GetQSValue(Constants.QSKeyNames.Footer, _context);
            Hashtable result = new Hashtable();
            EditorPageMode editPageMode = EditorPageMode.Edit;

            session = null;

            if (EditPageMode != null)
                Enum.TryParse<EditorPageMode>(EditPageMode, out editPageMode);

            if (int.TryParse(WebHelper.GetQSValue(Constants.QSKeyNames.AgendaItemID, _context), out AgendaItemID) && long.TryParse(WebHelper.GetQSValue(Constants.QSKeyNames.SpeakerID, _context), out SpeakerID)
                )
            {
                // check current user privellge
                SessionAudioFile file = SessionFileFacade.GetSessionFilesByID(Session_File_ID);
                if (CurrentUser.ID == file.UserID || (CurrentUser.ID != file.UserID && CurrentUser.Role != UserRole.DataEntry))
                {
                    result["Message"] = "success";

                    session = SessionHelper.GetSessionDetailsByID(Session_ID);

                    if (SpeakerID == -1)
                    {
                        SpeakerName = SpeakerName.Trim();
                        Attendant newAtt = new Attendant();
                        newAtt.Name = SpeakerName;
                        newAtt.JobTitle = SpeakerJob;
                        newAtt.SessionID = session.ID;
                        newAtt.Type = (int)Model.AttendantType.FromOutsideTheCouncil;
                        newAtt.AttendantTitle = "السيد";
                        newAtt.OrderByAttendantType = 6;
                        newAtt.AttendantAvatar = "unknown.jpg";
                        newAtt.State = (int)Model.AttendantState.Attended;
                        newAtt.ShortName = SpeakerName;
                        newAtt.LongName = SpeakerName;
                        newAtt.AttendantDegree = "";
                        AttendantHelper.AddNewSessionAttendant(newAtt, out SpeakerID);
                    }

                    result["SpeakerID"] = SpeakerID.ToString();
                    // save item to DB
                    SessionContentItem currContentItem = null;
                    if (editPageMode == EditorPageMode.Edit)
                        currContentItem = SessionContentItemFacade.GetSessionContentItemByIdAndFragmentOrder(Session_File_ID, FragOrder);
                    else
                    {
                        currContentItem = SessionContentItemHelper.GetSessionContentItemById(long.Parse(CorrectSessionContentItemID));
                    }
                    if (currContentItem != null) // update exist
                    {

                        //int status = (int)currContentItem.StatusID;
                        //if ((int)SpeakerID == (int)Model.AttendantType.UnKnown)
                        //    status = (int)Model.SessionContentItemStatus.Rejected;
                        
                        //ibrahim, in case of unknown attendant, we will check the below two conditions only, which will record the user who did the update
                        //and the status will remain rejected in all cases, neither fixed nor modified after approve

                        AgendaItem sessionUnknownItem = AgendaHelper.GetAgendaItemByNameAndSessionID("غير معرف", Session_ID);
                        if (sessionUnknownItem == null)
                        {
                            sessionUnknownItem = new AgendaItem();
                            sessionUnknownItem.IsCustom = true;
                            sessionUnknownItem.Name = "غير معرف";
                            //sessionUnknownItem.EParliamentID = null;
                            sessionUnknownItem.Order = null;
                            AgendaHelper.AddAgendaItem(sessionUnknownItem, Session_ID);

                        }

                        //usama march
                        if (currContentItem.AgendaItem.IsGroupSubAgendaItems != IsGroupSubAgendaItems)
                        {
                            AgendaHelper.MarkSubAgendaItemsAsGrouped(AgendaItemID, IsGroupSubAgendaItems);
                            currContentItem.AgendaItem.IsGroupSubAgendaItems = IsGroupSubAgendaItems;
                            current_session_info["IsGroupSubAgendaItems"] = IsGroupSubAgendaItems;
                        }

                        long unknownAgendaItemID = AgendaHelper.GetAgendaItemByNameAndSessionID("غير معرف", Session_ID).ID;


                        if ((int)SpeakerID == (int)Model.AttendantType.UnKnown)
                        {
                            int status = (int)Model.SessionContentItemStatus.Rejected;
                            if (CurrentUser.Role != UserRole.DataEntry)
                                SessionContentItemHelper.UpdateSessionContentItem(currContentItem.ID, Text, SpeakerID, AgendaItemID, AgendaSubItemID, SpeakerJob, Comments, Footer, status, true, CurrentUser.ID, SameAsPrevSpeaker, Ignored, long.Parse(AttachID), int.Parse(VoteID),long.Parse(TopicID), MergeWithPrevTopic,IsSessionPresident);
                            else
                                SessionContentItemFacade.UpdateSessionContentItem(currContentItem.ID, Text, SpeakerID, AgendaItemID, AgendaSubItemID, SpeakerJob, Comments, Footer, (Model.SessionContentItemStatus)status, SameAsPrevSpeaker, Ignored, long.Parse(AttachID), int.Parse(VoteID), long.Parse(TopicID),MergeWithPrevTopic, IsSessionPresident);
                        
                        }
                        else
                            if (currContentItem.StatusID == (int)Model.SessionContentItemStatus.Approved && session.SessionStatusID == (int)Model.SessionStatus.Approved)
                            {
                                SessionContentItemFacade.UpdateSessionContentItem(currContentItem.ID, Text, SpeakerID, AgendaItemID, AgendaSubItemID, SpeakerJob, Comments, Footer, Model.SessionContentItemStatus.ModefiedAfterApprove, SameAsPrevSpeaker, Ignored, long.Parse(AttachID), int.Parse(VoteID), long.Parse(TopicID),MergeWithPrevTopic, IsSessionPresident);
                                SessionHelper.UpdateSessionStatus(Session_ID, (int)Model.SessionStatus.Completed);
                            }
                            else if (currContentItem.StatusID == (int)Model.SessionContentItemStatus.Rejected)
                                SessionContentItemFacade.UpdateSessionContentItem(currContentItem.ID, Text, SpeakerID, AgendaItemID, AgendaSubItemID, SpeakerJob, Comments, Footer, Model.SessionContentItemStatus.Fixed, SameAsPrevSpeaker, Ignored, long.Parse(AttachID), int.Parse(VoteID), long.Parse(TopicID), MergeWithPrevTopic,IsSessionPresident);
                            else if (CurrentUser.Role != UserRole.DataEntry)
                                SessionContentItemHelper.UpdateSessionContentItem(currContentItem.ID, Text, SpeakerID, AgendaItemID, AgendaSubItemID, SpeakerJob, Comments, Footer, currContentItem.StatusID, true, CurrentUser.ID, SameAsPrevSpeaker, Ignored, long.Parse(AttachID), int.Parse(VoteID), long.Parse(TopicID),MergeWithPrevTopic, IsSessionPresident);
                            else
                                SessionContentItemFacade.UpdateSessionContentItem(currContentItem.ID, Text, SpeakerID, AgendaItemID, AgendaSubItemID, SpeakerJob, Comments, Footer, (Model.SessionContentItemStatus)currContentItem.StatusID, SameAsPrevSpeaker, Ignored, long.Parse(AttachID), int.Parse(VoteID), long.Parse(TopicID),MergeWithPrevTopic, IsSessionPresident);
                        
                    }
                    else // create new
                    {
                        int? agendaSubItemInt;
                        int temp;
                        if(int.TryParse(AgendaSubItemID.ToString(), out temp))
                            agendaSubItemInt = temp;
                        else
                            agendaSubItemInt = null; 

                        int status = (int)Model.SessionContentItemStatus.DataEntryApproved;
                        if ((int)SpeakerID == (int)Model.AttendantType.UnKnown)
                            status = (int)Model.SessionContentItemStatus.Rejected;


                        SessionContentItemFacade.AddNewSessionContentItem(Session_File_ID, (long)Session_ID, Text, (int)SpeakerID, AgendaItemID, agendaSubItemInt, CurrentUser.ID, status, Comments, SpeakerJob, Footer, SameAsPrevSpeaker, FragOrder, (float)stime, (float)etime, (float)duration, Ignored, long.Parse(AttachID), int.Parse(VoteID), long.Parse(TopicID), MergeWithPrevTopic, IsSessionPresident, (float)soriginaltime);
                    }

                    if (session.SessionStatusID == (int)Model.SessionStatus.Approved)
                        SessionHelper.UpdateSessionStatus(Session_ID, (int)Model.SessionStatus.Completed);
                    if (session.SessionStatusID == (int)Model.SessionStatus.Completed)
                        SessionHelper.UpdateSessionStatus(Session_ID, (int)Model.SessionStatus.InProgress);

                    // update session file LastInsertedFragNumInXml && LastModefiedDate
                    if (editPageMode == EditorPageMode.Edit)
                        SessionFileFacade.UpdateSessionFileFragOrderAndModifiedDate(Session_File_ID, FragOrder, DateTime.Now);
                    else
                        SessionFileHelper.UpdateSessionFileModifiedDate(Session_File_ID, DateTime.Now);
                    //SessionFileHelper.UpdateSessionFileStatus(Session_File_ID, (int)Model.SessionFileStatus.Completed);

                    //update segment last order flag
                    if (LastSegment != null && int.Parse(LastSegment) == 1)
                        SessionFileHelper.UpdateSessionFileLastSegmentFlag(Session_File_ID);

                    int editMode = 0;
                    if (EditPageMode != null && int.TryParse(EditPageMode, out editMode))
                    {
                        if (editMode == 2)
                        {
                            int nRejected = -1;
                            int nModefiedAfterApprove = -1;
                            int nFixed = -1;
                            Hashtable tblStats = EMadbatahFacade.GetSessionStatistics(CurrentUser, session.ID);
                            if (tblStats != null)
                            {
                                nRejected = int.Parse(tblStats[(int)Model.SessionContentItemStatus.Rejected].ToString());
                                nModefiedAfterApprove = int.Parse(tblStats[(int)Model.SessionContentItemStatus.ModefiedAfterApprove].ToString());
                                nFixed = int.Parse(tblStats[(int)Model.SessionContentItemStatus.Fixed].ToString());
                            }

                            var notCompletedSessionFiles = from sf in session.SessionFiles
                                                           where (sf.Status != (int)Model.SessionFileStatus.Completed && sf.IsActive == 1)
                                                           select sf;

                            if (notCompletedSessionFiles.ToList<SessionFile>().Count == 0
                                && tblStats != null
                                && nRejected == 0
                                && nModefiedAfterApprove == 0
                                && nFixed == 0)
                                //&& !DAL.SessionContentItemHelper.DoesSessionContainsUndefinedAttendants(session.ID))
                            {
                                EMadbatahFacade.UpdateSessionStatus(session.ID, Model.SessionStatus.Completed);
                            }
                        }
                    }
                }
                else
                {
                    result["Message"] = "fail";
                }
                return result;
                //break;
            }
            else
                return null;
                //break;
        }
        private void loadSessionValues()
        {
            current_session_info = (Hashtable)_context.Session["current_session_info"];
            FragOrder = (int)current_session_info["FragOrderInXml"];
            stime = (double)current_session_info["PargraphStartTime"];
            etime = (double)current_session_info["PargraphEndTime"];
            soriginaltime = (double)current_session_info["PargraphOriginalStartTime"];
            duration = Math.Round(etime - stime, 2);
            Session_ID = (long)current_session_info["SessionID"];
            Session_File_ID = (long)current_session_info["SessionFileID"];
        }
        private SessionContentItem getReturnedContentItemObject(SessionContentItem nextContentItem)
        {
            SessionContentItem retItem = new SessionContentItem();
            retItem.ID = nextContentItem.ID;
            retItem.AgendaItemID = nextContentItem.AgendaItemID;
            retItem.AgendaSubItemID = nextContentItem.AgendaSubItemID;
            retItem.Text = nextContentItem.Text;
            retItem.AttendantID = nextContentItem.AttendantID;
            retItem.CommentOnAttendant = nextContentItem.CommentOnAttendant;
            retItem.CommentOnText = nextContentItem.CommentOnText;
            retItem.PageFooter = nextContentItem.PageFooter;
            retItem.FragOrderInXml = nextContentItem.FragOrderInXml;
            retItem.AttachementID = nextContentItem.AttachementID;
            retItem.VotingID = nextContentItem.VotingID;
            retItem.TopicID = nextContentItem.TopicID;
            retItem.IsSessionPresident = nextContentItem.IsSessionPresident;
            retItem.MergedTopicWithPrevious = nextContentItem.MergedTopicWithPrevious;
            //retItem.Attendant = nextContentItem.Attendant;
            return retItem;
        }
        private SessionContentItem GetContentFromXML(string fName, int ItemFragOrderInXML)
        {
            TransFile tf = new TransFile();
            string xmlFilePath = _context.Server.MapPath("~") + fName.ToLower().Replace(".mp3", ".trans.xml");
            tf = TayaIT.Enterprise.EMadbatah.Vecsys.VecsysParser.ParseTransXml(xmlFilePath);
            List<Paragraph> p = VecsysParser.combineSegments(tf.SpeechSegmentList);
            // compose HTML spans from speech segments and set editor value
            string html = getHTML(p[ItemFragOrderInXML]);
            // compose SessionItem object
            SessionContentItem item = new SessionContentItem();
            item.Text = html;
            string ItemOrderFlag;
            if (ItemFragOrderInXML == 0)
            {
                //firstFrag = true;
                ItemOrderFlag = "first";
            }
            else if (ItemFragOrderInXML == p.Count - 1)
            {
                //lastFrag = false;
                ItemOrderFlag = "last";
            }
            else
                ItemOrderFlag = "null";

            if (current_session_info != null)
            {
                current_session_info["ItemOrderFlag"] = ItemOrderFlag;
                current_session_info["PargraphStartTime"] = p[ItemFragOrderInXML].speechSegmentsList[0].stime;
                current_session_info["PargraphOriginalStartTime"] = p[ItemFragOrderInXML].speechSegmentsList[0].stime;
                current_session_info["PargraphEndTime"] = p[ItemFragOrderInXML].speechSegmentsList[p[ItemFragOrderInXML].speechSegmentsList.Count - 1].etime;
                if (FragOrder != 0)
                {
                    SessionContentItem prevContentItemTemp = SessionContentItemFacade.GetSessionContentItemByIdAndFragmentOrder(Session_File_ID, FragOrder);
                    current_session_info["PargraphStartTime"] = (double)prevContentItemTemp.EndTime;
                }
            }
            //current_session_info["PargraphStartTime"] = p[FragOrder].speechSegmentsList[0].stime;
            //current_session_info["PargraphEndTime"] = p[FragOrder].speechSegmentsList[p[FragOrder].speechSegmentsList.Count - 1].etime;

            return item;
        }
        private string checkItemPosition(string fName, int ItemFragOrderInXML)
        {
            TransFile tf = new TransFile();
            string xmlFilePath = _context.Server.MapPath("~") + fName.ToLower().Replace(".mp3", ".trans.xml");
            tf = TayaIT.Enterprise.EMadbatah.Vecsys.VecsysParser.ParseTransXml(xmlFilePath);
            List<Paragraph> p = VecsysParser.combineSegments(tf.SpeechSegmentList);
            string ItemOrderFlag;
            if (ItemFragOrderInXML == 0)
                ItemOrderFlag = "first";
            else if (ItemFragOrderInXML == p.Count - 1)
                ItemOrderFlag = "last";
            else
                ItemOrderFlag = "null";
            return ItemOrderFlag;
        }
        private string getHTML(Paragraph p)
        {
            string output = "";
            foreach (SpeechSegment segment in p.speechSegmentsList)
            {
                foreach (TayaIT.Enterprise.EMadbatah.Model.VecSys.Word word in segment.words)
                {
                    //حالة اجراء -- تم كتابة الاجراء فى ملف ال
                    // xml فى حال القطع فقط
                    if (word.stime.ToString() == "0")
                    {
                        if (word.procedureid == "-1")
                            output += "<br />";
                        else
                            output += "<p procedure-id='" + word.procedureid.ToString() + "' style='text-align: " + word.textalign.ToString() + "'>" + word.value.Replace(".", "،").Replace("!####!", "&nbsp;").Replace("#!#!#!", "<br/>") + "</p> ";
                    }
                    else
                        output += "<span class='segment' data-stime='" + word.stime.ToString() + "'>" + word.value.Replace(".", "،").Replace("!####!", "&nbsp;").Replace("#!#!#!", "<br/>") + "</span> ";
                }
            }
            return output;
        }
        private long getPrevSegmentTopicID(int prevItemOrderID)
        {
            if (prevItemOrderID >= 0)
            {
                SessionContentItem prevContentItem = SessionContentItemFacade.GetSessionContentItemByIdAndFragmentOrder(Session_File_ID, prevItemOrderID);
                if (prevItemOrderID == 0)
                {
                    if (prevContentItem.TopicID == null || prevContentItem.TopicID == 0)
                        return 0;

                    else return (long)prevContentItem.TopicID;
                }
                if (prevContentItem.Ignored == true)
                {
                    prevItemOrderID = prevItemOrderID - 1;
                    getPrevSegmentTopicID(prevItemOrderID);
                }
                if (prevContentItem.TopicID == null || prevContentItem.TopicID == 0)
                    return 0;
                else
                    return (long)prevContentItem.TopicID;
            }
            else return 0;
        }

        private long getNextSegmentTopicID()
        {
            try
            {
                loadSessionValues();
                int nextItemOrderID = FragOrder + 1;
                SessionContentItem nextContentItem = SessionContentItemFacade.GetSessionContentItemByIdAndFragmentOrder(Session_File_ID, nextItemOrderID);
                if (nextContentItem.TopicID == null || nextContentItem.TopicID == 0)
                    return 0;
                else
                    return (long)nextContentItem.TopicID;
            }
            catch { return 0; }
        }

        private long getSegmentTopicID()
        {
            loadSessionValues();
            int prevItemOrderID = FragOrder - 1;
            long topicId = 0;
            SessionContentItem currContentItem = SessionContentItemFacade.GetSessionContentItemByIdAndFragmentOrder(Session_File_ID, FragOrder);
            if (FragOrder == 0)
            {
                if (currContentItem == null || currContentItem.TopicID == null || currContentItem.TopicID == 0)
                {
                  /*  Topic tpObj = new Topic();
                    tpObj.SessionID = Session_ID;
                    tpObj.CreatedAt = DateTime.Now;
                    tpObj.UserID = CurrentUser.ID;
                    topicId = TopicHelper.AddTopic(tpObj);*/
                }
                else
                {
                    topicId = (long)currContentItem.TopicID;
                }
            }
            else
            {
                topicId = getPrevSegmentTopicID(prevItemOrderID);
                if (topicId == 0 || topicId == null)
                    topicId = getNextSegmentTopicID();

                if (topicId == 0 || topicId == null)
                {
                  /*  Topic tpObj = new Topic();
                    tpObj.SessionID = Session_ID;
                    tpObj.CreatedAt = DateTime.Now;
                    tpObj.UserID = CurrentUser.ID;
                    topicId = TopicHelper.AddTopic(tpObj);*/
                }
            }

            return topicId;
        }

        public string EditPageMode
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.EDIT_PAGE_MODE, _context);
            }
        }
        public string CorrectSessionContentItemID
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.SESSION_CONTENT_ITEM_ID, _context);
            }
        }

        public string CurrentFragOrder
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.FRAGORDER, _context);
            }
        }
        public string XmlFilePath
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.XMLPATH, _context);
            }
        }
        public string SplittedText
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.SPLITTEDTEXT, _context);
            }
        }


    }
}