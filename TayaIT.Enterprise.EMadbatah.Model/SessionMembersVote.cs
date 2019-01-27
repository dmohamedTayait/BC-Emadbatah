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


        public SessionMembersVote(int id, int nonSecretVoteSubjectID, int personID, int memberVoteID, string memberFullName)
        {
            ID = id;
            NonSecretVoteSubjectID = nonSecretVoteSubjectID;
            PersonID = personID;
            MemberVoteID = memberVoteID;
            MemberFullName = memberFullName;
        }

        public int ID { get; set; }

        public int NonSecretVoteSubjectID { get; set; }

        public int PersonID { get; set; }
        public int? MemberVoteID { get; set; }
        public string MemberFullName { get; set; }
    }
}
