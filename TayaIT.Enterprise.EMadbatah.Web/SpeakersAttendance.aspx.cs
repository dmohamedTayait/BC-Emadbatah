using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TayaIT.Enterprise.EMadbatah.BLL;
using TayaIT.Enterprise.EMadbatah.DAL;
using System.Text;
using TayaIT.Enterprise.EMadbatah.Config;
using TayaIT.Enterprise.EMadbatah.Model;
using System.IO;
using System.Collections;
using TayaIT.Enterprise.EMadbatah.Localization;
using TayaIT.Enterprise.EMadbatah.Util;


namespace TayaIT.Enterprise.EMadbatah.Web
{
    public partial class SpeakersAttendance : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                EMadbatahEntities ee = new EMadbatahEntities();
                List<Session> Sessions = ee.Sessions.Where(c => c.SessionStatusID != (int)Model.SessionStatus.FinalApproved).Select(a => a).OrderByDescending(aa => aa.ID).ToList();

                ListItem liNew = new ListItem("-- اختر --", "0");
                ddlSessions.Items.Insert(0, liNew);
                foreach (Session sessionObj in Sessions)
                {
                    liNew = new ListItem("( " + sessionObj.EParliamentID.ToString() + " )", sessionObj.ID.ToString());
                    if (SessionID != null)
                    {
                        if (sessionObj.ID == long.Parse(SessionID))
                            liNew.Selected = true;
                    }
                    ddlSessions.Items.Add(liNew);
                }

                fill_attendant_type();

                if (SessionID != null)
                {
                    ddlSessions.SelectedValue = SessionID;
                    fill_gv_attendants(long.Parse(SessionID));
                }
            }
        }

        protected void ddlSessions_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblInfo1.Visible = false;
            lblInfo2.Visible = false;

            long SessionID = long.Parse(ddlSessions.SelectedValue);
            Session sessionObj = SessionHelper.GetSessionByID(SessionID);
            btnSave.Style.Add("display", "");

            ddlAttendantTypes.Style.Add("display", "block");
            fill_attendant_type();
            fill_gv_attendants(SessionID);
           
            bind_script();
        }

        protected void ddlAttendantTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblInfo1.Visible = false;
            lblInfo2.Visible = false;
           
            long SessionID = long.Parse(ddlSessions.SelectedValue);
            Session sessionObj = SessionHelper.GetSessionByID(SessionID);
            btnSave.Style.Add("display", "");

            if (ddlAttendantTypes.SelectedValue == ((int)Model.AttendantType.FromTheCouncilMembers).ToString() || ddlAttendantTypes.SelectedValue == ((int)Model.AttendantType.GovernmentRepresentative).ToString())
            {
                fill_gv_attendants(SessionID);
            }
            else
            {

            }

            bind_script();
        }

        protected void GVAttendants_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int SessionID = int.Parse(ddlSessions.SelectedValue);

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Int64 AttendantID = (Int64)DataBinder.Eval(e.Row.DataItem, "ID");
                Session sessionObj = SessionHelper.GetSessionByID(SessionID);

                EMadbatahEntities ee = new EMadbatahEntities();
                List<Attendant> attendant = ee.Attendants.Select(aa => aa).Where(ww => ww.ID == AttendantID).ToList();

                RadioButtonList rb = (RadioButtonList)e.Row.FindControl("RBLAttendantStates");
                TextBox txtAbsenceExcuse = (TextBox)e.Row.FindControl("txtAbsenceExcuse");
                if (attendant.Count != 0)
                {
                    if (rb.Items.FindByValue(attendant[0].State.ToString()) != null)
                    {
                        rb.Items.FindByValue(attendant[0].State.ToString()).Selected = true;
                    }
                    if (attendant[0].State == (int)Model.AttendantState.Apology)
                    {
                        txtAbsenceExcuse.Text = attendant.First().AbsenseExcuse;
                        txtAbsenceExcuse.Style.Add("display", "block");
                    }
                    else
                    {
                        txtAbsenceExcuse.Text = "";
                        txtAbsenceExcuse.Style.Add("display", "none");
                    }
                   
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblInfo1.Text = "يتم الان حفظ بياناتك برجاء الاتنظار";
            lblInfo1.Visible = true;
            lblInfo2.Text = "يتم الان حفظ بياناتك برجاء الاتنظار";
            lblInfo2.Visible = true;
            foreach (GridViewRow item in GVAttendants.Rows)
            {

                int SessionID = int.Parse(ddlSessions.SelectedValue);
                HiddenField HFID = item.Cells[0].FindControl("HFID") as HiddenField;

                int DefaultAttendantId = int.Parse(HFID.Value);

                RadioButtonList rdlist = item.Cells[2].FindControl("RBLAttendantStates") as RadioButtonList;
                TextBox txtAbsenceExcuse = item.Cells[3].FindControl("txtAbsenceExcuse") as TextBox;

                int AttendantStatus = 0;
                if (rdlist.SelectedItem != null)
                {
                    AttendantStatus = int.Parse(rdlist.SelectedValue);
                }

                if (AttendantStatus != 0)
                {
                    string absenceExcuse = AttendantStatus == (int)Model.AttendantState.Apology ? txtAbsenceExcuse.Text : "";
                    AttendantHelper.UpdateAttendantState(AttendantStatus, DefaultAttendantId, absenceExcuse);
                }

            }
            lblInfo1.Text = "تم الحفظ بنجاح";
            lblInfo1.Visible = true;
            lblInfo2.Text = "تم الحفظ بنجاح";
            lblInfo2.Visible = true;
        }

        private void fill_attendant_type()
        {
            ddlAttendantTypes.Items.Clear();
            
            ddlAttendantTypes.Items.Insert(0, new ListItem("نواب المجلس", ((int)Model.AttendantType.FromTheCouncilMembers).ToString()));
            ddlAttendantTypes.Items.Insert(1, new ListItem("ممثلى الحكومة", ((int)Model.AttendantType.GovernmentRepresentative).ToString()));
            ddlAttendantTypes.Items.Insert(2, new ListItem("خارج المجلس", ((int)Model.AttendantType.FromOutsideTheCouncil).ToString()));
            ddlAttendantTypes.Items[0].Selected = true;
        }

        private void fill_gv_attendants(long sessionID)
        {
            List<Attendant> attendantsLst = AttendantHelper.GetAttendantInSession(sessionID, int.Parse(ddlAttendantTypes.SelectedValue));
            GVAttendants.DataSource = attendantsLst;
            GVAttendants.DataBind();
            GVAttendants.Style.Add("display", "block");
        }

        private void fill_gv_external_attendants(long sessionID)
        {
            List<Attendant> attendantsLst = AttendantHelper.GetAttendantInSession(sessionID, int.Parse(ddlAttendantTypes.SelectedValue));
            GVAttendants.DataSource = attendantsLst;
            GVAttendants.DataBind();
            GVAttendants.Style.Add("display", "block");
        }

        private void bind_script()
        {
            string myScript = "\n<script type=\"text/javascript\" language=\"Javascript\" id=\"EventScriptBlock\">\n";
            myScript += "rd_att_type_change();";
            myScript += "\n\n </script>";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "myKey", myScript, false);
        }
    }
}