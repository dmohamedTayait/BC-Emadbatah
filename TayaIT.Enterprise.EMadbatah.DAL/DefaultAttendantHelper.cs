using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TayaIT.Enterprise.EMadbatah.Util;

namespace TayaIT.Enterprise.EMadbatah.DAL
{
    public class DefaultAttendantHelper
    {
        public static List<DefaultAttendant> GetAllDefaultAttendants(bool unassigned)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<DefaultAttendant> DefaultAttendants = new List<DefaultAttendant>();
                    if (unassigned)
                    {
                        DefaultAttendants = context.DefaultAttendants.Select(c => c).Where(c => c.Type != 8 && c.Status == 1).OrderBy(s => s.CreatedAt).ThenBy(s => s.LongName).ToList();
                    }
                    else
                    {
                        DefaultAttendants = context.DefaultAttendants.Select(c => c).Where(c => c.Type != 8 && c.Type != 9 && c.Status == 1).OrderBy(s => s.CreatedAt).ThenBy(s => s.LongName).ToList();
                    }
                   
                    return DefaultAttendants;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.DefaultAttendantHelper.GetAllDefaultAttendants()");
                return null;
            }
        }

        public static List<DefaultAttendant> GetDefaultAttendantsByTypeID(int type)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<DefaultAttendant> DefaultAttendants = new List<DefaultAttendant>();
                    DefaultAttendants = context.DefaultAttendants.Select(c => c).Where(c => c.Type == type && c.Status == 1).OrderBy(s => s.LongName).ThenBy(s => s.CreatedAt).ToList();
                    return DefaultAttendants;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.DefaultAttendantHelper.GetAllDefaultAttendants(" + type.ToString() + ")");
                return null;
            }
        }

        public static List<DefaultAttendant> GetDeletedDefaultAttendants()
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<DefaultAttendant> DefaultAttendants = new List<DefaultAttendant>();

                    DefaultAttendants = context.DefaultAttendants.Select(c => c).Where(c => c.Type != 8 && c.Status == 2).OrderBy(s => s.LongName).ThenBy(s => s.CreatedAt).ToList();
                    return DefaultAttendants;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.DefaultAttendantHelper.GetAllDefaultAttendants()");
                return null;
            }
        }

        public static DefaultAttendant GetAttendantById(long attendant_id)
        {
            try
            {
                DefaultAttendant attendant = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    attendant = context.DefaultAttendants.FirstOrDefault(c => c.ID == attendant_id);
                }
                return attendant;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.GetAttendantById(" + attendant_id + ")");
                return null;
            }
        }

        public static long AddNewDefaultAttendant(DefaultAttendant defAttObj)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<DefaultAttendant> chkIfAttendantExist = context.DefaultAttendants.Where(c => c.LongName == defAttObj.LongName).ToList();
                   if (chkIfAttendantExist.Count == 0)
                   {
                       context.DefaultAttendants.AddObject(defAttObj);
                       int result = context.SaveChanges();
                       return defAttObj.ID;
                   }
                   else
                       return -1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.AddNewDefaultAttendant(defAttObj)");
                return 0;
            }
        }

        public static int UpdateDefaultAttendantById(long attendant_id, string name, string shortName, string longName, string attTitle, string jobTitle, int type,string attDegree)
        {
            try
            {
                DefaultAttendant attendantForUpdate = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    attendantForUpdate = context.DefaultAttendants.FirstOrDefault(c => c.ID == attendant_id);
                    if (attendantForUpdate != null)
                    {
                        attendantForUpdate.AttendantTitle = attTitle;
                        attendantForUpdate.JobTitle = jobTitle;
                        attendantForUpdate.LongName = longName;
                        attendantForUpdate.Name = name;
                        attendantForUpdate.ShortName = shortName;
                        attendantForUpdate.Type = type;
                        attendantForUpdate.AttendantDegree = attDegree;
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

        public static int UpdateDefaultAttendantById(long attendant_id, string name, string shortName, string longName, string attTitle, string attAvatar, string jobTitle, int type, string attendantDegree)
        {
            try
            {
                DefaultAttendant attendantForUpdate = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    attendantForUpdate = context.DefaultAttendants.FirstOrDefault(c => c.ID == attendant_id);
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

        public static int UpdateDefaultAttendantById(long attendant_id, string name, string shortName, string longName, string attTitle, string attAvatar, string jobTitle, int type, int status, string attendantDegree)
        {
            try
            {
                DefaultAttendant attendantForUpdate = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    attendantForUpdate = context.DefaultAttendants.FirstOrDefault(c => c.ID == attendant_id);
                    if (attendantForUpdate != null)
                    {
                        attendantForUpdate.AttendantAvatar = attAvatar;
                        attendantForUpdate.AttendantTitle = attTitle;
                        attendantForUpdate.JobTitle = jobTitle;
                        attendantForUpdate.LongName = longName;
                        attendantForUpdate.Name = name;
                        attendantForUpdate.ShortName = shortName;
                        attendantForUpdate.Type = type;
                        attendantForUpdate.Status = status;
                        attendantForUpdate.AttendantDegree = attendantDegree;
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

        public static int UpdateDefaultAttendantStatusById(long attendant_id, int status)
        {
            try
            {
                DefaultAttendant attendantForUpdate = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    attendantForUpdate = context.DefaultAttendants.FirstOrDefault(c => c.ID == attendant_id);
                    if (attendantForUpdate != null)
                    {
                        attendantForUpdate.Status = status;
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
