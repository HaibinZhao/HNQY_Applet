using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CMCS.CarTransport.Queue.Core;
using CMCS.Common;
using CMCS.Common.Entities.BaseInfo;
using CMCS.Common.Entities.CarTransport;
using CMCS.Common.Entities.iEAA;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Metro;
using CMCS.CarTransport.DAO;

namespace CMCS.CarTransport.Queue.Frms.SysManage
{
    public partial class Frm_Module_Oper : MetroForm
    {
        SysModule CurrSysModule = null;
        QueuerDAO queuerDAO = QueuerDAO.GetInstance();

        public Frm_Module_Oper(SysModule sysmodule)
        {
            InitializeComponent();

            this.CurrSysModule = sysmodule;
        }

        private void Frm_Module_Oper_Load(object sender, EventArgs e)
        {
            if (this.CurrSysModule != null)
            {
                this.Text = "模块管理 - 详情";
                btnSubmit.Text = "修改";

                txtModuleName.Text = this.CurrSysModule.ModuleName;
                txtModuleNo.Text = this.CurrSysModule.Moduleno;
                txtModuleDll.Text = this.CurrSysModule.ModuleDll;
                chbStopUse.Checked = Convert.ToBoolean(this.CurrSysModule.StopUse);
                txtCreateDate.Text = this.CurrSysModule.CreateDate.ToString("yyyy-MM-dd HH:mm");
            }
            else
            {
                this.Text = "模块管理 - 新增";
                btnSubmit.Text = "新增";

                txtCreateDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                txtModuleNo.Text = queuerDAO.CreateModuleno();
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtModuleDll.Text))
            {
                MessageBoxEx.Show("请输入模块完整名称！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtModuleName.Text))
            {
                MessageBoxEx.Show("请输入模块名称！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                SysModule entityCheck = Dbers.GetInstance().SelfDber.Entity<SysModule>("where ModuleDll='" + txtModuleDll.Text.Trim() + "'");
                if ((this.CurrSysModule != null && entityCheck != null && this.CurrSysModule.Id != entityCheck.Id) || (this.CurrSysModule == null && entityCheck != null))
                {
                    MessageBoxEx.Show("已经存在该模块完整名称！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (this.CurrSysModule == null)
            {
                // 新增
                SysModule entity = new SysModule();
                entity.ModuleName = txtModuleName.Text.Trim();
                entity.Moduleno = txtModuleNo.Text.Trim();
                entity.ModuleDll = txtModuleDll.Text.Trim();
                entity.StopUse = Convert.ToInt16(chbStopUse.Checked);

                Dbers.GetInstance().SelfDber.Insert<SysModule>(entity);

                //增加增删改查功能
                if (chkInsert.Checked)
                {
                    List<SysResource> listResource = queuerDAO.GetResources(entity, true);
                    foreach (SysResource item in listResource)
                    {
                        Dbers.GetInstance().SelfDber.Insert<SysResource>(item);
                    }
                }

                MessageBoxEx.Show("新增成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtModuleName.Text = string.Empty;
                txtModuleDll.Text = string.Empty;
                txtModuleNo.Text = queuerDAO.CreateModuleno();
            }
            else
            {
                // 修改
                this.CurrSysModule.ModuleName = txtModuleName.Text.Trim();
                this.CurrSysModule.ModuleDll = txtModuleDll.Text.Trim();
                this.CurrSysModule.StopUse = Convert.ToInt16(chbStopUse.Checked);
                this.CurrSysModule.UpdateEntityBase(this.CurrSysModule, SelfVars.LoginUser.UserAccount);

                Dbers.GetInstance().SelfDber.Update<SysModule>(this.CurrSysModule);

                MessageBoxEx.Show("修改成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnCancel_Click(null, null);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
