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
    public partial class ManageDeletedDefaultAttendance : BasePage
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
            List<DefaultAttendant> DefaultAttendantLst = DefaultAttendantHelper.GetDeletedDefaultAttendants();
            if (DefaultAttendantLst.Count == 0)
            {
               /* DefaultAttendantLst = new List<DefaultAttendant>();
                DefaultAttendant DefaultAttendantObj = new DefaultAttendant();
                DefaultAttendantObj.Type = 3;
                DefaultAttendantLst.Add(DefaultAttendantObj);
                gvDefaultAttendants.DataSource = DefaultAttendantLst;
                gvDefaultAttendants.DataBind();
                int columncount = gvDefaultAttendants.Rows[0].Cells.Count;
                gvDefaultAttendants.Rows[0].Cells.Clear();
                gvDefaultAttendants.Rows[0].Cells.Add(new TableCell());
                gvDefaultAttendants.Rows[0].Cells[0].ColumnSpan = columncount;
                gvDefaultAttendants.Rows[0].Cells[0].Text = "لا يوجد أعضاء";*/
            }
            else
            {
                gvDefaultAttendants.DataSource = DefaultAttendantLst;
                gvDefaultAttendants.DataBind();
            }
        }

        protected void gvDefaultAttendants_RowUnDeleting(object sender, EventArgs e)
        {
            LinkButton lnkRemove = (LinkButton)sender;
            long defaultAttID = long.Parse(lnkRemove.CommandArgument);//Convert.ToInt64(gvDefaultAttendants.DataKeys[e.RowIndex].Value.ToString());
            DefaultAttendantHelper.UpdateDefaultAttendantStatusById(defaultAttID, (int)AttendantStatus.Active);
            gvbind();

        }

        protected void gvDefaultAttendants_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDefaultAttendants.PageIndex = e.NewPageIndex;
            gvbind();
        }
    }
}