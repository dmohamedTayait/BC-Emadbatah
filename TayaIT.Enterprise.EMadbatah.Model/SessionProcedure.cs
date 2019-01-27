using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TayaIT.Enterprise.EMadbatah.Model
{
    public class SessionProcedure
    {

        public SessionProcedure()
        {
        }


        public SessionProcedure(long procedureID, string procedureTitle, SessionProcedureType sessionProcedureTypeObj)
        {
            ID = procedureID;
            ProcedureTitle = procedureTitle;
            SessionProcedureTypeObj = sessionProcedureTypeObj;
        }

        public long ID { get; set; }

        public string ProcedureTitle { get; set; }

        public SessionProcedureType SessionProcedureTypeObj { get; set; }
    }
}
