using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CMCS.Common;
using CMCS.Common.DAO;
using CMCS.Common.Entities;
using CMCS.Common.Entities.iEAA;
using CMCS.Common.Utilities;
using CMCS.WeighCheck.MakeCheck.Utilities;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using DevComponents.DotNetBar.Metro.ColorTables;
using CMCS.Common.Enums;

namespace CMCS.WeighCheck.MakeCheck.Frms.Sys
{
    public partial class FrmLogin : DevComponents.DotNetBar.Metro.MetroForm
    {
        public FrmLogin()
        {
            InitializeComponent();

            //StyleManager.MetroColorGeneratorParameters = MetroColorGeneratorParameters.BlackSky;
        }

        CommonDAO commonDao = CommonDAO.GetInstance();

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            FormInit();
        }

        /// <summary>
        /// 窗体初始化
        /// </summary>
        private void FormInit()
        {
            // 加载用户
            cmbUserAccount.DataSource = commonDao.GetAllSystemUser(eUserRoleCodes.化验员.ToString());
            cmbUserAccount.DisplayMember = "UserName";
            cmbUserAccount.ValueMember = "UserAccount";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            #region 验证

            if (cmbUserAccount.SelectedItem == null) return;
            if (string.IsNullOrEmpty(txtUserPassword.Text)) return;

            #endregion

            User user = commonDao.Login(eUserRoleCodes.化验员.ToString(), cmbUserAccount.SelectedValue.ToString(), MD5Util.Encrypt(txtUserPassword.Text));
            if (user != null)
            {
                SelfVars.LoginUser = user;

                this.Hide();

                SelfVars.MainFrameForm = new FrmMainFrame();
                SelfVars.MainFrameForm.Show();
            }
            else
            {
                MessageBoxEx.Show("帐号或密码错误，请重新输入！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtUserPassword.ResetText();
                txtUserPassword.Focus();
            }
        }
    }
}