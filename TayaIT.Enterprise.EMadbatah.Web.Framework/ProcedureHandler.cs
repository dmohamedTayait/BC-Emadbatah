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
    public class ProcedureHandler : BaseHandler
    {

        protected override void HandleRequest()
        {
            string jsonStringOut = null;
            WebFunctions.ProcedureFunctions function;

            if ((AjaxFunctionName != null && Enum.TryParse<WebFunctions.ProcedureFunctions>(AjaxFunctionName, true, out function)))
            {
                switch (function)
                {
                    case WebFunctions.ProcedureFunctions.GetProcedureTypes:
                        List<ProcedureType> procTypeList = ProcedureHelper.GetProcedureTypes();
                        List<SessionProcedureType> procTypelst = new List<SessionProcedureType>();
                        if (procTypeList != null)
                        {
                            foreach (ProcedureType procTypeItem in procTypeList)
                            {
                                List<SessionProcedure> sessionProcedureLst = new List<SessionProcedure>();
                                foreach (Procedure sessionProcObj in procTypeItem.Procedures)
                                {
                                    SessionProcedure SessionProcedureObj = new SessionProcedure();
                                    SessionProcedureObj.ID = sessionProcObj.ID;
                                    SessionProcedureObj.ProcedureTitle = sessionProcObj.ProcedureTitle;
                                    sessionProcedureLst.Add(SessionProcedureObj);
                                }
                                procTypelst.Add(new SessionProcedureType((long)procTypeItem.ID, (string)procTypeItem.ProcedureTypeStr, (int)procTypeItem.ProcedureTypeOrder, sessionProcedureLst));
                            }
                        }
                        jsonStringOut = SerializeObjectInJSON(procTypelst);
                        break;
                    case WebFunctions.ProcedureFunctions.GetProcedures:
                        List<Procedure> procList = ProcedureHelper.GetProcedures(int.Parse(ProcedureTypeID));
                        List<SessionProcedure> proclst = new List<SessionProcedure>();
                        if (procList != null)
                        {
                            foreach (Procedure procItem in procList)
                            {
                                SessionProcedureType SessionProcedureTypeObj = new SessionProcedureType();
                                SessionProcedureTypeObj.ID = procItem.ProcedureType.ID;
                                SessionProcedureTypeObj.ProcedureType = procItem.ProcedureType.ProcedureTypeStr;
                                SessionProcedureTypeObj.ProcedureTypeOrder = (int)procItem.ProcedureType.ProcedureTypeOrder;
                                proclst.Add(new SessionProcedure(procItem.ID, procItem.ProcedureTitle, SessionProcedureTypeObj));
                            }
                        }
                        jsonStringOut = SerializeObjectInJSON(proclst);
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

        public string ProcedureTypeID
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.PROCEDURE_TYPE_ID, _context);
            }
        }

    }
}
