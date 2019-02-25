using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
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
                btnSave.Visible = true;
            }
            else
            {
                fill_gv_outside_attendants(SessionID);
                btnSave.Visible = false;
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
                HtmlGenericControl divAbsenceExcuse = (HtmlGenericControl)e.Row.FindControl("divAbsenceExcuse");

                DropDownList ddlAbsenseExcuse = (DropDownList)e.Row.FindControl("ddlAbsenseExcuse");
                ddlAbsenseExcuse.Items.Add(new ListItem("اخرى", "0"));

                TextBox txtAbsenceExcuse = (TextBox)e.Row.FindControl("txtAbsenceExcuse");
                if (attendant.Count != 0)
                {
                    //show-hide txtAbsenceExcuse
                    if (rb.Items.FindByValue(attendant[0].State.ToString()) != null)
                    {
                        rb.Items.FindByValue(attendant[0].State.ToString()).Selected = true;
                    }
                    if (attendant[0].State == (int)Model.AttendantState.Apology)
                    {
                        divAbsenceExcuse.Style.Add("display", "block");
                        if (ddlAbsenseExcuse.Items.Contains(new ListItem(attendant.First().AbsenseExcuse)))
                        {
                            ddlAbsenseExcuse.SelectedValue = attendant.First().AbsenseExcuse;
                            txtAbsenceExcuse.Text = "";
                            txtAbsenceExcuse.Style.Add("display", "none");
                        }
                        else
                        {
                            txtAbsenceExcuse.Text = attendant.First().AbsenseExcuse;
                            txtAbsenceExcuse.Style.Add("display", "block");
                            ddlAbsenseExcuse.SelectedValue = "0";
                        }
              
                    }
                    else
                    {
                        txtAbsenceExcuse.Text = "";
                        ddlAbsenseExcuse.SelectedIndex = 0;
                        //txtAbsenceExcuse.Style.Add("display", "none");
                        divAbsenceExcuse.Style.Add("display", "none");
                    }
                    //rdio btn list: show - hide the third option
                    if (ddlAttendantTypes.SelectedValue == ((int)Model.AttendantType.GovernmentRepresentative).ToString())
                    {
                        rb.Items[rb.Items.Count - 1].Attributes.Add("hidden", "hidden");
                    }
                    else
                    {
                        rb.Items[rb.Items.Count - 1].Attributes.Remove("hidden");
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
            int SessionID = int.Parse(ddlSessions.SelectedValue);
            foreach (GridViewRow item in GVAttendants.Rows)
            {
                HiddenField HFID = item.Cells[0].FindControl("HFID") as HiddenField;

                int DefaultAttendantId = int.Parse(HFID.Value);

                RadioButtonList rdlist = item.Cells[2].FindControl("RBLAttendantStates") as RadioButtonList;
                TextBox txtAbsenceExcuse = item.Cells[3].FindControl("txtAbsenceExcuse") as TextBox;
                HtmlGenericControl divAbsenceExcuse = item.Cells[3].FindControl("divAbsenceExcuse") as HtmlGenericControl;

                int AttendantStatus = 0;
                if (rdlist.SelectedItem != null)
                {
                    AttendantStatus = int.Parse(rdlist.SelectedValue);
                }

                if (AttendantStatus != 0)
                {
                    string absenceExcuse = "";
                    divAbsenceExcuse.Style.Add("display", "none");
                    if (AttendantStatus == (int)Model.AttendantState.Apology)
                    {
                        DropDownList ddlAbsenseExcuse = item.Cells[3].FindControl("ddlAbsenseExcuse") as DropDownList;
                      
                        if (ddlAbsenseExcuse.SelectedValue == "0")
                        {
                            absenceExcuse = txtAbsenceExcuse.Text;
                            txtAbsenceExcuse.Style.Add("display", "block");
                        }
                        else
                        {
                            absenceExcuse = ddlAbsenseExcuse.SelectedValue;
                            txtAbsenceExcuse.Text = "";
                            txtAbsenceExcuse.Style.Add("display", "none");
                        }
          
                        divAbsenceExcuse.Style.Add("display", "block");
                    }
                    AttendantHelper.UpdateAttendantState(AttendantStatus, DefaultAttendantId, absenceExcuse);
                }

            }
            lblInfo1.Text = "تم الحفظ بنجاح";
            lblInfo1.Visible = true;
            lblInfo2.Text = "تم الحفظ بنجاح";
            lblInfo2.Visible = true;
            //fill_gv_attendants(SessionID);
        }

        protected void GVOutsideAttendants_RowEditing(object sender, GridViewEditEventArgs e)
        {
            long SessionID = long.Parse(ddlSessions.SelectedValue);
            GVOutsideAttendants.EditIndex = e.NewEditIndex;
            fill_gv_outside_attendants(SessionID);
        }

        protected void GVOutsideAttendants_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            long SessionID = long.Parse(ddlSessions.SelectedValue);

            GridViewRow row = (GridViewRow)GVOutsideAttendants.Rows[e.RowIndex];
            long lblAttId = long.Parse(((Label)GVOutsideAttendants.Rows[e.RowIndex].FindControl("lblAttId")).Text);
            string textAttLongName = ((TextBox)GVOutsideAttendants.Rows[e.RowIndex].FindControl("textAttLongName")).Text;
            string textAttName = ((TextBox)GVOutsideAttendants.Rows[e.RowIndex].FindControl("textAttName")).Text;
            string textAttTitle = ((TextBox)GVOutsideAttendants.Rows[e.RowIndex].FindControl("textAttTitle")).Text;
            string textAttJobTitle = ((TextBox)GVOutsideAttendants.Rows[e.RowIndex].FindControl("textAttJobTitle")).Text;
            string ddlAttType = ((DropDownList)GVOutsideAttendants.Rows[e.RowIndex].FindControl("ddlAttType")).SelectedValue;
            string ddlDegree = ((DropDownList)GVOutsideAttendants.Rows[e.RowIndex].FindControl("ddlDegree")).SelectedValue;

            AttendantHelper.UpdateAttendantById(lblAttId, textAttName, textAttLongName, textAttLongName, textAttTitle, "unknown.jpg", textAttJobTitle,  int.Parse(ddlAttType), ddlDegree);

            GVOutsideAttendants.EditIndex = -1;
            fill_gv_outside_attendants(SessionID);
        }

        protected void GVOutsideAttendants_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            long SessionID = long.Parse(ddlSessions.SelectedValue);
            GVOutsideAttendants.EditIndex = -1;
            fill_gv_outside_attendants(SessionID);
        }

        protected void GVOutsideAttendants_RowDeleting(object sender, EventArgs e)
        {
            LinkButton lnkRemove = (LinkButton)sender;
            long attID = long.Parse(lnkRemove.CommandArgument);
            AttendantHelper.DeleteAttendantByID(attID);
            long SessionID = long.Parse(ddlSessions.SelectedValue);
            fill_gv_outside_attendants(SessionID);
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
            if (int.Parse(ddlAttendantTypes.SelectedValue) == (int)Model.AttendantType.GovernmentRepresentative)
            {
                GVAttendants.Columns[2].HeaderText = "<span class='space-st1' style='color:blue;width:90px'>حاضر</span><span class='space-st1' style='color:red;width:150px'>غائب</span>";
                GVAttendants.Columns[4].Visible = true;
            }
            else
            {
                GVAttendants.Columns[2].HeaderText = "<span class='space-st1' style='color:blue'>حاضر</span><span class='space-st1' style='color:red'>غائب</span><span class='space-st1' style='color:green'>غائب بعذر</span>";
                GVAttendants.Columns[4].Visible = false;
            }
            List<Attendant> attendantsLst = AttendantHelper.GetAttendantInSession(sessionID, int.Parse(ddlAttendantTypes.SelectedValue));
            GVAttendants.DataSource = attendantsLst;
            GVAttendants.DataBind();
            GVAttendants.Style.Add("display", "block");
            GVOutsideAttendants.Style.Add("display", "none");
        }

        private void fill_gv_outside_attendants(long sessionID)
        {
            List<Attendant> attendantsLst = AttendantHelper.GetAttendantInSession(sessionID, (int)Model.AttendantType.FromOutsideTheCouncil);
            GVOutsideAttendants.DataSource = attendantsLst;
            GVOutsideAttendants.DataBind();
            GVOutsideAttendants.Style.Add("display", "block");
            GVAttendants.Style.Add("display", "none");
        }

        private void bind_script()
        {
            string myScript = "\n<script type=\"text/javascript\" language=\"Javascript\" id=\"EventScriptBlock\">\n";
            myScript += "rd_att_type_change();";
            myScript += "\n\n </script>";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "myKey", myScript, false);
        }

        protected void AddNewOutSideAttendat(object sender, EventArgs e)
        {
            long SessionID = long.Parse(ddlSessions.SelectedValue);
            string textAttLongName = ((TextBox)GVOutsideAttendants.FooterRow.FindControl("textAttLongName")).Text;
            string textAttName = ((TextBox)GVOutsideAttendants.FooterRow.FindControl("textAttName")).Text;
            string textAttTitle = ((TextBox)GVOutsideAttendants.FooterRow.FindControl("textAttTitle")).Text;
            string textAttJobTitle = ((TextBox)GVOutsideAttendants.FooterRow.FindControl("textAttJobTitle")).Text;
            string ddlAttType = ((DropDownList)GVOutsideAttendants.FooterRow.FindControl("ddlAttType")).SelectedValue;
            string ddlDegree = ((DropDownList)GVOutsideAttendants.FooterRow.FindControl("ddlDegree")).SelectedValue;

            //Add new Season
            Attendant attObj = new Attendant();
            attObj.LongName = textAttLongName;
            attObj.ShortName = textAttLongName;
            attObj.Name = textAttName;
            attObj.AttendantTitle = textAttTitle;
            attObj.JobTitle = textAttJobTitle;
            attObj.Type = int.Parse(ddlAttType);
            attObj.AttendantDegree = ddlDegree;
            attObj.SessionID = SessionID;
            attObj.State = (int)Model.AttendantState.Attended;
            attObj.OrderByAttendantType = 6;
            attObj.AttendantAvatar = "unknown.jpg";

            AttendantHelper.AddNewSessionAttendant(attObj);
           
            fill_gv_outside_attendants(SessionID);
        }
    }
}