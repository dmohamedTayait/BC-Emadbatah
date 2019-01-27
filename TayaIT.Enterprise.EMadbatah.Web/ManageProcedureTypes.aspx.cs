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
    public partial class ManageProcedureTypes : BasePage
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
            List<ProcedureType> ProcedureLst = ProcedureHelper.GetProcedureTypes();
            gvProcedures.DataSource = ProcedureLst;
            gvProcedures.DataBind();
        }

        protected void gvProcedures_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvProcedures.EditIndex = e.NewEditIndex;
            gvbind();
        }

        protected void gvProcedures_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            long procedureTypeID = Convert.ToInt64(gvProcedures.DataKeys[e.RowIndex].Value.ToString());
            GridViewRow row = (GridViewRow)gvProcedures.Rows[e.RowIndex];

            string textProcedureTitle = ((TextBox)gvProcedures.Rows[e.RowIndex].FindControl("textProcedureTitle")).Text;
   
            gvProcedures.EditIndex = -1;
            ProcedureHelper.UpdateProcedureType(procedureTypeID, textProcedureTitle);
            gvbind();
           
        }

        protected void gvProcedures_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvProcedures.EditIndex = -1;
            gvbind();
        }

        protected void gvProcedures_RowDeleting(object sender, EventArgs e)
        {
            LinkButton lnkRemove = (LinkButton)sender;
            long procedureTypeID = long.Parse(lnkRemove.CommandArgument);//Convert.ToInt64(gvProcedures.DataKeys[e.RowIndex].Value.ToString());
            ProcedureHelper.DeleteProcedureTypeById(procedureTypeID);
            gvbind();

        }

        protected void AddNewProcedure(object sender, EventArgs e)
        {
            string textProcedureTitle = ((TextBox)gvProcedures.FooterRow.FindControl("textProcedureTitle")).Text;

            //Add new procedure
            ProcedureType procTypeObj = new ProcedureType();
            procTypeObj.ProcedureTypeStr = textProcedureTitle;
            procTypeObj.ProcedureTypeOrder = 10;
            ProcedureHelper.AddNewProcedureType(procTypeObj);

            gvbind();
        }

    }
}