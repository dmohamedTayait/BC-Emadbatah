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
    public partial class ManageDefaultAttendance : BasePage
    {
        public string procedureTitle = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentUser.Role != Model.UserRole.Admin)
                Response.Redirect(Constants.PageNames.ERROR_PAGE + "?" + Constants.QSKeyNames.ERROR_TYPE + "=" + (int)ErrorType.Unauthorized);
            if (!Page.IsPostBack)
            {
                gvbind();
            }
        }

        protected void gvbind()
        {

            EMadbatahEntities context = new EMadbatahEntities();
            List<DefaultAttendant> DefaultAttendantLst = DefaultAttendantHelper.GetAllDefaultAttendants(true);
            if (DefaultAttendantLst.Count == 0)
            {
                DefaultAttendantLst = new List<DefaultAttendant>();
                DefaultAttendant DefaultAttendantObj = new DefaultAttendant();

                DefaultAttendantLst.Add(DefaultAttendantObj);
                gvDefaultAttendants.DataSource = DefaultAttendantLst;
                gvDefaultAttendants.DataBind();
                int columncount = gvDefaultAttendants.Rows[0].Cells.Count;
                gvDefaultAttendants.Rows[0].Cells.Clear();
                gvDefaultAttendants.Rows[0].Cells.Add(new TableCell());
                gvDefaultAttendants.Rows[0].Cells[0].ColumnSpan = columncount;
                gvDefaultAttendants.Rows[0].Cells[0].Text = "لا يوجد أعضاء";
            }
            else
            {
                gvDefaultAttendants.DataSource = DefaultAttendantLst;
                gvDefaultAttendants.DataBind();
            }
        }

        protected void gvDefaultAttendants_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvDefaultAttendants.EditIndex = e.NewEditIndex;
            gvbind();
        }

        protected void gvDefaultAttendants_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvDefaultAttendants.EditIndex = -1;
            gvbind();
        }

        protected void gvDefaultAttendants_RowDeleting(object sender, EventArgs e)
        {
            LinkButton lnkRemove = (LinkButton)sender;
            long defaultAttID = long.Parse(lnkRemove.CommandArgument);//Convert.ToInt64(gvDefaultAttendants.DataKeys[e.RowIndex].Value.ToString());
            DefaultAttendantHelper.UpdateDefaultAttendantStatusById(defaultAttID, (int)AttendantStatus.Deleted);
            gvbind();

        }

        protected void gvDefaultAttendants_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                TextBox txtName = (TextBox)e.Row.FindControl("textAttName");
                txtName.TextMode = TextBoxMode.MultiLine;

                FileUpload fileUploadAttendantAvatar = (FileUpload)e.Row.FindControl("fileUploadAttendantAvatar");
                ScriptManager.GetCurrent(this).RegisterPostBackControl(fileUploadAttendantAvatar);
            }
        }

        protected void gvDefaultAttendants_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

            FileUpload fileUpload = gvDefaultAttendants.Rows[e.RowIndex].FindControl("fileUploadAttendantAvatar") as FileUpload;
            fileUpload.SaveAs(System.IO.Path.Combine(Server.MapPath("Images//AttendantAvatars"), fileUpload.FileName));
            gvDefaultAttendants.EditIndex = -1;
            gvbind();


        }


        protected void AddNewDefaultAttendant(object sender, EventArgs e)
        {
            string textAttName = ((TextBox)gvDefaultAttendants.FooterRow.FindControl("textAttName")).Text;
            string textAttShortName = ((TextBox)gvDefaultAttendants.FooterRow.FindControl("textAttShortName")).Text;
            string textAttLongName = ((TextBox)gvDefaultAttendants.FooterRow.FindControl("textAttLongName")).Text;
            string textAttTitle = ((TextBox)gvDefaultAttendants.FooterRow.FindControl("textAttTitle")).Text;
            string textAttJobTitle = ((TextBox)gvDefaultAttendants.FooterRow.FindControl("textAttJobTitle")).Text;
            string textAttAvatar = "unknown.jpg";// ((TextBox)gvDefaultAttendants.FooterRow.FindControl("textProcedireTitle")).Text;
            int textAttType = (int)Model.AttendantType.FromOutsideTheCouncil;//((TextBox)gvDefaultAttendants.FooterRow.FindControl("textProcedireTitle")).Text;
            //  string textProcedureTitle = ((TextBox)gvDefaultAttendants.FooterRow.FindControl("textProcedireTitle")).Text;

            //Add new Attendant
            DefaultAttendant defAttobj = new DefaultAttendant();
            defAttobj.AttendantAvatar = textAttAvatar;
            defAttobj.AttendantTitle = textAttTitle;
            defAttobj.JobTitle = textAttJobTitle;
            defAttobj.LongName = textAttLongName;
            defAttobj.Name = textAttName;
            defAttobj.ShortName = textAttLongName;// textAttShortName;
            defAttobj.Status = 1;
            defAttobj.Type = textAttType;
            defAttobj.CreatedAt = DateTime.Now;
            defAttobj.AttendantDegree = "";
            DefaultAttendantHelper.AddNewDefaultAttendant(defAttobj);

            gvbind();
        }

        protected void gvDefaultAttendants_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDefaultAttendants.PageIndex = e.NewPageIndex;
            gvbind();
        }
    }
}