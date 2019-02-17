using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TayaIT.Enterprise.EMadbatah.Model
{
    public class SessionMembersVote
    {
        public SessionMembersVote()
        {
        }

        public SessionMembersVote(long id, long voteID, long attID, string attName, int voteVal)
        {
            ID = id;
            VoteID = voteID;
            AttendantID = attID;
            AttendantName = attName;
            MemberVoteValue = voteVal;
        }

        public long ID { get; set; }
        public long VoteID { get; set; }
        public long AttendantID { get; set; }
        public string AttendantName { get; set; }
        public int? MemberVoteValue { get; set; }
      
    }
}
