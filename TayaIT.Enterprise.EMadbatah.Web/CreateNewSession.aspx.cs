using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TayaIT.Enterprise.EMadbatah.DAL;
using TayaIT.Enterprise.EMadbatah.Model;
using TayaIT.Enterprise.EMadbatah.Model.VecSys;
using TayaIT.Enterprise.EMadbatah.Vecsys;
using TayaIT.Enterprise.EMadbatah.BLL;
using System.Collections;
using TayaIT.Enterprise.EMadbatah.Util.Web;
using TayaIT.Enterprise.EMadbatah.Config;
using System.Text;
namespace TayaIT.Enterprise.EMadbatah.Web
{
    public partial class CreateNewSession : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentUser.Role != Model.UserRole.Admin)
                Response.Redirect(Constants.PageNames.ERROR_PAGE + "?" + Constants.QSKeyNames.ERROR_TYPE + "=" + (int)ErrorType.Unauthorized);
            if (!Page.IsPostBack)
            {
                EMadbatahEntities context = new EMadbatahEntities();
                List<DefaultAttendant> DefaultAttendants = context.DefaultAttendants.Select(aa => aa).OrderBy(x => x.OrderByAttendantType).Where(cc => cc.Type != (int)Model.AttendantType.UnAssigned && cc.Type != (int)Model.AttendantType.CountryPresidentFamily && cc.Status == (int)Model.AttendantStatus.Active).ToList();
                ddlPresident.DataSource = DefaultAttendants;
                ddlPresident.DataTextField = "LongName";
                ddlPresident.DataValueField = "ID";
                ddlPresident.DataBind();

                if (!string.IsNullOrEmpty(SessionID))
                {
                    Session sessionObj = SessionHelper.GetSessionByID(long.Parse(SessionID));

                    txtDate.Text = sessionObj.StartTime.Value.Date.ToShortDateString();
                    txtTime.Text = sessionObj.StartTime.Value.ToString("HH:mm");

                    txtSubject.Text = sessionObj.Subject.ToString();
                    txtEParliamentID.Text = sessionObj.EParliamentID.ToString();
     
                    txtSeason.Text = sessionObj.Season.ToString();
                    txtStage.Text = sessionObj.Stage.ToString();
                    ddlPresident.SelectedValue = sessionObj.PresidentID.ToString();

                    if (sessionObj.SessionStartFlag == (int)SessionOpenStatus.OnTime)
                        CBSessionStart.Checked = true;
                    else CBSessionStart.Checked = false;

                    if (SessionFileHelper.GetUnNewSessionFilesCount(long.Parse(SessionID)) > 0)
                        CBSessionStart.Enabled = false;
                    else
                        CBSessionStart.Enabled = true;
                    this.Title = "المضبطة الإلكترونية - تعديل بيانات المضبطة";
                    divPageTitle.InnerHtml = "تعديل بيانات المضبطة";
                }
            }
        }

        protected void btnCreateNewSession_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SessionID))
            {
                Session sessionObj = fillValues();
                SessionHelper.UpdateSessionInfo(long.Parse(SessionID), sessionObj);
                Session current_session = EditorFacade.GetSessionByID(long.Parse(SessionID));
                if (SessionFileHelper.GetUnNewSessionFilesCount(long.Parse(SessionID)) == 0)
                    AttendantHelper.GenerateSessionAttendants(long.Parse(SessionID), current_session, true);
                
            }
            else
            {
                Session sessionObj = fillValues();
                long SessionIDCreated = SessionHelper.CreateNewSession(sessionObj);

                if (SessionIDCreated != -1)
                    AttendantHelper.GenerateSessionAttendants(SessionIDCreated, sessionObj, false);
            }
            Response.Redirect("Default.aspx");
        }

        public Session fillValues()
        {
            DateTime plannedStartDate = Convert.ToDateTime(txtDate.Text + " " + txtTime.Text);
            string president = ddlPresident.SelectedItem.Text;
            string place = "الكويت";
            int EParliamentID = int.Parse(txtEParliamentID.Text);
            string Season = txtSeason.Text;
            string Stage = txtStage.Text;
            string StageType = txtStageType.Text;
            string Type = txtType.Text;
            Int32 PresidentID = Int32.Parse(ddlPresident.SelectedValue);
            string subject = txtSubject.Text;
            int SessionStartFlag = CBSessionStart.Checked ? (int)SessionOpenStatus.OnTime : (int)SessionOpenStatus.NotOnTime;

            Session sessionObj = new DAL.Session();
            sessionObj.Date = DateTime.Now;
            sessionObj.StartTime = plannedStartDate;
            sessionObj.President = president;
            sessionObj.Place = place;
            sessionObj.EParliamentID = EParliamentID;

            sessionObj.Season = Season;
            sessionObj.Type = Type;
            sessionObj.Stage = Stage;
            sessionObj.StageType = StageType;
            sessionObj.Serial = EParliamentID;
            sessionObj.SessionStatusID = (int)Model.SessionStatus.New;

            sessionObj.Subject = subject;
            sessionObj.ReviewerID = CurrentUser.ID;
            sessionObj.SessionStartFlag = SessionStartFlag;
            sessionObj.PresidentID = PresidentID;
            return sessionObj;
        }

        public string ConverToToTwoDigits(string num)
        {
            if (num.Length == 1)
                return "0" + num;
            else return num;
        }
    }
}