using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Collections;
using TayaIT.Enterprise.EMadbatah.Util;
using System.Data.Objects.DataClasses;
//using TayaIT.Enterprise.EMadbatah.Model;

namespace TayaIT.Enterprise.EMadbatah.DAL
{
    public static class ProcedureHelper
    {
        #region Procedure

        public static List<ProcedureType> GetProcedureTypes()
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<ProcedureType> allProcedureTypes = new List<ProcedureType>();
                    if (context.ProcedureTypes.Count<ProcedureType>() > 0)
                    {
                        allProcedureTypes = context.ProcedureTypes.Include("Procedures").ToList<ProcedureType>();
                    }
                    return allProcedureTypes;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SessionHelper.GetProcedureTypes()");
                return null;
            }
        }

        public static List<Procedure> GetProcedures(int procedureTypeID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<Procedure> allProcedures = new List<Procedure>();
                    if (context.Procedures.Count<Procedure>() > 0)
                    {
                        if (procedureTypeID != 0)
                        {
                            allProcedures = context.Procedures.Include("ProcedureType").Where(c => c.ProcedureTypeID == procedureTypeID).ToList<Procedure>();

                        }
                        else
                        {
                            allProcedures = context.Procedures.ToList<Procedure>();

                        }
                    }
                    return allProcedures;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SessionHelper.GetProcedures(" + procedureTypeID + ")");
                return null;
            }
        }

        public static int UpdateProcedure(long ProcedureID, string ProcedureTitle)
        {
            try
            {
                Procedure ProcedureForUpdate = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    ProcedureForUpdate = context.Procedures.FirstOrDefault(a => a.ID == ProcedureID);
                    if (ProcedureForUpdate != null)
                    {
                        ProcedureForUpdate.ProcedureTitle = ProcedureTitle;
                    }
                    
                    int res = context.SaveChanges();
                    return res;
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.ProcedureHelper.UpdateProcedure(" + ProcedureID + "," + ProcedureTitle + ")");
                return -1;
            }
        }

        public static int UpdateProcedureType(long ProcedureTypeID, string ProcedureTypeStr)
        {
            try
            {
                ProcedureType ProcedureTypeForUpdate = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    ProcedureTypeForUpdate = context.ProcedureTypes.FirstOrDefault(a => a.ID == ProcedureTypeID);
                    if (ProcedureTypeForUpdate != null)
                    {
                        ProcedureTypeForUpdate.ProcedureTypeStr = ProcedureTypeStr;
                    }

                    int res = context.SaveChanges();
                    return res;
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.ProcedureHelper.UpdateProcedure(" + ProcedureTypeID + "," + ProcedureTypeStr + ")");
                return -1;
            }
        }

        public static int DeleteProcedureById(long ProcedureID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    Procedure procedureObj = context.Procedures.FirstOrDefault(c => c.ID == ProcedureID);
                    context.DeleteObject(procedureObj);
                    int result = context.SaveChanges();
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.ProcedureHelper.DeleteProcedureById(" + ProcedureID + ")");
                return -1;
            }
        }

        public static int DeleteProcedureTypeById(long ProcedureTypeID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<Procedure> procedureLstObj = context.Procedures.Where(c => c.ProcedureTypeID == ProcedureTypeID).ToList();
                    foreach (Procedure procedureObj in procedureLstObj)
                    {
                        context.DeleteObject(procedureObj);
                    }
                    int result = context.SaveChanges();
                    ProcedureType procedureTypeObj = context.ProcedureTypes.FirstOrDefault(c => c.ID == ProcedureTypeID);
                    context.DeleteObject(procedureTypeObj);
                    result = context.SaveChanges();
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.ProcedureHelper.DeleteProcedureById(" + ProcedureTypeID + ")");
                return -1;
            }
        }

        public static string GetProcedureTypeByID(int ID)
        {
            string procedureTitle = "";
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<ProcedureType> allProcedures = new List<ProcedureType>();
                    if (context.ProcedureTypes.Count<ProcedureType>() > 0)
                    {
                        procedureTitle = context.ProcedureTypes.Where(c => c.ID == ID).First().ProcedureTypeStr;
                    }
                    return procedureTitle;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SessionHelper.GetProcedureTypeByID(" + ID + ")");
                return null;
            }
        }

        public static long AddNewProcedureType(ProcedureType procTypeObj)
        {
            try
            {

                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    context.ProcedureTypes.AddObject(procTypeObj);
                    int result = context.SaveChanges();
                    return procTypeObj.ID;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SessionHelper.AddNewProcedureType(procTypeObj)");
                return -1;
            }
        }

        public static long AddNewProcedure(Procedure procObj)
        {
            try
            {

                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    context.Procedures.AddObject(procObj);
                    int result = context.SaveChanges();
                    return procObj.ID;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SessionHelper.AddNewProcedureType(procTypeObj)");
                return -1;
            }
        }

        #endregion
    }
}
