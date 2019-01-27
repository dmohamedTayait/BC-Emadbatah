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
        public static string madbatahHeader5 = "التاريخ: %hijridate%";
        public static string madbatahHeader6 = "          %GeorgianDate%";
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

        public static string madbatahStartsection2 = "** وتولى الأمانة العامة السيد علام علي الكندري الأمين العام للمجلس والسيد عادل عيسى اللوغاني الأمين العام المساعد لقطاع الجلسات والسيد محمد عبدالمجيد الخنفر مدير إدارة المضابط .";
        public static string madbatahStartsection3 = "** و حضر الجلسة مندوبو الصحافة و الإعلام و لفيف من السادة المواطنين .";

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
        public static string defFontSize1 = "font-size:20pt;";
        public static string defFontSize2 = "font-size:18pt;";
        public static string lineHeight = "line-height:100%;";
        public static string valign = "vertical-align:top;";

        public static string basicPStyle = defFont + defFontWeight + lineHeight + marginZeroStyle + directionStyle;

        public static string textJustify = "text-align: justify;";
        public static string textJustifyKashida = "text-justify:kashida;";
        public static string textRight = "text-align: right;";
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
            body += emptyParag;
            body += emptyParag;
            body += "<p style='" + basicPStyle + textJustify + defFontSize2 + "'>" + madbatahHeader4.Replace("%type%", details.Type).Replace("%subject%", details.Subject) + "</p>";
            body += "<p style='" + basicPStyle + textJustify + defFontSize2 + "'>" + madbatahHeader5.Replace("%hijridate%", hijriDate) + "</p>";
            body += "<p style='" + basicPStyle + textJustify + defFontSize2 + "'>" + madbatahHeader6.Replace("%GeorgianDate%", gDate) + "</p>";
            body += emptyParag;

            string sessionStart = "<p style='" + basicPStyle + textJustify + defFontSize2 + "'>" + madbatahBodyHeader.Replace("%subject%", femail_numbers.getResultEnhanced(int.Parse(details.Subject)))
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
            List<Attendant> councilMemattendants = AttendantHelper.GetAttendantInSession(details.SessionID, (int) Model.AttendantType.FromTheCouncilMembers) ;
            List<Attendant> governmentMemAttendants = AttendantHelper.GetAttendantInSession(details.SessionID, (int)Model.AttendantType.GovernmentRepresentative);
            if(governmentMemAttendants.Count > 0) {
                body += writeCouncilMemAttendantNFile("", councilMemattendants, false);
                body += "<p style='" + basicPStyle + textJustify + defFontSize2 + "'>و قد مثل الحكومة كل من:</p>";
                body += writeGovernmentMemAttendantNFile("", governmentMemAttendants, false);
                body += emptyParag;
            }

            string madbatahStart = "<html style='" + directionStyle + "'>";
            madbatahStart += "<body dir='" + directionStyle + "'>";
            madbatahStart += body;
          //  madbatahStart += "<p style='" + basicPStyle + textJustify + "'>" + madbatahStartsection2 + "</p>" + emptyParag;
           // madbatahStart += "<p style='" + basicPStyle + textJustify + "'>" + madbatahStartsection3 + "</p>" + emptyParag;
            madbatahStart += "</body></html>";
            return madbatahStart;
        }

        public static string FormatDate(DateTime date)
        {
            DateTime fdate = new DateTime(date.Year, date.Month, date.Day);
            string s = fdate.ToString("d/M/yyyy", CultureInfo.InvariantCulture);

            return s + "  م";
        }

        public static string writeCouncilMemAttendantNFile(string head, List<Attendant> attendants, bool if_status_added)
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
                    if (att.Name != "غير معرف")
                    {
                        att_stats = (Model.AttendantState)att.State == Model.AttendantState.Attended ? "حاضر" : "غير موجود";
                        body += "<tr style='" + pagebreak + "'><td style='width:10%;display:none;'><p style=' " + tdJustifyStyle + " '></p></td><td><p style=' " + tdJustifyStyle + defFontSize2 + " '>" + count.ToString() + ". " + "سعادة " + att.AttendantTitle.Trim() + " " + att.Name.Trim() + "</p>";
                        /*if (!String.IsNullOrEmpty(att.JobTitle))
                            body += "<p style=' " + tdCenterStyle + "'>" + "(" + att.JobTitle.Trim() + ")" + "</p>";*/
                        body += "</td>";
                        /*if(if_status_added)
                            body += "<td  style='" + valign + "'><p style=' " + tdJustifyStyle + "'>" + "(" + att_stats + ")" + "</p></td>";*/
                        body += "</tr>";
                        count++;
                    }
                }
                body += "</table>";
                body += emptyParag;
            }
            return body;
        }

        public static string writeGovernmentMemAttendantNFile(string head, List<Attendant> attendants, bool if_status_added)
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
                    if (att.Name != "غير معرف")
                    {
                        att_stats = (Model.AttendantState)att.State == Model.AttendantState.Attended ? "حاضر" : "غير موجود";
                        body += "<tr style='" + pagebreak + "'><td style=" + valign + "width:65%;" + "><p style=' " + tdJustifyStyle + defFontSize2 + " '>" + att.AttendantTitle.Trim() + " / " + att.Name.Trim() + "</p></td>";
                        if (!String.IsNullOrEmpty(att.JobTitle))
                            body += "<td><p style=' " + tdJustifyStyle + defFontSize2 + " '>" + att.JobTitle.Trim() + "</p></td>";
                       
                        /*if(if_status_added)
                            body += "<td  style='" + valign + "'><p style=' " + tdJustifyStyle + "'>" + "(" + att_stats + ")" + "</p></td>";*/
                        body += "</tr>";
                        count++;
                    }
                }
                body += "</table>";
                body += emptyParag;
            }
            return body;
        }

        public static string writeOutCouncilMemAttendantNFile(string head, List<Attendant> attendants, bool if_status_added)
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
                    if (att.Name != "غير معرف")
                    {
                        att_stats = (Model.AttendantState)att.State == Model.AttendantState.Attended ? "حاضر" : "غير موجود";
                        body += "<tr style='" + pagebreak + "'><td style='width:15%;display:none;'><p style=' " + tdJustifyStyle + " '></p></td><td><p style=' " + tdJustifyStyle + defFontSize2 + " '>" + count.ToString() + ". " + "سعادة " + att.AttendantTitle.Trim() + " " + att.Name.Trim() + "</p>";
                        /*if (!String.IsNullOrEmpty(att.JobTitle))
                            body += "<p style=' " + tdCenterStyle + "'>" + "(" + att.JobTitle.Trim() + ")" + "</p>";*/
                        body += "</td>";
                        /*if(if_status_added)
                            body += "<td  style='" + valign + "'><p style=' " + tdJustifyStyle + "'>" + "(" + att_stats + ")" + "</p></td>";*/
                        body += "</tr>";
                        count++;
                    }
                }
                body += "</table>";
                body += emptyParag;
            }
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
