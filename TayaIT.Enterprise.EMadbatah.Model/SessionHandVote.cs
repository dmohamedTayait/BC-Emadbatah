using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TayaIT.Enterprise.EMadbatah.Model
{
    public class SessionHandVote
    {

        public SessionHandVote()
        {
        }


        public SessionHandVote(long voteID, long sID, string voteSubject, DateTime createdAt, int nofAgree, int nofDisagree, int nofNoVote)
        {
            ID = voteID;
            SessionID = sID;
            VoteSubject = voteSubject;
            NofAgree = nofAgree;
            NofDisagree = nofDisagree;
            NofNoVote = NofNoVote;
            CreatedAt = createdAt;
        }

        public long ID { get; set; }
        public long SessionID { get; set; }
        public string VoteSubject { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? NofAgree { get; set; }
        public int? NofDisagree { get; set; }
        public int? NofNoVote { get; set; }
    }
}
