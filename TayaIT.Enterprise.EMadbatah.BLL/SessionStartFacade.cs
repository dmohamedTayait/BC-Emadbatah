using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TayaIT.Enterprise.EMadbatah.Model;
using TayaIT.Enterprise.EMadbatah.DAL;
using System.Collections;
using TayaIT.Enterprise.EMadbatah.Localization;
using TayaIT.Enterprise.EMadbatah.Util;
using System.Globalization;
using System.Data.Objects.DataClasses;
namespace TayaIT.Enterprise.EMadbatah.BLL
{
    public class SessionStartFacade
    {
        public static string madbatahHeader1 = "مضبطة الجلسة %type% %subject%";
        public static string madbatahHeader2 = "دور الانعقاد %stage% %stageType%";
        public static string madbatahHeader3 = "الفصل التشريعي %season%";
        public static string madbatahHeader4 = "الرقم: %type% (%subject%)";
        public static string madbatahHeader5 = "%hijridate%";
        public static string madbatahHeader6 = "%GeorgianDate%";
        public static string madbatahBodyHeader = " إنه فى الساعة "
            + "%sessionTime%"
            + " "
            + "من صباح يوم "
            + "%hijriDate%"
            + " ، "
            + " الموافق "
            + "%GeorgianDate%"
            + "، عقدت الجلسة "
            + "%type% %subject%"
            + " لمجلس النواب من دور الانعقاد السنوى "
            + "%stage% %stageType%"
            + " من الفصل التشريعي "
            + "%season%"
            + " بالقاعة الكبرى للاجتماعات بمقر المجلس بالقضيبية، و ذلك برئاسة معالى"
            + " %President% "
            + " %PresidentTitle%"
            + "، و حضر الجلسة أصحاب السعادة النواب:-";

        public static string madbatahStartNotOnTime = "( أخرت الجلسة فى تمام الساعة "
                + "%sessionTime%"
                + " صباحا ثم عقد مجلس الأمة جلسته العادية العلنية فى تمام الساعة "
                + "%sessionTime%"
                + " من صباح يوم "
                + "%hijriDate%"
                + " ، "
                + " الموافق "
                + "%GeorgianDate%"
                + " برئاسة "
                + " %President% "
                + " %PresidentTitle%"
                + ")";

        public static string madbatahPresidentIntro = "بسم الله الرحمن الرحيم، والصلاة والسلام على سيدنا محمد وعلى آله وصحبه  أجمعين. نفتتح جلسة هذا اليوم  " +
            " " + "%day%" +  " الموافق " + "%GeorgianDate%" + " وهي الجلسة " + "%type% %subject%"
            + " من الانعقاد السنوي " + "%stageType% %stage%"
            + " من الفصل التشريعي " + "%season%"
            + ".";
        public static string madbatahStartsection2 = "وحضر الجلسة  " +
            "%out_attendants%" +
            " وعدد من موظفي الأمانة العامة، وقد تفضلت معالي الرئيس بافتتاح الجلسة:";
        public static string madbatahStartsection3 = "(كانت معالي رئيس المجلس قد أعلنت في الساعة التاسعة والنصف عن تأجيل انعقاد الجلسة لحين اكتمال النصاب القانوني للجلسة، وذلك وفقا للمادة (49) من اللائحة الداخلية للمجلس).";

        public static string presidentStr = "السيد الرئيــــــــــــــــــــــــــــــــــس :";
        public static string tempPresidentStr = "السيد رئيس الجلســــــــــــــــــــــة :";
        public static string sessionAttendantTitle = "* وبحضــور الســــادة الأعضــــــــاء : ";
        public static string sessionAttendantTitle2 = "* تليت بعد افتتاح الجلسة أسماء الأعضاء الحاضرين:";
        public static string attendantWithinSessionTitle = "* و فى أثناء الجلسة حضر كل من السادة الأعضـــاء:";
        public static string absentAttendantTitle = "*الغائبـــــــــون بدون عـــــــذر:";
        public static string abologizeAttendantTitle = "* الغائبــــــــــون بعــــــــــــــذر:";

        public static string madbatahIntro = "بسم الله ، و الحمد لله ، و الصلاة و السلام على رسول الله ، تفتح الجلسة و تتلى اسماء السادة الأعضاء ثم اسماء الأعضاء المعتذرين عن جلسة اليوم ، ثم اسماء السادة الأعضاء و الغائبين و المنصرفين عن الجلسة الماضية دون إذن أو إخطار ، ثم أسماء السادة الأعضاء اللذين تغيبوا باعتذار سابق أو بدونه عن عدم حضور اجتماع أو أكثر من الاجتماعات الى عقدتها اللجان منذ الجلسة السابقة .";

        public static string marginZeroStyle = "margin-top:0em;margin-bottom: 0em;";

        public static string directionStyle = "direction:rtl;";
        public static string defFont = "font-family:Sakkal Majalla;";
        public static string defFontWeight = "font-weight:bold;";
        public static string defFontSize0 = "font-size:22pt;";
        public static string defFontSize1 = "font-size:20pt;";
        public static string defFontSize2 = "font-size:18pt;";

        public static string lineHeight = "line-height:100%;";
        public static string midlineHeight = "line-height:115%;";
        public static string biglineHeight = "line-height:150%;";
        public static string valign = "vertical-align:text-top;";
        public static string vbalign = "vertical-align:text-bottom;";
        public static string vcalign = "vertical-align:baseline;";
        public static string textindent = "text-indent: 50px;";

        public static string basicPStyle = defFont + defFontWeight + lineHeight + marginZeroStyle + directionStyle;

        public static string textJustify = "text-align: justify;";
        public static string textJustifyKashida = "text-justify:kashida;";
        public static string textRight = "text-align: right;";
        public static string textLeft = "text-align: left;";
        public static string textCenter = "text-align: center;";
        public static string textunderline = "text-decoration:underline;";
        public static string pagebreak = "page-break-inside:avoid;";

        public static string tableWidth = "width:100%;";
        public static string tableStyle = tableWidth + directionStyle + marginZeroStyle;
        public static string tdJustifyStyle = basicPStyle + textRight + textJustifyKashida;
       // public static string tdCenterStyle = "text-indent: 70px;" + basicPStyle + textRight;
        public static string tdCenterStyle = basicPStyle + textCenter;

        public static string emptyParag = "<p style='" + basicPStyle + "'>&nbsp;</p>";

        public static List<List<Attendant>> GetSessionAttendantOrderedByStatus(long sessionID)
        {
            List<Attendant> sessionAttendants = new List<Attendant>();
            List<Attendant> attendants = new List<Attendant>();
            List<Attendant> attendantsWithinSession = new List<Attendant>();
            List<Attendant> absenceAttendants = new List<Attendant>();
            List<Attendant> abologyAttendants = new List<Attendant>();
            List<Attendant> inMissionAttendants = new List<Attendant>();
            List<List<Attendant>> allAttendants = new List<List<Attendant>>();

            sessionAttendants = AttendantHelper.GetAttendantInSession(sessionID, 1, true);


            foreach (Attendant attendant in sessionAttendants)
            {
                switch ((Model.AttendantState)attendant.State)
                {
                    case Model.AttendantState.Apology:
                        abologyAttendants.Add(attendant);
                        break;
                    case Model.AttendantState.Absent:
                        absenceAttendants.Add(attendant);
                        break;
                    case Model.AttendantState.Attended:
                        attendants.Add(attendant);
                        break;
                    case Model.AttendantState.InMission:
                        inMissionAttendants.Add(attendant);
                        break;
                    case Model.AttendantState.AttendWithinSession:
                        attendantsWithinSession.Add(attendant);
                        break;
                }
            }
            allAttendants.Add(attendants); //بحضور السادة الاعضاء
            allAttendants.Add(attendantsWithinSession); // حضر أثناء الجلسة
            allAttendants.Add(abologyAttendants);//الغائبون بعذر
            allAttendants.Add(absenceAttendants);// الغائبون بدون عذر
            allAttendants.Add(inMissionAttendants);//مهمة


            return allAttendants;
        }

        public static SessionDetails GetSessionDetails(long sessionID)
        {
            return EMadbatahFacade.GetSessionDetailsBySessionID(sessionID);
        }

        public static List<AgendaSubItem> GetAgendaSubItemsbyAgendaID(long agendaItemID)
        {
            return AgendaHelper.GetAgendaSubItemsByAgendaID(agendaItemID);
        }

        public static string GetAutomaticSessionStartText(long sessionID)
        {
            Util.NumberingFormatter mail_numbers = new Util.NumberingFormatter(true);
            Util.NumberingFormatter femail_numbers = new Util.NumberingFormatter(false);

            SessionDetails details = GetSessionDetails(sessionID);
            DateTimeFormatInfo dateFormat = Util.DateUtils.ConvertDateCalendar(details.Date, Util.CalendarTypes.Hijri, "en-us");
            string dayNameAr = details.Date.ToString("dddd", dateFormat); // LocalHelper.GetLocalizedString("strDay" + hijDate.DayOfWeek);
            string monthNameAr = LocalHelper.GetLocalizedString("strMonth" + details.Date.Month);
            string monthNameHijAr = details.Date.ToString("MMMM", dateFormat); //LocalHelper.GetLocalizedString("strHijMonth"+hijDate.Month);
            string dayOfMonthNumHij = details.Date.Subtract(new TimeSpan(1, 0, 0, 0)).ToString("dd", dateFormat);//hijDate.Day;

            try
            {
                int dayOfMonthNumHijNum = int.Parse(dayOfMonthNumHij);
                dayOfMonthNumHij = dayOfMonthNumHijNum.ToString();
            }
            catch
            {
            }

            string yearHij = details.Date.ToString("yyyy", dateFormat);  //hijDate.Year;
            string hijriDate = dayOfMonthNumHij + " " + monthNameHijAr + " " + yearHij + " هـ";//" 10 رجب سنة 1431 ه";//"الثلاثاء 10 رجب سنة 1431 ه";
            string gDate = details.Date.Day + " " + monthNameAr + " " + details.Date.Year + " م "; //"22 يونيو سنة 2010 م";
            string timeInHour = LocalHelper.GetLocalizedString("strHour" + details.StartTime.Hour);// +" " + LocalHelper.GetLocalizedString("strTime" + details.Date.ToString("tt"));//"التاسعة صباحا";
            string stage = mail_numbers.getResultEnhanced(int.Parse(details.Stage)); //;// "الخامس";
            string season = mail_numbers.getResultEnhanced(int.Parse(details.Season));//  + "";// "الرابع عشر";
            string president = "";
            string presidentTitle = "";
            DefaultAttendant att = DefaultAttendantHelper.GetAttendantById(details.PresidentID);
            if (att != null)
            {
                president = (att.AttendantTitle + " " + att.AttendantDegree + " " + att.LongName).Trim();
                if (att.Type == (int)Model.AttendantType.President)
                    presidentTitle = "رئيس مجلس الأمة";
                else if (att.Type != (int)Model.AttendantType.President && att.JobTitle != null)
                    presidentTitle = att.JobTitle;

            }

            string body = "<p style='" + basicPStyle + textCenter + defFontSize1 + "'>" + madbatahHeader1.Replace("%type%", details.Type).Replace("%subject%", femail_numbers.getResultEnhanced(int.Parse(details.Subject))) + "</p>";
            body += "<p style='" + basicPStyle + textCenter + defFontSize1 + "'>" + madbatahHeader2.Replace("%stageType%", details.StageType).Replace("%stage%", stage) + "</p>";
            body += "<p style='" + basicPStyle + textCenter + defFontSize1 + "'>" + madbatahHeader3.Replace("%season%", season) + "</p>";
            body += emptyParag;
            body += "<p style='" + basicPStyle + textJustify + defFontSize2 + "'>" + madbatahHeader4.Replace("%type%", details.Type).Replace("%subject%", details.Subject) + "</p>";
            body += "<table style='" + directionStyle + marginZeroStyle + ";'>";
            body += "<tr>";
            body += "<td><p style='" + basicPStyle + textJustify + defFontSize2 + "'>التاريخ:</p></td>";
            body += "<td><p style='" + basicPStyle + textJustify + defFontSize2 + "'>" + madbatahHeader5.Replace("%hijridate%", hijriDate) + "</p></td>";
            body += "</tr>";
            body += "<tr>";
            body += "<td></td>";
            body += "<td><p style='" + basicPStyle + textJustify + defFontSize2 + "'>" + madbatahHeader6.Replace("%GeorgianDate%", gDate) + "</p></td>";
            body += "</tr>";
            body += "</table>";
            body += emptyParag;

            string sessionStart = "<p style='" + basicPStyle + textJustify + defFontSize2 + textindent + "'>" + madbatahBodyHeader.Replace("%subject%", femail_numbers.getResultEnhanced(int.Parse(details.Subject)))
                .Replace("%season%", season)
                .Replace("%type%", details.Type)
                .Replace("%stageType%", details.StageType)
                .Replace("%stage%", stage)
                .Replace("%GeorgianDate%", gDate)
                .Replace("%sessionTime%", timeInHour)
                .Replace("%hijriDate%", hijriDate)
                .Replace("%President%", president)
                .Replace("%PresidentTitle%", presidentTitle) + "</p>";
            body += sessionStart;
            body += emptyParag;
            List<Attendant> councilMemattendants = AttendantHelper.GetAttendantInSession(details.SessionID, (int)Model.AttendantType.FromTheCouncilMembers);
            if (councilMemattendants.Count > 0)
            {
                body += writeCouncilMemAttendantNFile("", councilMemattendants, (int)Model.AttendantState.Attended);
            }
            List<Attendant> governmentMemAttendants = AttendantHelper.GetAttendantInSession(details.SessionID, (int)Model.AttendantType.GovernmentRepresentative);
            if (governmentMemAttendants.Count > 0)
            {
                body += emptyParag;
                body += "<p style='" + basicPStyle + textJustify + defFontSize2 +  "'>و قد مثل الحكومة كل من:</p>";
                body += writeGovernmentMemAttendantNFile("", governmentMemAttendants, (int)Model.AttendantState.Attended);
            }
            List<Attendant> outsideAttendants = AttendantHelper.GetAttendantInSession(details.SessionID, new List<int> { (int)Model.AttendantType.FromOutsideTheCouncil, (int)Model.AttendantType.Secretariat, (int)Model.AttendantType.SecretaryPresident }, 0);
            if (outsideAttendants.Count > 0)
            {
                body += emptyParag;
                body += writeOutCouncilMemAttendantNFile(outsideAttendants);
            }

            string madbatahStart = "<html style='" + directionStyle + "'>";
            madbatahStart += "<body dir='" + directionStyle + "'>";
            madbatahStart += body;
            if (details.SessionStartFlag == (int)SessionOpenStatus.NotOnTime)
                madbatahStart += "<p style='" + basicPStyle + textCenter + "'>" + madbatahStartsection3 + "</p>";
            body += "<p style='" + basicPStyle + textJustify + defFontSize1  + defFontWeight + "'>الرئيس: </p>";
            body += "<p style='" + basicPStyle + textJustify + defFontSize2 + textindent + "'>" + madbatahPresidentIntro.Replace("%subject%", femail_numbers.getResultEnhanced(int.Parse(details.Subject)))
                .Replace("%season%", season)
                .Replace("%type%", details.Type)
                .Replace("%stageType%", details.StageType)
                .Replace("%stage%", stage)
                .Replace("%GeorgianDate%", gDate)
                .Replace("%sessionTime%", timeInHour)
                .Replace("%hijriDate%", hijriDate)
                .Replace("%day%", dayNameAr) + "</p>";
            madbatahStart += "</body></html>";
            return madbatahStart;
        }

        public static string GetPresidentIntro(long sessionID)
        {
            Util.NumberingFormatter mail_numbers = new Util.NumberingFormatter(true);
            Util.NumberingFormatter femail_numbers = new Util.NumberingFormatter(false);

            SessionDetails details = GetSessionDetails(sessionID);
            DateTimeFormatInfo dateFormat = Util.DateUtils.ConvertDateCalendar(details.Date, Util.CalendarTypes.Hijri, "en-us");
            string dayNameAr = details.Date.ToString("dddd", dateFormat); // LocalHelper.GetLocalizedString("strDay" + hijDate.DayOfWeek);
            string monthNameAr = LocalHelper.GetLocalizedString("strMonth" + details.Date.Month);
            string monthNameHijAr = details.Date.ToString("MMMM", dateFormat); //LocalHelper.GetLocalizedString("strHijMonth"+hijDate.Month);
            string dayOfMonthNumHij = details.Date.Subtract(new TimeSpan(1, 0, 0, 0)).ToString("dd", dateFormat);//hijDate.Day;

            try
            {
                int dayOfMonthNumHijNum = int.Parse(dayOfMonthNumHij);
                dayOfMonthNumHij = dayOfMonthNumHijNum.ToString();
            }
            catch
            {
            }

            string yearHij = details.Date.ToString("yyyy", dateFormat);  //hijDate.Year;
            string hijriDate = dayOfMonthNumHij + " " + monthNameHijAr + " " + yearHij + " هـ";//" 10 رجب سنة 1431 ه";//"الثلاثاء 10 رجب سنة 1431 ه";
            string gDate = details.Date.Day + " من شهر " + monthNameAr + " لسنة " + details.Date.Year + " م "; //"22 يونيو سنة 2010 م";
            string timeInHour = LocalHelper.GetLocalizedString("strHour" + details.StartTime.Hour);// +" " + LocalHelper.GetLocalizedString("strTime" + details.Date.ToString("tt"));//"التاسعة صباحا";
            string stage = mail_numbers.getResultEnhanced(int.Parse(details.Stage)); //;// "الخامس";
            string season = mail_numbers.getResultEnhanced(int.Parse(details.Season));//  + "";// "الرابع عشر";
      
            string body = madbatahPresidentIntro.Replace("%subject%", femail_numbers.getResultEnhanced(int.Parse(details.Subject)))
                .Replace("%season%", season)
                .Replace("%type%", details.Type)
                .Replace("%stageType%", details.StageType)
                .Replace("%stage%", stage)
                .Replace("%GeorgianDate%", gDate)
                .Replace("%sessionTime%", timeInHour)
                .Replace("%hijriDate%", hijriDate)
                .Replace("%day%", dayNameAr);
            List<Attendant> abologyAttendants = AttendantHelper.GetAttendantInSession(details.SessionID, new List<int> { (int)Model.AttendantType.FromTheCouncilMembers }, (int)Model.AttendantState.Apology);
            List<Attendant> absentAttendants = AttendantHelper.GetAttendantInSession(details.SessionID, new List<int> { (int)Model.AttendantType.FromTheCouncilMembers }, (int)Model.AttendantState.Absent);
            List<string> attstrings = new List<string>();
            string attendant_str = "";
            if (abologyAttendants.Count > 0 || absentAttendants.Count > 0)
            {
                body += " البند (1): تلاوة أسماء: ";
                if (abologyAttendants.Count > 0)
                {
                    body += " الإخوة المعتذرين عن عدم حضور الجلسة وهم أصحاب السعادة: ";
                    foreach (Attendant att in abologyAttendants)
                    {
                        if (att.Name != "غير معرف")
                        {
                            attendant_str = "";
                            attendant_str += att.AttendantTitle.Trim() + " " + att.AttendantDegree.Trim() + " " + att.LongName.Trim();
                            if (!string.IsNullOrEmpty(att.AbsenseExcuse.Trim()))
                                attendant_str += " (" + att.AbsenseExcuse.Trim() + ")، ";
                            attstrings.Add(attendant_str);
                        }
                    }
                    body += string.Join("، ", attstrings);
                   
                }
                if (abologyAttendants.Count > 0 && absentAttendants.Count > 0)
                {
                    body += " وقد تغيب عن حضور الجلسة أصحاب السعادة النواب ";
                }
                else if (abologyAttendants.Count == 0 && absentAttendants.Count > 0)
                {
                    body += " الإخوة المتغيبين عن عدم حضور الجلسة وهم أصحاب السعادة: ";
                }
                attstrings.Clear();
                foreach (Attendant att in absentAttendants)
                {
                    if (att.Name != "غير معرف")
                    {
                        attendant_str = att.AttendantTitle.Trim() + " " + att.AttendantDegree.Trim() + " " + att.LongName.Trim();
                        attstrings.Add(attendant_str);
                    }
                }
                body += string.Join("، ", attstrings);
            }

            return body;
        }

        public static string FormatDate(DateTime date)
        {
            DateTime fdate = new DateTime(date.Year, date.Month, date.Day);
            string s = fdate.ToString("d/M/yyyy", CultureInfo.InvariantCulture);

            return s + "  م";
        }

        public static string writeCouncilMemAttendantNFile(string head, List<Attendant> attendants, int status_filter)
        {
            string body = "";
            if (attendants.Count > 0)
            {
                int table_width = 100;
                if (head != "")
                {
                    body += "<p style='" + basicPStyle + textunderline + textRight + "'>" + head + "</p>";
                }
                body += "<table style='" + tableStyle + ";width:" + table_width.ToString() + "%'>";
                int count = 1;
                string att_stats = "1";
                foreach (Attendant att in attendants)
                {
                    if (att.Name != "غير معرف" && att.State == status_filter)
                    {
                        att_stats = (Model.AttendantState)att.State == Model.AttendantState.Attended ? "حاضر" : "غير موجود";
                        body += "<tr style='" + pagebreak + "'><td style='width:10%;display:none;'><p style=' " + tdJustifyStyle + " '></p></td><td><p style=' " + tdJustifyStyle + defFontSize2 + " '>" + count.ToString() + ". " + "سعادة " + att.AttendantTitle.Trim() + " " + att.Name.Trim() + "</p>";
                        /*if (!String.IsNullOrEmpty(att.JobTitle))
                            body += "<p style=' " + tdCenterStyle + "'>" + "(" + att.JobTitle.Trim() + ")" + "</p>";*/
                        body += "</td>";
                        /*if(if_status_added)
                            body += "<td ><p style=' " + tdJustifyStyle + "'>" + "(" + att_stats + ")" + "</p></td>";*/
                        body += "</tr>";
                        count++;
                    }
                }
                body += "</table>";
            }
            return body;
        }

        public static string writeGovernmentMemAttendantNFile(string head, List<Attendant> attendants, int status_filter)
        {
            string body = "";
            if (attendants.Count > 0)
            {
                int table_width = 100;
                if (head != "")
                {
                    body += "<p style='" + basicPStyle + textunderline + textRight + "'>" + head + "</p>";
                }
                body += "<table style='" + tableStyle + ";width:" + table_width.ToString() + "%'>";
                int count = 1;
                string att_stats = "1";
                foreach (Attendant att in attendants)
                {
                    if (att.Name != "غير معرف" && att.State == status_filter)
                    {
                        att_stats = (Model.AttendantState)att.State == Model.AttendantState.Attended ? "حاضر" : "غير موجود";
                        body += "<tr style='" + pagebreak + "'><td style=" + "width:65%;" + "><p style=' " + tdJustifyStyle + defFontSize2 + " '>" + att.AttendantTitle.Trim() + " / " + att.Name.Trim() + "</p></td>";
                        if (!String.IsNullOrEmpty(att.JobTitle))
                            body += "<td><p style=' " + tdJustifyStyle + defFontSize2 + " '>" + att.JobTitle.Trim() + "</p></td>";
                       
                        /*if(if_status_added)
                            body += "<td ><p style=' " + tdJustifyStyle + "'>" + "(" + att_stats + ")" + "</p></td>";*/
                        body += "</tr>";
                        count++;
                    }
                }
                body += "</table>";
            }
            return body;
        }

        public static string writeOutCouncilMemAttendantNFile(List<Attendant> attendants)
        {
            string body = "";
            string attendant_str = "";
            if (attendants.Count > 0)
            {
                int count = 1;
                foreach (Attendant att in attendants)
                {
                    if (att.Name != "غير معرف")
                    {
                        attendant_str += att.AttendantTitle.Trim() + " " + att.AttendantDegree.Trim() + " " + att.LongName.Trim() + " " + att.JobTitle.Trim() + "، ";
                        count++;
                    }
                }
            }
            body = "<p style='" + basicPStyle + textJustify + defFontSize2 + textindent + "'>" + madbatahStartsection2.Replace("%out_attendants%", attendant_str) + "</p>";
            return body;
        }

        public static bool AddUpdateSessionStart(long sessionId, string sessionStartText, long userID, string startName)
        {

            //Session session = SessionHelper.GetSessionByID(sessionId);
            SessionFile sessionStart = SessionStartHelper.GetSessionStartBySessionId(sessionId);
            if (sessionStart == null)
            {
                DAL.SessionFile start = SessionStartHelper.AddSessionStart(sessionStartText, userID, sessionId, startName);
                if (start != null)
                    return true;
                else
                    return false;
                //SessionStartFacade.UpdateSessionSetSessionStartID(sessionId, start.ID);
            }
            else
            {
                if (SessionStartHelper.UpdateSessionStartText(sessionStart.ID, sessionStartText, userID) > 0)
                    return true;
                else
                    return false;
            }


        }

        public static bool AddNewSessionStart(long sessionId, string sessionStartText, string startName)
        {


            DAL.SessionFile start = SessionStartHelper.AddSessionStart(sessionStartText, sessionId, startName);
            if (start != null)
                return true;
            else
                return false;
            //SessionStartFacade.UpdateSessionSetSessionStartID(sessionId, start.ID);


        }

        public static SessionFile GetSessionStartBySessionID(long sessionID)
        {
            return SessionStartHelper.GetSessionStartBySessionId(sessionID);
        }
    }
}
