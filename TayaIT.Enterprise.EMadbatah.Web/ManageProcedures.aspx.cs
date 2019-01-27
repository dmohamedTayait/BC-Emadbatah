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
    public partial class ManageProcedures : BasePage
    {
        public string procedureTitle = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentUser.Role != Model.UserRole.Admin)
                Response.Redirect(Constants.PageNames.ERROR_PAGE + "?" + Constants.QSKeyNames.ERROR_TYPE + "=" + (int)ErrorType.Unauthorized);
            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(ProcedureTypeID))
                {
                    gvbind(long.Parse(ProcedureTypeID));
                }
            }
        }

        protected void gvbind(long procedureTypeID)
        {
            procedureTitle = ProcedureHelper.GetProcedureTypeByID(int.Parse(ProcedureTypeID));
            EMadbatahEntities context = new EMadbatahEntities();
            List<Procedure> ProcedureLst = ProcedureHelper.GetProcedures(Convert.ToInt32(procedureTypeID));//context.Procedures.Select(aa => aa).Where(cc => cc.ProcedureTypeID == procedureTypeID).ToList();
            if (ProcedureLst.Count == 0)
            {
                ProcedureLst = new List<Procedure>();
                Procedure procObj = new Procedure();
                procObj.ProcedureTitle = "";
                procObj.ProcedureTypeID = 0;
                ProcedureLst.Add(procObj);
                gvProcedures.DataSource = ProcedureLst;
                gvProcedures.DataBind();
                int columncount = gvProcedures.Rows[0].Cells.Count;
                gvProcedures.Rows[0].Cells.Clear();
                gvProcedures.Rows[0].Cells.Add(new TableCell());
                gvProcedures.Rows[0].Cells[0].ColumnSpan = columncount;
                gvProcedures.Rows[0].Cells[0].Text = "لا يوجد اجراءات تحت هذه القائمة";
            }
            else
            {
                gvProcedures.DataSource = ProcedureLst;
                gvProcedures.DataBind();
            }
        }

        protected void gvProcedures_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvProcedures.EditIndex = e.NewEditIndex;
            gvbind(long.Parse(ProcedureTypeID));
            string textProcedureTitle = ((TextBox)gvProcedures.Rows[e.NewEditIndex].FindControl("textProcedireTitle")).Text;
            textProcedureTitle = textProcedureTitle.Replace("</br>", "\n").Replace("&nbsp;", " ");
            ((TextBox)gvProcedures.Rows[e.NewEditIndex].FindControl("textProcedireTitle")).Text = textProcedureTitle;
        }

        protected void gvProcedures_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            long procedureID = Convert.ToInt64(gvProcedures.DataKeys[e.RowIndex].Value.ToString());
            GridViewRow row = (GridViewRow)gvProcedures.Rows[e.RowIndex];

            string textProcedureTitle = ((TextBox)gvProcedures.Rows[e.RowIndex].FindControl("textProcedireTitle")).Text;
   
            gvProcedures.EditIndex = -1;
            ProcedureHelper.UpdateProcedure(procedureID, textProcedureTitle.Replace("\n", "</br>").Replace(" ","&nbsp;" ));
            gvbind(long.Parse(ProcedureTypeID));
           
        }

        protected void gvProcedures_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvProcedures.EditIndex = -1;
            gvbind(long.Parse(ProcedureTypeID));
        }

        protected void gvProcedures_RowDeleting(object sender, EventArgs e)
        {
            LinkButton lnkRemove = (LinkButton)sender;
            long procedureID = long.Parse(lnkRemove.CommandArgument);//Convert.ToInt64(gvProcedures.DataKeys[e.RowIndex].Value.ToString());
            ProcedureHelper.DeleteProcedureById(procedureID);
            gvbind(long.Parse(ProcedureTypeID));

        }

        protected void AddNewProcedure(object sender, EventArgs e)
        {
            string textProcedureTitle = ((TextBox)gvProcedures.FooterRow.FindControl("textProcedireTitle")).Text;

            //Add new procedure
            Procedure procObj = new Procedure();
            procObj.ProcedureTitle = textProcedureTitle.Replace("\n", "</br>").Replace(" ","&nbsp;" );
            procObj.ProcedureTypeID = long.Parse(ProcedureTypeID);
            ProcedureHelper.AddNewProcedure(procObj);

            gvbind(long.Parse(ProcedureTypeID));
        }

    }
}