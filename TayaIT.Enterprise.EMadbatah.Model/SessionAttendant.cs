using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TayaIT.Enterprise.EMadbatah.Model
{
    public class SessionAttendant
    {

        public SessionAttendant()
        { }
        public SessionAttendant(long sID, string name, string jobTitle, AttendantState attendantState, AttendantType attendantType, long attendtantTitleID)
        {
            SessionID = sID;
            Name = name;
            JobTitle = jobTitle;
            State = attendantState;
            Type = attendantType;
            AttendantTitleID = attendtantTitleID;
        }

        public SessionAttendant(long sID, long eMadbatahID, string name, string jobTitle, AttendantState attendantState, AttendantType attendantType, long attendtantTitleID)
        {
            SessionID = sID;
            ID = eMadbatahID;
            Name = name;
            JobTitle = jobTitle;
            State = attendantState;
            Type = attendantType;
            AttendantTitleID = attendtantTitleID;
        }

        public SessionAttendant(long sID, string name, string longname, string jobTitle, AttendantState attendantState, AttendantType attendantType, string attendantdegree,string attendantavatar)
        {
            SessionID = sID;
            Name = name;
            LongName = longname;
            JobTitle = jobTitle;
            State = attendantState;
            Type = attendantType;
            AttendantDegree = attendantdegree;
            AttendantAvatar = attendantavatar;
        }

        public long SessionID { get; set; }
        public long ID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string JobTitle { get; set; }
        public string AttendantDegree { get; set; }
        public string AttendantAvatar { get; set; }
        public AttendantState State { get; set; }
        public AttendantType Type { get; set; }
        public double TotalSpeakTime { get; set; }
        public long AttendantTitleID { get; set; }
    }
}
