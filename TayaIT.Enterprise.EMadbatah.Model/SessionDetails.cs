using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Objects;
using System.Collections;

namespace TayaIT.Enterprise.EMadbatah.Model
{
    public class SessionDetails
    {
        public SessionDetails()
        {
            Attendance = new List<SessionAttendant>();
            AgendaItems = new Hashtable();

        }

        public SessionDetails(long sessionID,
            DateTime date,
            SessionStatus sessionStatus,
            List<SessionAttachment> attachments,
            List<SessionAudioFile> sessionFiles,
            List<string> reviewStatus,
            SessionStatus status,
            string subject)
        {

            Date = date;
            Status = sessionStatus;
            Attachments = attachments;
            SessionFiles = sessionFiles;
            ReviewerStatus = reviewStatus;
            SessionID = sessionID;
            Status = status;
            Subject = subject;

        }

        public SessionDetails(int eparlimentID,
            long serial,
            DateTime date,
            DateTime dateHijri,
            DateTime startTime,
            string president,
            string place,
            string season,
            string type,
            string stage,
            string stageType,
            List<SessionAttendant> attendance,
            Hashtable agendaItems,
            SessionStatus status,
            string subject,
            int presidentID)
        {
            Initialize(eparlimentID, -1, serial, date, dateHijri, startTime, president, place, season, type, stageType, stage, null, attendance, null,null, agendaItems, status, subject, 0, presidentID);

        }

        public SessionDetails(int eparlimentID, long sessionID, long serial, DateTime date, DateTime dateHijri, DateTime startTime,
            string president, string place, string season, string type, string stage, string stageType, List<SessionAudioFile> sessionFiles, List<SessionAttendant> attendance,
           List<SessionAttachment> attachments, List<SessionAttachment> votes, Hashtable agendaItems, SessionStatus status, string subject, long? reviewerID, string reviewerName, string mp3FolderPath, int sessionStartFlag, int presidentID)
        {
            Initialize(eparlimentID, sessionID, serial, date, dateHijri, startTime, president, place, season, type, stage, stageType, sessionFiles, attendance, attachments, votes, agendaItems, status, subject, sessionStartFlag, presidentID);
            ReviewerID = reviewerID;
            ReviewerName = reviewerName;
            MP3FolderPath = mp3FolderPath;
        }

        private void Initialize(int eparlimentID,
            long sessionID,
            long serial,
            DateTime date,
            DateTime dateHijri,
            DateTime startTime,
            string president,
            string place,
            string season,
            string type,
            string stage,
            string stageType,
            List<SessionAudioFile> sessionFiles,
            List<SessionAttendant> attendance,
            List<SessionAttachment> attachments,
            List<SessionAttachment> votes,
            Hashtable agendaItems,
            SessionStatus status,
            string subject, int sessionStartFlag, int presidentID)
        {
            Status = status;
            EparlimentID = eparlimentID;
            SessionID = sessionID;
            Serial = serial;
            Date = date;
            DateHijri = dateHijri;
            StartTime = startTime;
            Type = type;
            Presidnt = president;
            Place = place;
            Season = season;
            Stage = stage;
            StageType = stageType;
            Subject = subject;
            SessionStartFlag = (int)sessionStartFlag;
            PresidentID = presidentID;
            if (attendance != null)
                Attendance = attendance;
            else
                Attendance = new List<SessionAttendant>();

            if (agendaItems != null)
                AgendaItems = agendaItems;
            else
                AgendaItems = new Hashtable();

            if (sessionFiles != null)
                SessionFiles = sessionFiles;
            else
                SessionFiles = new List<SessionAudioFile>();

            if (attachments != null)
                Attachments = attachments;
            else
                Attachments = new List<SessionAttachment>();
            if (votes != null)
                Votes = votes;
            else
                Votes = new List<SessionAttachment>();
        }

        public int EparlimentID { get; set; }
        public long Serial { get; set; } //(مسلسل)
        public DateTime Date { get; set; }
        public DateTime DateHijri { get; set; } //date hijri
        public DateTime StartTime { get; set; } //session start time
        public string Presidnt { get; set; } //sessionpresident (رئيس الجلسة)
        public string Place { get; set; } //sessionplace
        public string Type { get; set; } //sessiontype (علنية)
        public string Season { get; set; } // (الفصل)
        public string Stage { get; set; } // (الدور)
        public string StageType { get; set; } //(نوع الدور)
        public List<SessionAttendant> Attendance { get; set; }
        public long UserId { get; set; }
        public List<SessionAttachment> Attachments { get; set; }
        public List<SessionAttachment> Votes { get; set; }
        public List<SessionAudioFile> SessionFiles { get; set; }
        public List<string> ReviewerStatus { get; set; }
        public long SessionID { get; set; }
        public Hashtable AgendaItems { get; set; }
        public SessionStatus Status { get; set; }
        public long? ReviewerID { get; set; }
        public string ReviewerName { get; set; }
        public string Subject { get; set; }
        public int SessionStartFlag { get; set; }
        public int PresidentID { get; set; }
        public string MP3FolderPath { get; set; }
    }
}
