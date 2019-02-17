using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TayaIT.Enterprise.EMadbatah.Util;

namespace TayaIT.Enterprise.EMadbatah.DAL
{
    public static class VoteHelper
    {
        public static long AddNewVote(long sessionID, string voteSubject)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    Vote item = new Vote
                    {
                        VoteSubject = voteSubject,
                        TotalNofAgree = 0,
                        TotalNofDisagree = 0,
                        TotalNofNoVote = 0,
                        CreatedAt = DateTime.Now,
                        SessionID = sessionID
                    };
                    context.Votes.AddObject(item);
                    context.SaveChanges();
                    return item.ID;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttachmentHelper.AddNewVote(" + sessionID + "," + voteSubject + ")");
                return -1;
            }

        }

        public static List<Vote> GetSessionVotes(long sessionID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    return (from obj in context.Votes
                            where obj.SessionID == sessionID
                            select obj).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttachmentHelper.GetSessionVotes(" + sessionID + ")");
                return null;
            }
        }

        public static List<VoteMember> GetSessionVoteMemberValues(long vote_ID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    return context.VoteMembers.Where(c => c.VoteID == vote_ID).ToList<VoteMember>();
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.VoteHelper.GetSessionvoteMemberValues(" + vote_ID + ")");
                return null;
            }
        }
        public static VoteMember AddSessionVoteMemberValues(long vote_ID,long attendant_id,int voteVal)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    VoteMember ifExist = context.VoteMembers.FirstOrDefault(c => c.VoteID == vote_ID && c.AttendantID == attendant_id);
                    if (ifExist != null)
                        ifExist.VoteValue = voteVal;
                    else
                    {
                        VoteMember vm = new DAL.VoteMember();
                        vm.VoteValue = voteVal;
                        vm.AttendantID = attendant_id;
                        vm.VoteID = vote_ID;
                        context.VoteMembers.AddObject(vm);
                    }
                    context.SaveChanges();
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.VoteHelper.GetSessionvoteMemberValues(" + vote_ID + ")");
                return null;
            }
        }
    }
}
