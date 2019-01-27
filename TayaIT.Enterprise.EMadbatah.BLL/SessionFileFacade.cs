using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TayaIT.Enterprise.EMadbatah.Model;
using TayaIT.Enterprise.EMadbatah.DAL;

namespace TayaIT.Enterprise.EMadbatah.BLL
{
    public static class SessionFileFacade
    {
        public static List<SessionAudioFile> GetSessionFilesBySessionID(long sessionID)
        {


            List<SessionFile> sessionFiles = SessionFileHelper.GetSessionFilesBySessionID(sessionID);
            List<SessionAudioFile> sessionAudioFiles = new List<SessionAudioFile>();

            if (sessionFiles != null && sessionFiles.Count > 0)
            {
                foreach (SessionFile sf in sessionFiles)
                {
                    sessionAudioFiles.Add(new SessionAudioFile(sf.ID, (long)sf.UserID, sf.SessionID, sf.Name, sf.LastInsertedFragNumInXml, sf.Order, sf.DurationSecs, (Model.SessionFileStatus)sf.Status, sf.FileReviewer.FName, (DateTime)sf.LastModefied, sf.FileReviewer == null ? null : sf.FileReviewer.Email, sf.IsSessionStart, sf.SessionStartText, sf.FileReviewerID, (sf.FileReviewer != null) ? sf.FileReviewer.FName : null, (sf.FileReviewer != null) ? sf.FileReviewer.Email : null,(int)sf.IsActive,(int)sf.IsLastSegment));
                }

                return sessionAudioFiles;
            }
            else
                return null;

        }
        public static SessionAudioFile GetSessionFilesByID(long sessionFileID)
        {
            SessionFile sf = new SessionFile();
            sf = SessionFileHelper.GetSessionFileByID(sessionFileID);
            return new SessionAudioFile(sf.ID, sf.UserID, sf.SessionID, sf.Name, sf.LastInsertedFragNumInXml, sf.Order, sf.DurationSecs, (Model.SessionFileStatus)sf.Status, (sf.FileReviewer == null ? null : sf.FileReviewer.FName), sf.LastModefied, sf.FileReviewer == null ? null : sf.FileReviewer.Email, sf.IsSessionStart, sf.SessionStartText, sf.FileReviewerID, (sf.FileReviewer != null) ? sf.FileReviewer.FName : null, (sf.FileReviewer != null) ? sf.FileReviewer.Email : null,(int)sf.IsActive,(int)sf.IsLastSegment);
        }
        public static bool LockSessionFile(long sessionFileID, long userID, bool isAdmin)
        {
            return SessionFileHelper.LockSessionFile(sessionFileID, userID, isAdmin);
        }
        public static int UpdateSessionFileFragOrderAndModifiedDate(long sessionFileID, int FragOrder, DateTime ModifiedDate)
        {
            return SessionFileHelper.UpdateSessionFileFragOrderAndModifiedDate(sessionFileID, FragOrder, ModifiedDate);
        }


        public static bool AddSessionFiles(List<SessionAudioFile> files, long sessionID)
        {
            List<SessionFile> sessionFiles = new List<SessionFile>();
            foreach (SessionAudioFile file in files)
            {
                sessionFiles.Add(new SessionFile()
                {
                    DurationSecs = file.DurationInSec,
                    Name = file.Name,
                    Order = file.Order,
                    Status = (int)file.Status,
                    SessionID = sessionID,
                    IsActive = file.IsActive,
                    IsLastSegment = file.IsLastSegment
                });
            }
            
            return SessionFileHelper.AddNewSessionFiles(sessionFiles);
        }

        public static int GetSessionFilesCount(long sessionID)
        {
            return SessionFileHelper.GetSessionFilesCount(sessionID);
        }


    }
}
