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
    public partial class AddEditDefaultAttendant : BasePage
    {
        public string pageTitle = "اضافة عضو جديد:";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentUser.Role != Model.UserRole.Admin)
                Response.Redirect(Constants.PageNames.ERROR_PAGE + "?" + Constants.QSKeyNames.ERROR_TYPE + "=" + (int)ErrorType.Unauthorized);
            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(AttendantID))
                {
                    DefaultAttendant defAttObj = DefaultAttendantHelper.GetAttendantById(long.Parse(AttendantID));
                    fillFields(defAttObj);
                    pageTitle = "تعديل بيانات العضو:";
                    this.Title = "المضبطة الإلكترونية - " + pageTitle;
                }
            }
        }

        protected void btnAddEditDefAtt_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = "unknown.jpg";
                if (fuAttAvatar.HasFile)
                {
                    string[] temp = fuAttAvatar.FileName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    filename = temp[0] + "_" + DateTime.Now.ToShortTimeString() + "." + temp[1];
                    filename = filename.Replace("/", "_").Replace(" ", "_").Replace(":", "_");
                }
                fuAttAvatar.SaveAs(System.IO.Path.Combine(Server.MapPath("Images//AttendantAvatars"), filename));

                if (string.IsNullOrEmpty(AttendantID))
                {
                    DefaultAttendant defAttendant = fillAttendant(filename);
                    long attID = DefaultAttendantHelper.AddNewDefaultAttendant(defAttendant);
                    if (attID == -1)
                    {
                        lblInfo1.Text = "هذا العضو تم ادراجه من قبل";
                        lblInfo1.Visible = true;
                        lblInfo2.Text = "هذا العضو تم ادراجه من قبل";
                        lblInfo2.Visible = true;
                    }
                    else
                    {
                        lblInfo1.Text = "تم الحفظ بنجاح";
                        lblInfo1.Visible = true;
                        lblInfo2.Text = "تم الحفظ بنجاح";
                        lblInfo2.Visible = true;
                        clearFields();
                    }
                   
                }
                else
                {
                    if (fuAttAvatar.HasFile)
                    {
                        DefaultAttendantHelper.UpdateDefaultAttendantById(long.Parse(AttendantID), textAttName.Text, textAttShortName.Text, textAttLongName.Text, textAttTitle.Text, filename, textAttJobTitle.Text, int.Parse(ddlAttType.SelectedValue),ddlDegree.SelectedValue);
                    }
                    else
                    {
                        DefaultAttendantHelper.UpdateDefaultAttendantById(long.Parse(AttendantID), textAttName.Text, textAttShortName.Text, textAttLongName.Text, textAttTitle.Text, textAttJobTitle.Text, int.Parse(ddlAttType.SelectedValue), ddlDegree.SelectedValue);
                    }
                    lblInfo1.Text = "تم الحفظ بنجاح";
                    lblInfo1.Visible = true;
                    lblInfo2.Text = "تم الحفظ بنجاح";
                    lblInfo2.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblInfo1.Text = "حدث خطأ";
                lblInfo1.Visible = true;
                lblInfo2.Text = "حدث خطأ";
                lblInfo2.Visible = true;
            }
            pageTitle = "اضافة عضو جديد:";
            if (!string.IsNullOrEmpty(AttendantID))
                pageTitle = "تعديل بيانات العضو:";
            this.Title = "المضبطة الإلكترونية - " + pageTitle;
        }

        public DefaultAttendant fillAttendant(string imgName)
        {
            DefaultAttendant defAttendant = new DefaultAttendant();
            defAttendant.Name = textAttName.Text;
            defAttendant.ShortName = textAttShortName.Text;
            defAttendant.LongName = textAttLongName.Text;
            defAttendant.JobTitle = textAttJobTitle.Text;
            defAttendant.Type = int.Parse(ddlAttType.SelectedValue);
            defAttendant.AttendantTitle =   textAttTitle.Text;
            defAttendant.OrderByAttendantType = int.Parse(ddlAttType.SelectedValue) == (int)Model.AttendantType.President ? 1 : 6;
            defAttendant.AttendantAvatar = imgName;
            defAttendant.Status = 1;
            defAttendant.CreatedAt = DateTime.Now;
            defAttendant.AttendantDegree = ddlDegree.SelectedValue;
            return defAttendant;
        }

        public void fillFields(DefaultAttendant defAttObj)
        {
            textAttName.Text = defAttObj.Name;
            textAttShortName.Text = defAttObj.ShortName;
            textAttLongName.Text = defAttObj.LongName;
            textAttJobTitle.Text = defAttObj.JobTitle;
            ddlAttType.SelectedValue = defAttObj.Type.ToString();
            ddlDegree.SelectedValue = defAttObj.AttendantDegree.ToString();
            textAttTitle.Text = defAttObj.AttendantTitle;
            imgAttendantAvatar.ImageUrl = String.Format("/images/AttendantAvatars/{0}", defAttObj.AttendantAvatar);
        }

        public void clearFields()
        {
            textAttName.Text = "";
            textAttShortName.Text = "";
            textAttLongName.Text = "";
            textAttJobTitle.Text = "";
            ddlAttType.SelectedValue = "1";
            ddlDegree.SelectedValue = "";
            textAttTitle.Text = "السيد";
        }
    }
}