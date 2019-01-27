using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TayaIT.Enterprise.EMadbatah.Model
{
    public class SessionProcedureType
    {

        public SessionProcedureType()
        {
           
        }


        public SessionProcedureType(long procedureID, string procedureType, int procedureTypeOrder, List<SessionProcedure> sessionProcedureObj)
        {
            ID = procedureID;
            ProcedureType = procedureType;
            ProcedureTypeOrder = procedureTypeOrder;
            SessionProcedureObj = sessionProcedureObj;
        }

        public long ID { get; set; }

        public string ProcedureType { get; set; }

        public int ProcedureTypeOrder { get; set; }

        public List<SessionProcedure> SessionProcedureObj { get; set; }
    }
}
