using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TayaIT.Enterprise.EMadbatah.DAL;
using TayaIT.Enterprise.EMadbatah.Model;
using TayaIT.Enterprise.EMadbatah.Util;
using TayaIT.Enterprise.EMadbatah.Localization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Globalization;
using System.IO;
using TayaIT.Enterprise.EMadbatah.Pdf2Word;
using TayaIT.Enterprise.EMadbatah.OpenXml.Word;
using System.Xml.Linq;


namespace TayaIT.Enterprise.EMadbatah.BLL
{
    public class MabatahCreatorFacade
    {
        public static string srvMapPath = "";
        public static bool CreateMadbatah(long sessionID, string SessionWorkingDir, string ServerMapPath)
        {
            try
            {
                int coverSize = 1;
                srvMapPath = ServerMapPath;
                List<MadbatahIndexItem> index = new List<MadbatahIndexItem>();
                List<SpeakersIndexItem> speakersIndex = new List<SpeakersIndexItem>();
                TayaIT.Enterprise.EMadbatah.Model.SessionDetails details = SessionStartFacade.GetSessionDetails(sessionID);
                //MadbatahCover
                CreateMadbatahCover(details, SessionWorkingDir + "MadbatahcoverDoc.docx", ServerMapPath);

                int sessionStartSize = 1;
                SessionFile start = SessionStartFacade.GetSessionStartBySessionID(sessionID);
                //HtmlToOpenXml.SaveHtmlToWord(start.SessionStartText, SessionWorkingDir + "sessionStartDoc.docx", ServerMapPath+ "\\resources\\", out sessionStartSize);
                HTMLtoDOCX hd = new HTMLtoDOCX();
                hd.CreateFileFromHTML(start.SessionStartText, SessionWorkingDir + "bodyDoc.docx");
                //  WordprocessingWorker.SaveDOCX(SessionWorkingDir + "bodyDoc.docx", start.SessionStartText, false, 1, 1, .5, .5);

                //Madbatah Body
                int bodySize = MabatahCreatorFacade.CreateMadbatahBody(sessionID, SessionWorkingDir + "bodyDoc.docx", SessionWorkingDir, ServerMapPath, details, out index, out speakersIndex);
                if (bodySize == -1)
                    throw new Exception("Madbatah Body Creation Failed.");

                //Madbatah Index
                int indexSize = 0;
                indexSize = MabatahCreatorFacade.CreateMadbatahIndex(index, SessionWorkingDir, indexSize, SessionWorkingDir + "indexDoc.docx", ServerMapPath, details);
                if (indexSize == -1)
                    throw new Exception("Index with Attachment Creation Failed.");

                //Speakers Index
                int speakerSize = 0;
                speakerSize = MabatahCreatorFacade.CreateSpeakersIndex(speakersIndex, speakerSize, ServerMapPath, SessionWorkingDir + "indexSpeakers.docx");
                if (speakerSize == -1)
                    throw new Exception("Speakers Index Creation Failed.");

                MabatahCreatorFacade.CreateMadbatahIndex(index, SessionWorkingDir, indexSize + speakerSize + coverSize + sessionStartSize, SessionWorkingDir + "indexDoc.docx", ServerMapPath, details);
                MabatahCreatorFacade.CreateSpeakersIndex(speakersIndex, indexSize + speakerSize + coverSize + sessionStartSize, ServerMapPath, SessionWorkingDir + "indexSpeakers.docx");

                //Merge All Generated Files
                List<string> mergeList = new List<string>();
                mergeList.Add(SessionWorkingDir + "indexDoc.docx");
                mergeList.Add(SessionWorkingDir + "indexSpeakers.docx");
                mergeList.Add(SessionWorkingDir + "bodyDoc.docx");
                for (int h = 0; h < bodySize; h++)
                {
                    mergeList.Add(SessionWorkingDir + "bodyDoc" + (h + 1).ToString() + ".docx");
                }
                //Add madbatah Attachments
                List<SessionAttachment> attachment = details.Attachments.ToList();
                foreach (SessionAttachment attach in attachment)
                {
                    attach.FileContent = AttachmentHelper.GetAttachementByID((int)attach.ID).FileContent;
                    mergeList.Add(downloads_attachment_files(SessionWorkingDir, ServerMapPath, attach));
                }

                List<SessionAttachment> votes = details.Votes.ToList();
                foreach (SessionAttachment attach in votes)
                {
                    attach.FileContent = AttachmentHelper.GetAttachementByID((int)attach.ID).FileContent;
                    mergeList.Add(downloads_attachment_files(SessionWorkingDir, ServerMapPath, attach));
                }
                //Madbatah cover
                File.Copy(SessionWorkingDir + "MadbatahcoverDoc.docx", SessionWorkingDir + sessionID + ".docx", true);
                WordprocessingWorker.MergeWithAltChunk(SessionWorkingDir + sessionID + ".docx", mergeList.ToArray());

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatah(" + sessionID + "," + SessionWorkingDir + ")");
                return false;
            }
        }

        private static string downloads_attachment_files(string SessionWorkingDir,string ServerMapPath, SessionAttachment attach)
        {
            System.IO.File.WriteAllBytes(SessionWorkingDir + attach.Name, attach.FileContent);
            FileInfo fInfo = new FileInfo(SessionWorkingDir + attach.Name);

            if (!fInfo.Extension.ToLower().Equals(".pdf"))
                return SessionWorkingDir + attach.Name;
            else
            {
                /* String pdfFilePath = SessionWorkingDir + attach.Name;
                 pdf2ImageConvert.convertPdfFile(pdfFilePath);
                 string wordAttFilePath = SessionWorkingDir + attach.Name.ToLower().Replace(".pdf", ".pdf.docx");
                 string[] files = Directory.GetFiles(SessionWorkingDir + fInfo.Name.Replace(fInfo.Extension, ""));
                 ImageWriter.CreateImageDocument(wordAttFilePath, ServerMapPath + "\\resources\\", files);
                 return SessionWorkingDir + attach.Name.ToLower().Replace(".pdf", ".pdf.docx");
                 //attach.Name = attach.Name.ToLower().Replace(".pdf", ".pdf.docx");*/
                return "";
            }
        }
        public static void CreateMadbatahCover(Model.SessionDetails details, string outCoverPath, string ServerMapPath)
        {
            File.Copy(ServerMapPath + "\\docs\\templates\\MadbatahStartCover.docx", outCoverPath, true);

            //calculate hijri date
            DateTimeFormatInfo dateFormat = Util.DateUtils.ConvertDateCalendar(details.Date, Util.CalendarTypes.Hijri, "en-us");

            string dayNameAr = details.Date.ToString("dddd", dateFormat); // LocalHelper.GetLocalizedString("strDay" + hijDate.DayOfWeek);
            string monthNameAr = LocalHelper.GetLocalizedString("strMonth" + details.Date.Month); //details.Date.Month.ToString();
            string monthNameHijAr = details.Date.ToString("MMMM", dateFormat); //(int.Parse(details.Date.ToString("MMMM", dateFormat))).ToString(); //LocalHelper.GetLocalizedString("strHijMonth"+hijDate.Month);
            string dayOfMonthNumHij = details.Date.Subtract(new TimeSpan(1, 0, 0, 0)).ToString("dd", dateFormat);//hijDate.Day;
            string yearHij = details.Date.ToString("yyyy", dateFormat);  //hijDate.Year;

            try
            {
                int dayOfMonthNumHijNum = int.Parse(dayOfMonthNumHij);
                dayOfMonthNumHij = dayOfMonthNumHijNum.ToString();
            }
            catch
            {
            }
            //for header
            string sessionNum = details.Subject; //"الخامسة عشره";
            string hijriDate = dayOfMonthNumHij + " " + monthNameHijAr + " " + yearHij + " هـ";//" 10 رجب سنة 1431 ه";//"الثلاثاء 10 رجب سنة 1431 ه";
            string gDate = details.Date.Day + " " + monthNameAr + " " + details.Date.Year + " م "; //"22 يونيو سنة 2010 م";

            NumberingFormatter fomratterFemale = new NumberingFormatter(false);
            NumberingFormatter fomratterMale = new NumberingFormatter(true);
            string sessionName = details.EparlimentID.ToString();

            string timeInHour = "الساعة ";
            timeInHour += LocalHelper.GetLocalizedString("strHour" + details.StartTime.Hour);

            string timeInMin = "";
            if (details.StartTime.Minute != 0)
            {
                timeInMin = " و الدقيقة  ";
                timeInMin += fomratterFemale.getResultEnhanced(details.StartTime.Minute);
            }
            // )
            if (details.StartTime.ToString().IndexOf("AM") > 0)
                timeInMin += " صباحاً";

            StringBuilder sb = new StringBuilder();
            sb.Append("<root>");

            //sb.Append("<Name>").Append(sessionName).Append("</Name>");
            string footer = details.Type.Trim() + " (" + details.Subject + ") | " + hijriDate + " - " + gDate;
            sb.Append("<Season>").Append(fomratterMale.getResultEnhanced(int.Parse(details.Season))).Append("</Season>");
            sb.Append("<Name>").Append(details.Type).Append("</Name>");
            sb.Append("<Stage>").Append(fomratterMale.getResultEnhanced(int.Parse(details.Stage)).Trim()).Append("</Stage>");//fomratterMale.getResultEnhanced((int)details.Stage)
            sb.Append("<StageType>").Append(details.StageType.Trim()).Append("</StageType>");
            sb.Append("<Subject>").Append(fomratterFemale.getResultEnhanced(int.Parse(details.Subject))).Append("</Subject>");
            sb.Append("<DateHijri>").Append(hijriDate).Append("</DateHijri>");
            sb.Append("<DateMilady>").Append(gDate).Append("</DateMilady>");
            sb.Append("<StartTime>").Append(timeInHour + timeInMin).Append("</StartTime>");
            sb.Append("<Footer>").Append(footer).Append("</Footer>");
            sb.Append("</root>");

            WordTemplateHandler.replaceCustomXML(outCoverPath, sb.ToString());
        }
        public static string docPath = "";
        public static string sWorkingDir = "";
        public static DocXmlParts xmlFilesPaths = new DocXmlParts();
        public static WordprocessingWorker doc;
        public static int docNum = 0;
        public static int docPageCount = 0;
        public static int CreateMadbatahBody(long sessionID, string outFilePath, string SessionWorkingDir, string ServerMapPath, Model.SessionDetails details, out List<MadbatahIndexItem> index, out List<SpeakersIndexItem> speakersIndex)
        {

            docPath = outFilePath;
            docNum = 0;
            sWorkingDir = SessionWorkingDir;

            string resfolderpath = ServerMapPath + "\\resources\\";

            xmlFilesPaths.CoreFilePropertiesPart = resfolderpath + "core.xml";
            xmlFilesPaths.EndNotes = resfolderpath + "endnotes.xml";
            xmlFilesPaths.FilePropPartPath = resfolderpath + "app.xml";
            xmlFilesPaths.FontTablePart = resfolderpath + "fontTable.xml";
            xmlFilesPaths.FooterPartPath = resfolderpath + "footer1.xml";
            xmlFilesPaths.FootnotesPart = resfolderpath + "footnotes.xml";
            xmlFilesPaths.NumberingDefinitionsPartPath = resfolderpath + "numbering.xml";
            xmlFilesPaths.SettingsPartPath = resfolderpath + "settings.xml";
            xmlFilesPaths.StylePartPath = resfolderpath + "styles.xml";
            xmlFilesPaths.ThemePartPath = resfolderpath + "theme1.xml";
            xmlFilesPaths.WebSettingsPartPath = resfolderpath + "webSettings.xml";
            xmlFilesPaths.HeaderPartPath = resfolderpath + "header.xml";
            try
            {
                doc = new WordprocessingWorker(docPath, xmlFilesPaths, DocFileOperation.Open, true);
                {
                    doc.DocHeaderString = "ــــــــــــــــــــ        " + details.EparlimentID.ToString() + " / " + details.Type + "        ــــــــــــــــــــ";
                    doc.ApplyFooter();

                    index = new List<MadbatahIndexItem>();// Fehres Index for Madbatah Topics
                    speakersIndex = new List<SpeakersIndexItem>();//Fehres index for Madbatah Speakers

                    List<SessionContentItem> allItems = SessionContentItemHelper.GetItemsBySessionIDOrderedBySessionFile(sessionID);//All segments

                    int pageNum = 0;//page num in word

                    long lineNum = 0;//line num in word
                    int ii = 1000;//section ID reserved for images mut be large umber like 1000
                    int j = 0;
                    int k = 0;//loop counter
                    docPageCount = 0;

                    doc.AddParagraph("space", ParagraphStyle.NormalArabic, ParagrapJustification.RTL, false, "");
                    lineNum = doc.CountLineNum(doc, docPath, xmlFilesPaths, srvMapPath, out doc);
                    doc.DeleteLastParagraph("space");
                    if (lineNum != 1)
                        doc.AddParagraph("", ParagraphStyle.NormalArabic, ParagrapJustification.RTL, false, "");

                    List<List<SessionContentItem>> speakerGroup = new List<List<SessionContentItem>>();
                    List<SessionContentItem> newGroup = GroupSpeakerSimilarArticles(allItems, out speakerGroup);//to Group Segments by Speakers 
                    List<SessionContentItem> contentItemGrp = new List<SessionContentItem>();
                    // string currentWorkingDoc = docPath;
                    foreach (SessionContentItem sessionItem in newGroup)
                    {
                        string text = "";
                        string contentItemAsText = "";
                        foreach (SessionContentItem contentItem in speakerGroup[j])
                        {
                            if (!doc.IsOpen)
                                doc = new WordprocessingWorker(docPath, xmlFilesPaths, DocFileOperation.Open);

                            //Prepare AgendaItem to be written in Word Fehres
                            AgendaItem updatedAgenda = contentItem.AgendaItem;
                            if (updatedAgenda.Name != "غير معرف")
                            {
                                pageNum = doc.CountPagesUsingOpenXML(doc, docPath, xmlFilesPaths, ServerMapPath, out doc);
                                int ind = index.FindIndex(curIndexItem => curIndexItem.ID == updatedAgenda.ID);
                                if (ind == -1)
                                {
                                    index.Add(new MadbatahIndexItem(updatedAgenda.ID, updatedAgenda.Name, (pageNum + docPageCount) + "", true, "", "", false, int.Parse(updatedAgenda.IsIndexed.ToString()), false));
                                    if (updatedAgenda.IsIndexed == 1)
                                    {
                                        doc.AddParagraph(TextHelper.StripHTML(updatedAgenda.Name.Trim()), ParagraphStyle.UnderLineParagraphTitle, ParagrapJustification.RTL, false, "");
                                        doc.AddParagraph("space", ParagraphStyle.NormalArabic, ParagrapJustification.RTL, false, "");
                                        lineNum = doc.CountLineNum(doc, docPath, xmlFilesPaths, srvMapPath, out doc);
                                        doc.DeleteLastParagraph("space");
                                        if (lineNum != 1)
                                            doc.AddParagraph("", ParagraphStyle.NormalArabic, ParagrapJustification.RTL, false, "");
                                    }
                                }
                                else
                                    index[ind].PageNum += ", " + (pageNum + docPageCount);
                            }

                            if (k == 0)//First Time only to be Executed 
                            {
                                Attendant att = sessionItem.Attendant;
                                string name_reprsented_n_file = "";
                                if (att.Type == (int)Model.AttendantType.President || att.Type == (int)Model.AttendantType.FromTheCouncilMembers || att.Type == (int)Model.AttendantType.FromOutsideTheCouncil || att.Type == (int)Model.AttendantType.GovernmentRepresentative)
                                {
                                    //Prepare Speaker to be written in Word SpeakersFehres
                                    if (att.Type == (int)Model.AttendantType.President)
                                        name_reprsented_n_file = "الرئيـــس";
                                    else
                                    {
                                        if (contentItem.IsSessionPresident == 1)
                                        {
                                            name_reprsented_n_file = "رئيس الجلسة ";
                                            if (!String.IsNullOrEmpty(contentItem.CommentOnAttendant))
                                                name_reprsented_n_file += " (" + contentItem.CommentOnAttendant.Trim() + ")";
                                        }
                                        else
                                            name_reprsented_n_file = att.Type == (int)Model.AttendantType.FromTheCouncilMembers ? MabatahCreatorFacade.GetAttendantTitleNSpeakersIndex(att) : att.JobTitle;
                                    }

                                    pageNum = doc.CountPagesUsingOpenXML(doc, docPath, xmlFilesPaths, ServerMapPath, out doc);
                                    int itemIndex = speakersIndex.IndexOf(new SpeakersIndexItem(name_reprsented_n_file, (pageNum + docPageCount).ToString(), att.Type));
                                    if (itemIndex == -1)
                                    {
                                        if (att.Type == (int)Model.AttendantType.President)
                                            speakersIndex.Insert(0,new SpeakersIndexItem(name_reprsented_n_file, (pageNum + docPageCount).ToString() + ",", att.Type));
                                        else
                                            speakersIndex.Add(new SpeakersIndexItem(name_reprsented_n_file, (pageNum + docPageCount).ToString() + ",", att.Type));
                                    }
                                    else
                                        speakersIndex[itemIndex].PageNum += (pageNum + docPageCount) + ", ";
                                }
                            }
                            if ((contentItem.TopicID != null && contentItem.TopicID != 0 && contentItem.MergedTopicWithPrevious == false) || contentItem.TopicID == null)
                            {
                                if (contentItemGrp.Count > 0)
                                {
                                    SessionContentItem prevItem = contentItemGrp[contentItemGrp.Count - 1];

                                    if (prevItem.TopicID != null && prevItem.TopicID != 0)
                                    {
                                       /* WriteParagraphInWord(sessionItem, contentItemAsText, contentItemGrp, 0);
                                        List<TopicAttendant> tpcAtts = TopicHelper.GetTopicAttsByTopicID(long.Parse(prevItem.TopicID.ToString()));
                                        List<string> attNamesLst = new List<string>();
                                        for (int u = 0; u < tpcAtts.Count(); u += 2)
                                        {
                                            Attendant att1 = new Attendant();
                                            Attendant att2 = new Attendant();
                                            string attNames = "";
                                            att1 = AttendantHelper.GetAttendantById((long)tpcAtts[u].AttendantID);
                                            // attNames = (att1.AttendantDegree + " " + att1.Name).Trim();
                                            attNames = att1.Name.Trim();
                                            if (u + 1 < tpcAtts.Count())
                                            {
                                                att2 = AttendantHelper.GetAttendantById((long)tpcAtts[u + 1].AttendantID);
                                                // attNames += "," + (att2.AttendantDegree + " " + att2.Name).Trim();
                                                attNames += "," + att2.Name.Trim();
                                            }
                                            attNamesLst.Add(attNames);
                                        }
                                        // doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.Center, false, "");
                                        if (attNamesLst.Count > 0)
                                        {
                                            doc.AddParagraph("مقدمو الطلب", ParagraphStyle.ParagraphTitle, ParagrapJustification.Center, false, "");
                                            doc.AddTable(attNamesLst);
                                        }
                                        doc.AddParagraph("space", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                                        lineNum = doc.CountLineNum(doc, docPath, xmlFilesPaths, srvMapPath, out doc);
                                        doc.DeleteLastParagraph("space");
                                        if (lineNum != 1)
                                            doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                                        text = "";
                                        contentItemAsText = "";
                                        contentItemGrp.Clear();//clear the segments array after writing them in the word*/
                                    }
                                    else
                                    {
                                        if (contentItem.TopicID != null && contentItem.TopicID != 0 && contentItem.MergedTopicWithPrevious == false)
                                        {
                                            WriteParagraphInWord(sessionItem, contentItemAsText, contentItemGrp, 1);
                                            text = "";
                                            contentItemAsText = "";
                                            contentItemGrp.Clear();//clear the segments array after writing them in the word
                                        }
                                    }
                                }
                            }

                            text += " " + contentItem.Text.Trim();
                            contentItemAsText = text.Replace("<br/>", "#!#!#!").Replace("<br>", "#!#!#!").Replace("<br >", "#!#!#!").Replace("<br />", "#!#!#!");
                            contentItemGrp.Add(contentItem);
                            k++;

                       /*     if (contentItem.VotingID != null && contentItem.VotingID != 0)// To write Vote Table
                            {
                                WriteParagraphInWord(sessionItem, contentItemAsText, contentItemGrp, 1);// Copy the previuos segments before witing the attach
                                text = "";
                                contentItemAsText = "";
                                contentItemGrp.Clear();//clear the segments array after writing them in the word

                                TBLNonSecretVoteSubject voteSubject = NonSecretVoteSubjectHelper.GetSessionVoteByVoteID((int)contentItem.VotingID);
                                List<MembersVote> mems = MembersVoteHelper.GetMembersVoteNonSecretVoteID((int)contentItem.VotingID);
                                List<SessionMembersVote> membersVoteLst = new List<SessionMembersVote>();
                                foreach (MembersVote mem in mems)
                                {
                                    SessionMembersVote s = new SessionMembersVote(mem.AutoID, mem.NonSecretVoteSubjectID, mem.PersonID, (int)mem.MemberVoteID, mem.MemberFullName);
                                    membersVoteLst.Add(s);
                                }
                                doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.Center, false, "");
                                doc.AddCustomTable(membersVoteLst);
                                doc.AddParagraph("space", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                                lineNum = doc.CountLineNum(doc, docPath, xmlFilesPaths, srvMapPath, out doc);
                                doc.DeleteLastParagraph("space");
                                if (lineNum != 1)
                                    doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                            }
                            */
                            if (speakerGroup[j].Count == k)// reach the loop end
                            {
                                if (contentItem.TopicID != null && contentItem.TopicID != 0)
                                {
                                    /*WriteParagraphInWord(sessionItem, contentItemAsText, contentItemGrp, 0);
                                    List<TopicAttendant> tpcAtts = TopicHelper.GetTopicAttsByTopicID(long.Parse(contentItem.TopicID.ToString()));
                                    List<string> attNamesLst = new List<string>();
                                    for (int u = 0; u < tpcAtts.Count(); u += 2)
                                    {
                                        Attendant att1 = new Attendant();
                                        Attendant att2 = new Attendant();
                                        string attNames = "";
                                        att1 = AttendantHelper.GetAttendantById((long)tpcAtts[u].AttendantID);
                                        // attNames = (att1.AttendantDegree + " " + att1.Name).Trim(); 
                                        attNames = att1.Name.Trim();
                                        if (u + 1 < tpcAtts.Count())
                                        {
                                            att2 = AttendantHelper.GetAttendantById((long)tpcAtts[u + 1].AttendantID);
                                            // attNames += "," + (att2.AttendantDegree + " " + att2.Name).Trim();
                                            attNames += "," + att2.Name.Trim();
                                        }
                                        attNamesLst.Add(attNames);
                                    }
                                    // doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.Center, false, "");
                                    if (attNamesLst.Count > 0)
                                    {
                                        doc.AddParagraph("مقدمو الطلب", ParagraphStyle.ParagraphTitle, ParagrapJustification.Center, false, "");
                                        doc.AddTable(attNamesLst);
                                    }
                                    doc.AddParagraph("space", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                                    lineNum = doc.CountLineNum(doc, docPath, xmlFilesPaths, srvMapPath, out doc);
                                    doc.DeleteLastParagraph("space");
                                    if (lineNum != 1)
                                        doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");*/
                                }
                                else
                                {
                                    WriteParagraphInWord(sessionItem, contentItemAsText, contentItemGrp, 1);
                                }
                                contentItemGrp.Clear();
                            }
                        }

                        k = 0;
                        text = "";
                        j++;
                    }///loop sessionitem

                    // doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                    doc.AddParagraph("الرئيـــــــــــــــس", ParagraphStyle.ParagraphTitle, ParagrapJustification.LTR, false, "");
                    doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                    doc.AddParagraph("الأمين العـــــام", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                    doc.Save();
                    doc.Dispose();

                    return docNum;
                }
            }
            catch (Exception ex)
            {
                index = new List<MadbatahIndexItem>();
                speakersIndex = new List<SpeakersIndexItem>();
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatahBody(" + sessionID + "," + docPath + ")");
                return -1;
            }
        }

        public static void WriteAttendantInWord(SessionContentItem contentItem, Attendant att)
        {
            if (att.Type != (int)Model.AttendantType.UnAssigned)
            {
                long lineNum = doc.CountLineNum(doc, docPath, xmlFilesPaths, srvMapPath, out doc);
                if (lineNum > 20)
                {
                    doc.AddParagraph("test line num", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                    long newlineNum = doc.CountLineNum(doc, docPath, xmlFilesPaths, srvMapPath, out doc);
                    doc.DeleteLastParagraph("test line num");

                    if (newlineNum != 1)
                    {
                        int pageCnt = doc.CountPagesUsingOpenXML(doc, docPath, xmlFilesPaths, srvMapPath, out doc);
                        if (pageCnt < 15)
                        {
                            doc.AddPageBreak();
                        }
                        else
                        {
                            docPageCount += pageCnt;
                            doc.Dispose();
                            docNum++;
                            docPath = sWorkingDir + "bodyDoc" + docNum.ToString() + ".docx";
                            doc = new WordprocessingWorker(docPath, xmlFilesPaths, DocFileOperation.CreateNew, true);
                        }
                    }
                }

                string attFullPresentationName = "";
                if ((Model.AttendantType)att.Type == Model.AttendantType.President)
                {
                    doc.AddParagraph("الرئيـــس :", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                }
                else
                {
                    if (contentItem.IsSessionPresident == 1)
                    {
                        attFullPresentationName = "رئيس الجلسة ";
                        if (!String.IsNullOrEmpty(contentItem.CommentOnAttendant))
                            attFullPresentationName += " (" + contentItem.CommentOnAttendant.Trim() + ")";
                        attFullPresentationName += " :";
                        doc.AddParagraph(attFullPresentationName, ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                    }
                    else
                    {
                        if(att.Type == (int)Model.AttendantType.FromTheCouncilMembers)
                        {
                            attFullPresentationName = att.AttendantTitle == null ? "النائب " + att.LongName.Trim() : att.AttendantTitle.Trim() + " " + att.LongName.Trim();
                            if (!String.IsNullOrEmpty(contentItem.CommentOnAttendant))
                                attFullPresentationName += " (" + contentItem.CommentOnAttendant.Trim() + ")";
                            attFullPresentationName += " :";
                            doc.AddParagraph(attFullPresentationName, ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                        }
                        else
                        {
                            doc.AddParagraph(att.JobTitle + " :", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                        }
                    }
                }

            }
        }

        public static void WriteParagraphInWord(SessionContentItem sessionItem, string contentItemAsText, List<SessionContentItem> grp, int replace = 0)
        {
            try
            {
                List<string> myCollection = new List<string>();
                List<string> myCollection2 = new List<string>();
                string fullparag = "";
                string parag = "";
                string proc = "";
                for (int i = 0; i < grp.Count; i++)
                {
                    fullparag = TextHelper.StripHTML(grp[i].Text.Replace("#!#!#!", " ")).Trim();
                    string[] p = new string[] { };
                    string[] paragraphs = GetParagraphsArr(grp[i].Text.Replace("<br/>", "#!#!#!").Replace("<br>", "#!#!#!").Replace("<br >", "#!#!#!").Replace("<br />", "#!#!#!"), out p);
                    string[] procedureArr = new string[] { };

                    if (fullparag != "")
                    {
                        myCollection2.Add(grp[i].PageFooter);

                        for (int pp = 0; pp < paragraphs.Length; pp++)
                        {
                            parag += TextHelper.StripHTML(paragraphs[pp].Replace("#!#!#!", " ")).Trim().Replace("&nbsp;", " ");
                            if (replace == 1)
                                myCollection.Add(TextHelper.StripHTML(paragraphs[pp].Replace("#!#!#!", " ")).Trim().Replace("&nbsp;", " "));
                            else myCollection.Add(TextHelper.StripHTML(paragraphs[pp]).Trim().Replace("&nbsp;", " "));
                            if (paragraphs.Length != 1 && pp < p.Length)
                            {
                                if (parag.Trim() != "")
                                {
                                    WriteAttendantInWord(sessionItem, sessionItem.Attendant);
                                    doc.AddParagraph(myCollection, ParagraphStyle.NormalArabic, ParagrapJustification.RTL, true, myCollection2);
                                    doc.AddParagraph("space", ParagraphStyle.NormalArabic, ParagrapJustification.RTL, false, "");
                                    long lineNum = doc.CountLineNum(doc, docPath, xmlFilesPaths, srvMapPath, out doc);
                                    doc.DeleteLastParagraph("space");
                                    if (lineNum != 1)
                                        doc.AddParagraph("", ParagraphStyle.NormalArabic, ParagrapJustification.RTL, false, "");

                                    myCollection.Clear();
                                    myCollection2.Clear();
                                    parag = "";
                                }
                            }

                            if (pp < p.Length)
                            {
                                proc = TextHelper.StripHTML(p[pp].ToString()).Trim();
                                if (proc != "")
                                {
                                    string[] sep = new string[1] { "#!#!#!" };
                                    procedureArr = proc.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                                    ParagrapJustification align = ParagrapJustification.RTL;
                                    if (p[pp].ToString().IndexOf("center") > 0)
                                        align = ParagrapJustification.Center;

                                    for (int x = 0; x < procedureArr.Length; x++)
                                    {
                                        doc.AddParagraph(procedureArr[x].Replace("&nbsp;", " "), ParagraphStyle.ParagraphTitle, align, false, "");
                                    }
                                    doc.AddParagraph("space", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                                    long lineNum = doc.CountLineNum(doc, docPath, xmlFilesPaths, srvMapPath, out doc);
                                    doc.DeleteLastParagraph("space");
                                    if (lineNum != 1)
                                        doc.AddParagraph("", ParagraphStyle.NormalArabic, ParagrapJustification.RTL, false, "");
                                }
                            }
                        }
                    }
                }
                if (myCollection.Count > 0)
                {
                    string lastparag = "";
                    for (int x = 0; x < myCollection.Count; x++)
                    {
                        lastparag += myCollection[x].Replace("&nbsp;", " ");
                    }
                    if (lastparag.Trim() != "")
                    {
                        WriteAttendantInWord(sessionItem, sessionItem.Attendant);
                        doc.AddParagraph(myCollection, ParagraphStyle.NormalArabic, ParagrapJustification.RTL, true, myCollection2);

                        doc.AddParagraph("space", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                        long lineNum = doc.CountLineNum(doc, docPath, xmlFilesPaths, srvMapPath, out doc);
                        doc.DeleteLastParagraph("space");
                        if (lineNum != 1)
                            doc.AddParagraph("", ParagraphStyle.NormalArabic, ParagrapJustification.RTL, false, "");
                    }
                    myCollection.Clear();
                    myCollection2.Clear();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.WriteParagraphInWord(" + TextHelper.StripHTML(contentItemAsText) + ")");
                System.Threading.Thread.ResetAbort();
            }
            // doc.Dispose();
        }

        public static string[] GetParagraphsArr(string parag, out string[] p)
        {
            parag = ClearParagFromEmptyProc(parag);
            MatchCollection matches1, matches2;
            string pattern1 = @"<p (.*?) procedure-id[^>]*?>(.*?)</p>";
            matches1 = Regex.Matches(parag, pattern1);
            string pattern2 = @"<p procedure-id[^>]*?>(.*?)</p>";
            matches2 = Regex.Matches(parag, pattern2);

            p = new string[matches1.Count + matches2.Count];

            for (int i = 0; i < matches1.Count; i++)
            {
                parag = parag.Replace(matches1[i].ToString(), "<%#####%>");
                p[i] = matches1[i].ToString();
            }

            for (int i = 0; i < matches2.Count; i++)
            {
                parag = parag.Replace(matches2[i].ToString(), "<%#####%>");
                p[i + matches1.Count] = matches2[i].ToString();
            }

            string[] sep = new string[1] { "<%#####%>" };
            return parag.Split(sep, StringSplitOptions.None);

        }

        public static string ClearParagFromEmptyProc(string parag)
        {
            MatchCollection matches1, matches2;
            string pattern1 = @"<p (.*?) procedure-id[^>]*?>(.*?)</p>";
            matches1 = Regex.Matches(parag, pattern1);
            string pattern2 = @"<p procedure-id[^>]*?>(.*?)</p>";
            matches2 = Regex.Matches(parag, pattern2);
            string procStr = "";
            string tempstr = "";

            for (int i = 0; i < matches1.Count; i++)
            {
                procStr = matches1[i].ToString();
                tempstr = TextHelper.StripHTML(procStr.Replace("&nbsp;", "")).Trim();
                if (string.IsNullOrEmpty(tempstr))
                    parag = parag.Replace(procStr, "");
            }

            for (int i = 0; i < matches2.Count; i++)
            {

                procStr = matches2[i].ToString();
                tempstr = TextHelper.StripHTML(procStr.Replace("&nbsp;", "")).Trim();
                if (string.IsNullOrEmpty(tempstr))
                    parag = parag.Replace(procStr, "");
            }
            return parag;
        }

        public static string tblBorder = "border:0px";
        public static string pRightDefStyle = SessionStartFacade.marginZeroStyle + SessionStartFacade.defFont + SessionStartFacade.defFontSize1 + SessionStartFacade.textRight;
        public static string pCenterDefStyle = SessionStartFacade.marginZeroStyle + SessionStartFacade.defFont + SessionStartFacade.defFontSize1 + SessionStartFacade.textCenter;
        public static string pRightDefBoldStyle = SessionStartFacade.marginZeroStyle + SessionStartFacade.defFont + SessionStartFacade.defFontWeight + SessionStartFacade.defFontSize1 + SessionStartFacade.textRight;
        public static string pLeftDefBoldStyle = SessionStartFacade.marginZeroStyle + SessionStartFacade.defFont + SessionStartFacade.defFontWeight + SessionStartFacade.defFontSize1 + SessionStartFacade.textLeft;
        public static string pCenterDefBoldStyle = SessionStartFacade.marginZeroStyle + SessionStartFacade.defFont + SessionStartFacade.defFontWeight + SessionStartFacade.defFontSize1 + SessionStartFacade.textCenter;
        public static string pDefStyle = SessionStartFacade.marginZeroStyle + SessionStartFacade.defFont + SessionStartFacade.defFontWeight + SessionStartFacade.defFontSize0 + SessionStartFacade.textCenter;
        public static string emptyParag = "<p style='" + SessionStartFacade.marginZeroStyle + "'>&nbsp;</p>";
        public static int CreateMadbatahIndex(List<MadbatahIndexItem> index, string folderPath, int indexSize, string outIndexPath, string ServerMapPath, Model.SessionDetails details)
        {
            try
            {
                string indexHeader = @"<p style='" + pDefStyle + SessionStartFacade.biglineHeight + "'>فــــــــــــهـــــــــــــــــــــــــــــــرس الـــمـــــــــــــوضــــــــــــوعـــــــــــــــــــــــــات</p>"
                   + emptyParag
                   + "<table style='width:100%;" + tblBorder + SessionStartFacade.lineHeight + SessionStartFacade.directionStyle + SessionStartFacade.marginZeroStyle + "' "
                   + "  <tr style='" + SessionStartFacade.pagebreak + "'>"
                   + "   <th style='" + tblBorder + ";width:85%'><p style='" + pRightDefBoldStyle + SessionStartFacade.biglineHeight + "'>المــوضـــــــوع</p></th>"
                   + "   <th style='" + tblBorder + "'><p style='" + pCenterDefBoldStyle + SessionStartFacade.biglineHeight + "'>الصفحة</p></th>"
                   + " </tr>";


                int i = 1, j = 1;
                string indx = "";
                string emptyRowBold = "<tr style='" + SessionStartFacade.pagebreak + "'>" +
                                          "<td style='" + tblBorder + "'><p style='" + pRightDefStyle + "'>ItemName</p></td>" +
                                          "<td style='" + tblBorder + "' valign='bottom'><p style='" + pCenterDefStyle + "'>PageNum</p></td>" +
                                          "</tr>";
                StringBuilder sb = new StringBuilder();
                sb.Append(indexHeader);

                string toBeReplaced = emptyRowBold.Replace("ItemName", "1. " + "أسماء السادة الأعضاء").Replace("PageNum", (indexSize + 1).ToString());
                sb.Append(toBeReplaced);

                foreach (MadbatahIndexItem item in index)
                {
                    string name = "";
                    string pageNum = "";
                    i++;
                    name = item.Name;
                    pageNum = item.PageNum.ToString();
                    string[] pageNums = pageNum.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    pageNum = (int.Parse(pageNums[0]) + indexSize).ToString();
                    if (item.IsIndexed == 1)
                    {
                        j++;
                        indx = j.ToString();
                    }
                    else
                    {
                        indx = "";
                    }
                    name = indx + " ." +name;
                    toBeReplaced = emptyRowBold.Replace("ItemNum", indx).Replace("ItemName", name).Replace("PageNum", pageNum);
                    sb.Append(toBeReplaced);
                }

                DocXmlParts xmlFilesPaths = WordprocessingWorker.GetDocParts(ServerMapPath + "\\resources\\");
                sb.Append("</table> </tr></table>");

                int stats = 0;

                // HTMLtoDOCX hd = new HTMLtoDOCX();
                // hd.CreateFileFromHTML(sb.ToString(), @outIndexPath);
                WordprocessingWorker.SaveDOCX(@outIndexPath, sb.ToString(), false, 1,1,1,1);

                using (WordprocessingWorker doc = new WordprocessingWorker(outIndexPath, xmlFilesPaths, DocFileOperation.Open))
                {
                    WordprocessingWorker doctmp = doc;
                    stats = doc.CountPagesUsingOpenXML(doc, outIndexPath, xmlFilesPaths, ServerMapPath, out doctmp);
                    doctmp.Dispose();
                }

                return stats;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatahIndexWithAttachment");
                return -1;
            }
        }

        public static int CreateMadbatahIndex2(List<MadbatahIndexItem> index, string folderPath, int indexSize, string outIndexPath, string ServerMapPath, Model.SessionDetails details)
        {
            try
            {
                //calculate hijri date
                DateTimeFormatInfo dateFormat = Util.DateUtils.ConvertDateCalendar(details.Date, Util.CalendarTypes.Hijri, "en-us");
                string dayNameAr = details.Date.ToString("dddd", dateFormat); // LocalHelper.GetLocalizedString("strDay" + hijDate.DayOfWeek);
                string monthNameAr = LocalHelper.GetLocalizedString("strMonth" + details.Date.Month);
                string monthNameHijAr = details.Date.ToString("MMMM", dateFormat); //LocalHelper.GetLocalizedString("strHijMonth"+hijDate.Month);
                string dayOfMonthNumHij = details.Date.Subtract(new TimeSpan(1, 0, 0, 0)).ToString("dd", dateFormat);//hijDate.Day;
                string yearHij = details.Date.ToString("yyyy", dateFormat);  //hijDate.Year;

                try
                {
                    int dayOfMonthNumHijNum = int.Parse(dayOfMonthNumHij);
                    dayOfMonthNumHij = dayOfMonthNumHijNum.ToString();
                }
                catch
                {
                }

                //for header
                string sessionNum = details.Subject; //"الخامسة عشره";
                string hijriDate = dayNameAr + " " + dayOfMonthNumHij + " من " + monthNameHijAr + " سنة " + yearHij + " هـ";//" 10 رجب سنة 1431 ه";//"الثلاثاء 10 رجب سنة 1431 ه";
                string gDate = " " + details.Date.Day + " من " + monthNameAr + " سنة " + details.Date.Year + " م "; //"22 يونيو سنة 2010 م";
                string sessionName = details.Subject + " رقم (" + details.Type + ")";

                string indexHeader = @"<table width='100%' border='0' align='right' style='writing-mode: tb-rl; " + SessionStartFacade.lineHeight + SessionStartFacade.textRight + SessionStartFacade.directionStyle + SessionStartFacade.defFontWeight + "'>"
                  + " <tr>"
                  + "  <p style='" + pCenterDefBoldStyle + "'>بسم الله الرحمن الرحيم</p>"
                  + "  <p style='" + pCenterDefBoldStyle + "'>مجلس الأمة</p>"
                  + "  <p style='" + pCenterDefBoldStyle + "'>الأمانة العامة</p>"
                  + "  <p style='" + pCenterDefBoldStyle + "'>ادارة شؤون المضابط</p>"
                  + emptyParag
                  + "<p style='" + pCenterDefBoldStyle + "'>فهرس الموضوعات</p>"
                  + " </tr>"
                  + " <tr>"
                  + "  <table border='1' style='width:100%;border-collapse:collapse; " + tblBorder + SessionStartFacade.lineHeight + SessionStartFacade.textCenter + SessionStartFacade.directionStyle + SessionStartFacade.defFontWeight + SessionStartFacade.defFontSize1 + SessionStartFacade.marginZeroStyle + "' align='center' cellpadding='5' cellspacing='0'>"
                  + " <tr style='" + SessionStartFacade.pagebreak + "'>"
                  + "   <th style='" + tblBorder + "'><p style='" + SessionStartFacade.marginZeroStyle + "'>م</p></th>"
                  + "   <th style='" + tblBorder + "'><p style='" + SessionStartFacade.marginZeroStyle + "'>الموضوع</p></th>"
                  + "   <th style='" + tblBorder + "'><p style='" + SessionStartFacade.marginZeroStyle + "'>الصفحات</p></th>"
                  + " </tr>";


                int i = 1, j = 1;
                string indx = "";
                string emptyRowBold = "<tr style='" + SessionStartFacade.pagebreak + "'>" +
                                          "<td style='" + tblBorder + "'><p style='" + pCenterDefBoldStyle + "'>ItemNum</p></td>" +
                                          "<td style='" + tblBorder + "'><p style='font-family: UsedFont;" + SessionStartFacade.marginZeroStyle + SessionStartFacade.textRight + SessionStartFacade.defFontSize1 + "'>ItemName</p></td>" +
                                          "<td style='" + tblBorder + "'><p style='" + pCenterDefBoldStyle + "'>PageNum</p></td>" +
                                          "</tr>";
                StringBuilder sb = new StringBuilder();
                sb.Append(indexHeader);

                string toBeReplaced = emptyRowBold.Replace("ItemNum", "1").Replace("ItemName", "- أسماء السادة الأعضاء").Replace("PageNum", (indexSize + 1).ToString()).Replace("UsedFont", "AdvertisingBold");
                sb.Append(toBeReplaced);

                foreach (MadbatahIndexItem item in index)
                {
                    string font = "AdvertisingBold";
                    string name = "";
                    string pageNum = "";
                    i++;
                    name = item.Name;
                    pageNum = item.PageNum.ToString();
                    string[] pageNums = pageNum.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    pageNum = (int.Parse(pageNums[0]) + indexSize).ToString();
                    if (item.IsIndexed == 1)
                    {
                        j++;
                        indx = j.ToString();
                    }
                    else
                    {
                        indx = "";
                        font = "AdvertisingMedium";
                    }
                    toBeReplaced = emptyRowBold.Replace("ItemNum", indx).Replace("ItemName", " - " + name).Replace("PageNum", pageNum).Replace("UsedFont", font);
                    sb.Append(toBeReplaced);
                }

                DocXmlParts xmlFilesPaths = WordprocessingWorker.GetDocParts(ServerMapPath + "\\resources\\");
                sb.Append("</table> </tr></table>");

                int stats = 0;

                // HTMLtoDOCX hd = new HTMLtoDOCX();
                // hd.CreateFileFromHTML(sb.ToString(), @outIndexPath);
                WordprocessingWorker.SaveDOCX(@outIndexPath, sb.ToString(), false, 1, 1, 1, 1);

                using (WordprocessingWorker doc = new WordprocessingWorker(outIndexPath, xmlFilesPaths, DocFileOperation.Open))
                {
                    WordprocessingWorker doctmp = doc;
                    stats = doc.CountPagesUsingOpenXML(doc, outIndexPath, xmlFilesPaths, ServerMapPath, out doctmp);//CountPagesUsingOpenXML(DocumentType.DOCX, folderPath + att.Name);
                    doctmp.Dispose();
                }

                return stats;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatahIndexWithAttachment");
                return -1;
            }
        }

        public static int CreateSpeakersIndex(List<SpeakersIndexItem> index, int increment, string ServerMapPath, string outPath)
        {
            try
            {
                string indexHeader = @"<p style='" + pDefStyle + SessionStartFacade.biglineHeight + "'>فـــــــــــــهـــــــــــــــــــــــــــــــرس الأقــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــوال</p>"
                    + emptyParag
                    + "<table style='width:100%;" + tblBorder + SessionStartFacade.lineHeight + SessionStartFacade.textCenter + SessionStartFacade.directionStyle + SessionStartFacade.marginZeroStyle + "' align='center' cellpadding='3' cellspacing='0'>"
                    + "  <tr style='" + SessionStartFacade.pagebreak + "'>"
                    + "   <th style='" + tblBorder + "width:5%'><p style='" + pCenterDefBoldStyle + SessionStartFacade.biglineHeight + "'>الرقم</p></th>"
                    + "   <th style='" + tblBorder + "width:75%'><p style='" + pCenterDefBoldStyle + SessionStartFacade.biglineHeight + "'>الاســـــــــــــــــــــــم</p></th>"
                    + "   <th style='" + tblBorder + "'><p style='" + pCenterDefBoldStyle + SessionStartFacade.biglineHeight + "'>الصفحــــــــــــــات</p></th>"
                    + " </tr>";


                int i = 1;
                string emptyRowBold = @"<tr style='" + SessionStartFacade.pagebreak + "'>" +
                            "<td style='" + tblBorder + "width:5%'><p style='" + pRightDefStyle + SessionStartFacade.midlineHeight + "'>count</p></td>"+
                            "<td style='" + SessionStartFacade.textRight + tblBorder + ";width:75%'><p style='" + SessionStartFacade.midlineHeight + pRightDefStyle + "'>ItemName</p></td>" +
                            "<td style='" + SessionStartFacade.textRight + tblBorder + "'><p style='" + SessionStartFacade.midlineHeight + pRightDefStyle + "'>PageNum</p></td></tr>";
                StringBuilder sb = new StringBuilder();
                sb.Append(indexHeader);

                foreach (SpeakersIndexItem item in index)
                {

                    string[] parts = item.PageNum.Trim().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> pages = new List<string>();
                    string pagesStr = "";
                    foreach (string part in parts)
                    {
                        if (pages.IndexOf((int.Parse(part) + increment).ToString()) == -1)
                        {
                            pagesStr += (int.Parse(part) + increment) + ", ";
                            pages.Add((int.Parse(part) + increment).ToString());
                        }
                    }
                    pages = parts.Distinct().ToList();

                    if (pagesStr.Length > 2)
                        pagesStr = pagesStr.Remove(pagesStr.Length - 2);
                    sb.Append(emptyRowBold.Replace("ItemName", item.Name.Trim()).Replace("PageNum", pagesStr).Replace("count", i.ToString() + "."));
                    i++;
                }
                sb.Append("</table>");

                int stats = 0;
                //   HTMLtoDOCX hd = new HTMLtoDOCX();
                //   hd.CreateFileFromHTML(sb.ToString(), outPath);
                WordprocessingWorker.SaveDOCX(outPath, sb.ToString(), false, 1,1,1,1);

                DocXmlParts xmlFilesPaths = WordprocessingWorker.GetDocParts(ServerMapPath + "\\resources\\");

                using (WordprocessingWorker doc = new WordprocessingWorker(outPath, xmlFilesPaths, DocFileOperation.Open))
                {
                    WordprocessingWorker doctmp = doc;
                    stats = doc.CountPagesUsingOpenXML(doc, outPath, xmlFilesPaths, ServerMapPath, out doctmp);//CountPagesUsingOpenXML(DocumentType.DOCX, folderPath + att.Name);
                    doctmp.Dispose();
                }
                return stats;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatahIndex(" + ")");
                return -1;
            }
        }

        public static List<SessionContentItem> GroupSpeakerSimilarArticles(List<SessionContentItem> groupedItems, out List<List<SessionContentItem>> speakerContentItem)
        {
            List<SessionContentItem> newGroup = new List<SessionContentItem>();
            List<SessionContentItem> newSpeaker = new List<SessionContentItem>();
            List<List<SessionContentItem>> speakerSessionContentItem = new List<List<SessionContentItem>>();
            speakerContentItem = new List<List<SessionContentItem>>();
            int i = 0;
            long speakerID = 0, nextSpeakerID = 0;
            foreach (SessionContentItem item in groupedItems)
            {
                if (newGroup.Count == 0)
                {
                    item.PageFooter = item.PageFooter == null ? "" : item.PageFooter;
                    newGroup.Add(item);
                    newSpeaker.Add(item);
                    speakerContentItem.Add(newSpeaker);
                    speakerID = item.AttendantID;
                    nextSpeakerID = item.AttendantID;
                }
                else
                {
                    nextSpeakerID = item.AttendantID;
                    if (nextSpeakerID == speakerID)
                    {
                        newSpeaker.Add(item);
                        speakerContentItem[i] = newSpeaker;

                        //  newGroup[newGroup.Count - 1].Text += " " + item.Text;

                        if (!string.IsNullOrEmpty(item.CommentOnAttendant)
                            && !string.IsNullOrEmpty(newGroup[newGroup.Count - 1].CommentOnAttendant)
                            && !newGroup[newGroup.Count - 1].CommentOnAttendant.Contains(item.CommentOnAttendant))
                            newGroup[newGroup.Count - 1].CommentOnAttendant += " - " + item.CommentOnAttendant;

                        if (!string.IsNullOrEmpty(item.CommentOnText)
                            && !string.IsNullOrEmpty(newGroup[newGroup.Count - 1].CommentOnText)
                            && !newGroup[newGroup.Count - 1].CommentOnText.Contains(item.CommentOnText))
                            newGroup[newGroup.Count - 1].CommentOnText += " - " + item.CommentOnText;

                        /*   if (!string.IsNullOrEmpty(item.PageFooter)
                               && !string.IsNullOrEmpty(newGroup[newGroup.Count - 1].PageFooter)
                               && !newGroup[newGroup.Count - 1].PageFooter.Contains(item.PageFooter))
                               newGroup[newGroup.Count - 1].PageFooter += " - " + item.PageFooter;

                        if (item.TopicID != null && item.TopicID != 0
                           && (newGroup[newGroup.Count - 1].TopicID == null || newGroup[newGroup.Count - 1].TopicID == 0))
                            newGroup[newGroup.Count - 1].TopicID = item.TopicID;*/
                    }
                    else
                    {
                        newSpeaker = new List<SessionContentItem>();
                        item.PageFooter = item.PageFooter == null ? "" : item.PageFooter;
                        newSpeaker.Add(item);
                        speakerContentItem.Add(newSpeaker);
                        newGroup.Add(item);
                        speakerID = item.AttendantID;
                        nextSpeakerID = item.AttendantID;
                        i++;
                    }
                }
            }
            return newGroup;
        }

        public static string GetAttendantTitleNSpeakersIndex(Attendant att)
        {
            return (att.AttendantTitle.Trim() + " " + att.AttendantDegree.Trim() + " " + att.Name.Trim()).Trim();

        }

        public static string setFormatAtrrInSpan(string text)
        {
            string temp = text;
            MatchCollection collections = Regex.Matches(text, "<strong.*?</strong>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            foreach (Match match in collections)
            {
                MatchCollection subCollections = Regex.Matches(match.Value, "<span.*?</span>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                foreach (Match submatch in subCollections)
                    temp = temp.Replace(submatch.Value, submatch.Value.Replace("<span ", "<span bold=\"1\" "));
            }
            collections = Regex.Matches(temp, "<em.*?</em>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            foreach (Match match in collections)
            {
                MatchCollection subCollections = Regex.Matches(match.Value, "<span.*?</span>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                foreach (Match submatch in subCollections)
                    temp = temp.Replace(submatch.Value, submatch.Value.Replace("<span ", "<span italic=\"1\" "));
            }

            return temp.Replace("<strong><", "<").Replace("></strong>", ">").Replace("<em>", "").Replace("</em>", "");


        }
    }
}
