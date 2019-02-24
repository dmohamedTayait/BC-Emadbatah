using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.Script.Serialization;
using TayaIT.Enterprise.EMadbatah.Model;
using TayaIT.Enterprise.EMadbatah.BLL;
using TayaIT.Enterprise.EMadbatah.DAL;
using TayaIT.Enterprise.EMadbatah.Config;
using System.IO;
using TayaIT.Enterprise.EMadbatah.Util.Web;
using TayaIT.Enterprise.EMadbatah.Util;
using System.Threading;
using System.Collections;
using System.Data.SqlClient;
using System.Data;


namespace TayaIT.Enterprise.EMadbatah.Web.Framework
{
    public class VotingHandler : BaseHandler
    {

        protected override void HandleRequest()
        {
            string jsonStringOut = null;
            WebFunctions.VotingFunctions function;

            if ((AjaxFunctionName != null && Enum.TryParse<WebFunctions.VotingFunctions>(AjaxFunctionName, true, out function)))
            {
                switch (function)
                {
                    case WebFunctions.VotingFunctions.AddVote:
                        long vote_id = 0;
                        if (!string.IsNullOrEmpty(SessionID) && !string.IsNullOrEmpty(VoteSubject))
                        {
                            vote_id = VoteHelper.AddNewVote(long.Parse(SessionID), VoteSubject);
                            List<Attendant> attLst = AttendantHelper.GetAttendantInSession(long.Parse(SessionID), new List<int> { (int)Model.AttendantType.FromTheCouncilMembers, (int)Model.AttendantType.GovernmentRepresentative, (int)Model.AttendantType.President });
                            foreach (Attendant attObj in attLst)
                                VoteHelper.AddSessionVoteMemberValues(vote_id, attObj.ID, 0);
                        }
 
                        jsonStringOut = SerializeObjectInJSON(vote_id);
                        break;
                    case WebFunctions.VotingFunctions.GetSessionVotes:
                        List<Vote> votes = new List<Vote>();
                        List<SessionHandVote> votesLst = new List<SessionHandVote>();
                        if (!string.IsNullOrEmpty(SessionID))
                        {
                            votes = VoteHelper.GetSessionVotes(long.Parse(SessionID)).ToList();

                            if (votes != null && votes.Count > 0)
                            {
                                foreach (Vote voteObj in votes)
                                {
                                    votesLst.Add(new SessionHandVote(voteObj.ID, (long)voteObj.SessionID, voteObj.VoteSubject, (DateTime)voteObj.CreatedAt, (int)voteObj.TotalNofAgree, (int)voteObj.TotalNofDisagree, (int)voteObj.TotalNofNoVote));
                                }
                            }
                        }
                        jsonStringOut = SerializeObjectInJSON(votesLst);
                        break;
                    case WebFunctions.VotingFunctions.GetVoteMemVal:
                        List<VoteMember> voteMems = VoteHelper.GetSessionVoteMemberValues(long.Parse(VoteID)).ToList();
                        List<SessionMembersVote> voteMemsLst = new List<SessionMembersVote>();
                        if (!string.IsNullOrEmpty(VoteID))
                        {
                            if (voteMems != null && voteMems.Count > 0)
                            {
                                foreach (VoteMember voteObj in voteMems)
                                    voteMemsLst.Add(new SessionMembersVote(voteObj.ID, long.Parse(VoteID), (long)voteObj.AttendantID, AttendantHelper.GetAttendantById((long)voteObj.AttendantID).LongName, (int)voteObj.VoteValue));
                            }
                            else
                            {
                                List<Attendant> attLst = AttendantHelper.GetAttendantInSession(long.Parse(SessionID), new List<int> { (int)Model.AttendantType.FromTheCouncilMembers, (int)Model.AttendantType.GovernmentRepresentative, (int)Model.AttendantType.President });
                                foreach (Attendant attObj in attLst)
                                {
                                    VoteHelper.AddSessionVoteMemberValues(long.Parse(VoteID), attObj.ID, 0);
                                    voteMemsLst.Add(new SessionMembersVote(0, long.Parse(VoteID), (long)attObj.ID, AttendantHelper.GetAttendantById((long)attObj.ID).LongName, 0));
                                }
                            }
                        }
                        jsonStringOut = SerializeObjectInJSON(voteMemsLst);
                        break;
                    case WebFunctions.VotingFunctions.SaveVoteMemVal:
                        List<string> AgreedMemLst = serializer.Deserialize<List<string>>(AgreedMem);
                        List<string> DisAgreedMemLst = serializer.Deserialize<List<string>>(DisAgreedMem);
                        List<string> NoVoteMemLst = serializer.Deserialize<List<string>>(NoVoteMem);
                        foreach (string attID in AgreedMemLst)
                        {
                            VoteHelper.AddSessionVoteMemberValues(long.Parse(VoteID), long.Parse(attID), (int)Model.VoteType.Agree);
                        }
                        foreach (string attID in DisAgreedMemLst)
                        {
                            VoteHelper.AddSessionVoteMemberValues(long.Parse(VoteID), long.Parse(attID), (int)Model.VoteType.Disagree);
                        }
                        foreach (string attID in NoVoteMemLst)
                        {
                            VoteHelper.AddSessionVoteMemberValues(long.Parse(VoteID), long.Parse(attID), (int)Model.VoteType.Novote);
                        }
                       
                        break;
                    default:
                        break;
                }
            }

            if (jsonStringOut != null)
            {
                _context.Response.AddHeader("Encoding", "UTF-8");
                _context.Response.Write(jsonStringOut);
            }
        }
        public string VoteID
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.Vote_ID, _context);
            }
        }
        public string VoteSubject
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.Vote_Subject, _context);
            }
        }

        public string AgreedMem
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.AGREEDMEM, _context);
            }
        }

        public string DisAgreedMem
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.DISAGREEDMEM, _context);
            }
        }

        public string NoVoteMem
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.NOVOTEMEM, _context);
            }
        }
    }
}
