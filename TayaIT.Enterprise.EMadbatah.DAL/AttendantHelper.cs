using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TayaIT.Enterprise.EMadbatah.Util;

namespace TayaIT.Enterprise.EMadbatah.DAL
{
    public class AttendantHelper
    {

        public static int GenerateSessionAttendants(long sid, Session current_session, bool edit)
        {
            try
            {
                EMadbatahEntities context = new EMadbatahEntities();
                List<DefaultAttendant> DefaultAttendants = context.DefaultAttendants.Select(aa => aa).Where(aa => aa.Status == 1).ToList();
                Attendant chIfAddedb4 = null;
                for (int i = 0; i < DefaultAttendants.Count; i++)
                {
                    chIfAddedb4 = null;
                    Attendant attendant = fillAttendant(DefaultAttendants[i], current_session);
                    if (edit)
                        chIfAddedb4 = current_session.Attendants.FirstOrDefault(c => c.DefaultAttendantID == attendant.DefaultAttendantID);

                    if (chIfAddedb4 == null)
                        AddNewSessionAttendant(attendant);
                }

                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.GenerateSessionAttendants()");
                return -1;
            }
        }

        public static bool AddNewSessionAttendant(Attendant attendant)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    context.Attendants.AddObject(attendant);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.AddNewAttendant()");
                return false;
            }
        }

        public static bool AddNewSessionAttendant(Attendant newAttendant, out long AttendantID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    Attendant attendant = context.Attendants.FirstOrDefault(c => c.Name == newAttendant.Name && c.SessionID == newAttendant.SessionID);
                    if (attendant != null)
                        AttendantID = attendant.ID;
                    else
                    {
                        context.Attendants.AddObject(attendant);
                        context.SaveChanges();
                        AttendantID = newAttendant.ID;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                AttendantID = 0;
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.GetUnknownAttendantId(" + newAttendant.SessionID + ")");
                return false;
            }
        }

        public static Attendant fillAttendant(DefaultAttendant defAtt, Session sessionObj)
        {
            Attendant attendant = new Attendant();
            attendant.Name = defAtt.Name;
            attendant.JobTitle = defAtt.JobTitle;
            attendant.DefaultAttendantID = defAtt.ID;
            attendant.SessionID = sessionObj.ID;
            attendant.Type = defAtt.Type;
            attendant.AttendantTitle = defAtt.AttendantTitle;
            attendant.OrderByAttendantType = defAtt.OrderByAttendantType;
            attendant.AttendantAvatar = defAtt.AttendantAvatar;
            attendant.State = defAtt.Type ==  3 ? 2 : 1; //if he is from government, so by default he is absent
            attendant.ShortName = defAtt.ShortName;
            attendant.LongName = defAtt.LongName;
            attendant.CreatedAt = defAtt.CreatedAt;
            attendant.AttendantDegree = defAtt.AttendantDegree;
            return attendant;
        }

        public static int DeleteAttendantByID(long attendant_id)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    Attendant attendant = context.Attendants.FirstOrDefault(c => c.ID == attendant_id);
                    context.DeleteObject(attendant);
                    int res = context.SaveChanges();
                    return res;
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.DeleteAttendantByID(" + attendant_id + ")");
                return -1;
            }
        }

        public static Attendant GetAttendantById(long attendant_id)
        {
            try
            {
                Attendant attendant = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    attendant = context.Attendants.FirstOrDefault(c => c.ID == attendant_id);
                }
                return attendant;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.GetAttendantById(" + attendant_id + ")");
                return null;
            }
        }

        public static Attendant GetAttendantByDefaultAttendantId(long attendant_id)
        {
            try
            {
                Attendant attendant = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    attendant = context.Attendants.FirstOrDefault(c => c.DefaultAttendantID == attendant_id);
                }
                return attendant;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.GetAttendantById(" + attendant_id + ")");
                return null;
            }
        }

        public static long GetUnknownAttendantId(long sessionID)
        {
            try
            {

                Attendant attendant = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    attendant = context.Sessions.FirstOrDefault(c => c.ID == sessionID).Attendants.FirstOrDefault(c => c.Name == "غير معرف");
                    if (attendant != null)
                        return attendant.ID;
                    else
                    {
                        attendant = new Attendant
                        {
                            Name = "غير معرف",
                            JobTitle = "",
                            SessionID = sessionID,
                            Type = 7,
                            State = 1
                        };
                        context.Attendants.AddObject(attendant);
                        context.SaveChanges();
                        return attendant.ID;

                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.GetUnknownAttendantId(" + sessionID + ")");
                return -1;
            }
        }

        public static List<Attendant> GetAttendants(long sessionID, long agendaitemID, long subagendaid, bool isAllSpeakers)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<Attendant> attendants = new List<Attendant>();
                    List<SessionContentItem> scis = null;

                    if (!isAllSpeakers && (agendaitemID < 0 || subagendaid < 0))
                    {
                        attendants = (from att in context.Sessions.FirstOrDefault(s => s.ID == sessionID).Attendants
                                      where att.State != null && att.State == 1
                                      select att).ToList<Attendant>();
                        return attendants;
                    }

                    if (subagendaid > 0)
                        scis = context.AgendaSubItems.FirstOrDefault(c => c.ID == subagendaid).SessionContentItems.ToList<SessionContentItem>();
                    else if (agendaitemID > 0)
                        scis = context.AgendaItems.FirstOrDefault(c => c.ID == agendaitemID).SessionContentItems.ToList<SessionContentItem>();

                    if (scis != null)
                    {
                        foreach (SessionContentItem sci in scis)
                        {
                            if (!attendants.Contains(sci.Attendant))
                            {
                                if (isAllSpeakers)
                                    attendants.Add(sci.Attendant);
                                else if (sci.Attendant.State != null && sci.Attendant.State.Value == 1)
                                    attendants.Add(sci.Attendant);
                            }
                        }
                        return attendants;
                    }


                }
                return null;

            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.GetAttendantsByAgendaItemID(" + agendaitemID + ")");
                return null;
            }
        }

        public static List<Attendant> GetAttendantInSession(long SessionID, int attendantType)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<Attendant> attendantsInTime = context.Attendants.Select(aa => aa).Where(ww => ww.Type == attendantType && ww.SessionID == SessionID).OrderBy(s => s.LongName).ThenBy(s => s.CreatedAt).ToList();
                    return attendantsInTime;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<Attendant> GetAttendantInSession(long SessionID, List<int> attendantTypes)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<Attendant> attendantsInTime = context.Attendants.Select(aa => aa).Where(ww => ww.SessionID == SessionID && ww.State == 1 && attendantTypes.Contains((int)ww.Type)).OrderBy(s => s.LongName).ThenBy(s => s.CreatedAt).ToList();
                    return attendantsInTime;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<Attendant> GetAttendantInSession(long SessionID, int SessionAttendantType, bool excluded)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    long sessionPresidentID = 0;
                    List<Attendant> attendantsInTime = new List<Attendant>();
                    if (excluded)
                    {
                        Session sessionObj = SessionHelper.GetSessionByID(SessionID);
                        if (sessionObj.PresidentID != null && sessionObj.PresidentID != 0)
                        {
                            Attendant attObj = AttendantHelper.GetAttendantByDefaultAttendantId(long.Parse(sessionObj.PresidentID.ToString()));
                            sessionPresidentID = attObj.ID;
                            attendantsInTime = context.Attendants.Select(aa => aa).Where(ww => ww.Type != 8 && ww.Type != 9 && ww.DefaultAttendantID != sessionObj.PresidentID && ww.SessionID == SessionID).OrderBy(s => s.LongName).ThenBy(s => s.CreatedAt).ToList();
                        }
                    }
                    else
                    {
                        attendantsInTime = context.Attendants.Select(aa => aa).Where(ww => ww.Type != 8 && ww.Type != 9 && ww.SessionID == SessionID).OrderBy(s => s.LongName).ThenBy(s => s.CreatedAt).ToList();
                    }
                    return attendantsInTime;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool UpdateAttendantState(int AttendantStatus, int DefaultAttendantId, string absenseReason)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    Attendant atendant = context.Attendants.Select(eee => eee).Where(aaa => aaa.ID == DefaultAttendantId).FirstOrDefault();
                    atendant.State = AttendantStatus;
                    if (absenseReason.ToString() != "")
                    {
                        atendant.AbsenseExcuse = absenseReason;
                    }

                    if (AttendantStatus != 0)
                    {
                        int res = context.SaveChanges();
                    }
                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static int UpdateAttendantById(long attendant_id, string name, string shortName, string longName, string attTitle, string attAvatar, string jobTitle, int type, string attendantDegree)
        {
            try
            {
                Attendant attendantForUpdate = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    attendantForUpdate = context.Attendants.FirstOrDefault(c => c.ID == attendant_id);
                    if (attendantForUpdate != null)
                    {
                        attendantForUpdate.AttendantAvatar = attAvatar;
                        attendantForUpdate.AttendantTitle = attTitle;
                        attendantForUpdate.JobTitle = jobTitle;
                        attendantForUpdate.LongName = longName;
                        attendantForUpdate.Name = name;
                        attendantForUpdate.ShortName = shortName;
                        attendantForUpdate.Type = type;
                        attendantForUpdate.AttendantDegree = attendantDegree;
                        //  attendantForUpdate.CreatedAt = DateTime.Now;
                    }
                    return context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.GetAttendantById(" + attendant_id + ")");
                return 0;
            }
        }
    }
}
