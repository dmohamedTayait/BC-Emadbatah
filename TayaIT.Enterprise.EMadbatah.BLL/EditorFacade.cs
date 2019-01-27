using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TayaIT.Enterprise.EMadbatah.DAL;
using TayaIT.Enterprise.EMadbatah.Model;
using System.Collections;
namespace TayaIT.Enterprise.EMadbatah.BLL
{
    public class EditorFacade
    {
        public static Session GetSessionByID(long sessionID)
        {
            return SessionHelper.GetSessionDetailsByID(sessionID);
        }
        public static List<SessionAgendaItem> GetAgendaSubItemsByAgendaID(long agendaID)
        {
            List<AgendaSubItem> items = AgendaHelper.GetAgendaSubItemsByAgendaID(agendaID);
            List<SessionAgendaItem> subItems = new List<SessionAgendaItem>();
            foreach (AgendaSubItem subItem in items)
                // subItems.Add(new SessionAgendaItem(subItem.ID, subItem.EParliamentID.Value, subItem.EParliamentParentID.Value, subItem.Name, subItem.Order , ));
                subItems.Add(new SessionAgendaItem(subItem.ID, 0, 0, subItem.Name, subItem.Order, subItem.QFrom, subItem.QTo));
            return subItems;
        }
        public static long AddNewAgendaItem(string itemText,int itemIndexed ,long sessionID)
        {
            AgendaItem item = AgendaHelper.GetAgendaItemByName(itemText);
            long itemID;
            if (item == null)
            {
                AgendaItem newitem = new AgendaItem
                {
                    Name = itemText,
                    SessionID = sessionID,
                    IsIndexed = itemIndexed,
                    IsCustom = false,
                    Order = 1
                };
                itemID = AgendaHelper.AddAgendaItem(newitem, sessionID);
                //TayaIT.Enterprise.EMadbatah.DAL.SessionStartHelper.UpdateSessionStartStatus(sessionID, (int)Model.SessionFileStatus.New);
            }
            else
            {
                itemID = item.ID;
            }
            return itemID;
        }
        public static string UpdateAgendaItem(long id, string itemText)
        {
            AgendaHelper.UpdateAgendaItem(id, itemText);
            return "success";
        }
        public static string UpdateAgendaItem(long id, string itemText, int itemIndexed)
        {
            AgendaHelper.UpdateAgendaItem(id, itemText, itemIndexed);
            return "success";
        }
        public static int AssignAttachmentToSessionContentItem(long attachmentID, long sessionContentItemID)
        {
            return SessionContentItemHelper.AssignAttachmentToSessionContentItem(attachmentID, sessionContentItemID);
        }
    }
}
