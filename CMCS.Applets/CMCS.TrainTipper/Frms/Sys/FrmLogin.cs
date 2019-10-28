using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Metro.ColorTables;
using DevComponents.DotNetBar.Controls;
using CMCS.Common.DAO;
using CMCS.Common.Entities;
using CMCS.Common;
using CMCS.Common.Utilities;
using CMCS.Common.Entities.iEAA;
using CMCS.Common.Enums;
using CMCS.TrainTipper.Core;

namespace CMCS.TrainTipper.Frms.Sys
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
        /// �����ʼ��
        /// </summary>
        private void FormInit()
        {
            // �����û�
            cmbUserAccount.DataSource = commonDao.GetAllSystemUser(eUserRoleCodes.����Ա.ToString());
            cmbUserAccount.DisplayMember = "UserName";
            cmbUserAccount.ValueMember = "UserAccount";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            #region ��֤

            if (cmbUserAccount.SelectedItem == null) return;
            if (string.IsNullOrEmpty(txtUserPassword.Text)) return;

            #endregion

            User user = commonDao.Login(eUserRoleCodes.����Ա.ToString(), cmbUserAccount.SelectedValue.ToString(), MD5Util.Encrypt(txtUserPassword.Text));
            if (user != null)
            {
                SelfVars.LoginUser = user;

                this.Hide();

                SelfVars.MainFrameForm = new Form1();
                SelfVars.MainFrameForm.Show();
            }
            else
            {
                MessageBoxEx.Show("�ʺŻ�����������������룡", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtUserPassword.ResetText();
                txtUserPassword.Focus();
            }
        }
    }
}