using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
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
using CMCS.CarTransport.Queue.Core;
using CMCS.Common.Entities.iEAA;

namespace CMCS.CarTransport.Queue.Frms.Sys
{
    public partial class FrmPassword : DevComponents.DotNetBar.Metro.MetroForm
    {
        public FrmPassword()
        {
            InitializeComponent();
        }

        CommonDAO commonDao = CommonDAO.GetInstance();

        private void FrmPassword_Load(object sender, EventArgs e)
        {
            FormInit();
        }

        /// <summary>
        /// �����ʼ��
        /// </summary>
        private void FormInit()
        {

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            #region ��֤

            if (string.IsNullOrWhiteSpace(txtUserPassword.Text.Trim()))
            {
                MessageBoxEx.Show("����������룡", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtUserNewPassword.Text.Trim()))
            {
                MessageBoxEx.Show("�����������룡", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtUserNewPasswordSecond.Text.Trim()))
            {
                MessageBoxEx.Show("������ȷ�����룡", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtUserNewPassword.Text.Trim() != txtUserNewPasswordSecond.Text.Trim())
            {
                MessageBoxEx.Show("���������벻һ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            #endregion

            User user = Dbers.GetInstance().SelfDber.Entities<User>(" where UserAccount=:UserAccount and MDPassword=:MDPassword", new { UserAccount = SelfVars.LoginUser.UserAccount, MDPassword = MD5Util.Encrypt(txtUserPassword.Text.Trim()) }).FirstOrDefault();
            if (user != null)
            {
                Dbers.GetInstance().SelfDber.Execute("update " + DapperDber.Util.EntityReflectionUtil.GetTableName<User>() + " set MDPassword=:MDPassword where PartyId=:PartyId", new { MDPassword = MD5Util.Encrypt(txtUserNewPassword.Text.Trim()), PartyId = user.PartyId });

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBoxEx.Show("�ʺŻ�����������������룡", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtUserPassword.ResetText();
                txtUserPassword.Focus();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}