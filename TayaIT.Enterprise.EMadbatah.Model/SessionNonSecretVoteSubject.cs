using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TayaIT.Enterprise.EMadbatah.Model
{
    public class SessionNonSecretVoteSubject
    {

        public SessionNonSecretVoteSubject()
        {
        }


        public SessionNonSecretVoteSubject(long nonSecretVoteSubjectID, int meetingID, int nonSecretVoteSubjectNumber, string nonSecretVoteSubject, DateTime nonSecretVoteSubjectDate, bool nonSecreatVoteSubjectIsClosed,
            int nofAgree, int nofDisagree, int nofNoVote, int nofAttendance)
        {
            ID = nonSecretVoteSubjectID;
            MeetingID = meetingID;
            NonSecretVoteSubjectNumber = nonSecretVoteSubjectNumber;
            NonSecretVoteSubjectDate = nonSecretVoteSubjectDate;
            NonSecreatVoteSubjectIsClosed = nonSecreatVoteSubjectIsClosed;
            NonSecretVoteSubject = nonSecretVoteSubject;
            NofAgree = nofAgree;
            NofDisagree = nofDisagree;
            NofNoVote = NofNoVote;
            NofAttendance = nofAttendance;
        }

        public long ID { get; set; }
        public int MeetingID { get; set; }
        public int NonSecretVoteSubjectNumber { get; set; }
        public string NonSecretVoteSubject { get; set; }
        public DateTime NonSecretVoteSubjectDate { get; set; }
        public bool NonSecreatVoteSubjectIsClosed { get; set; }
        public int? NofAgree { get; set; }
        public int? NofDisagree { get; set; }
        public int? NofNoVote { get; set; }
        public int? NofAttendance { get; set; }
    }
}
