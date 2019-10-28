using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using CMCS.Common;
using DevComponents.DotNetBar.SuperGrid;
using CMCS.CarTransport.DAO;
using CMCS.Common.DAO;
using CMCS.Common.Entities.CarTransport;
using CMCS.Common.Entities;
using CMCS.Common.Entities.BaseInfo;
using CMCS.CarTransport.Queue.Core;

namespace CMCS.CarTransport.Queue.Frms.BaseInfo.Mine
{
    public partial class FrmMine_List : DevComponents.DotNetBar.Metro.MetroForm
    {
        /// <summary>
        /// ����Ψһ��ʶ��
        /// </summary>
        public static string UniqueKey = "FrmMine_List";
        /// <summary>
        /// ѡ�е�ʵ��
        /// </summary>
        public CmcsMine Output;

        public FrmMine_List()
        {
            InitializeComponent();
        }

        private void FrmMine_List_Shown(object sender, EventArgs e)
        {
            advTree1.Nodes.Clear();

            CmcsMine rootEntity = Dbers.GetInstance().SelfDber.Entity<CmcsMine>("where ParentId is null");
            DevComponents.AdvTree.Node rootNode = CreateNode(rootEntity);

            LoadData(rootEntity, rootNode);

            advTree1.Nodes.Add(rootNode);
            addCmcsMine(rootEntity);
            CMCS.CarTransport.Queue.Utilities.Helper.ControlReadOnly(this);


            //01�鿴 02���� 03�޸� 04ɾ��
            BtnAdd.Visible = QueuerDAO.GetInstance().CheckPower(this.GetType().ToString(), "02", SelfVars.LoginUser);
            BtnUpdate.Visible = QueuerDAO.GetInstance().CheckPower(this.GetType().ToString(), "03", SelfVars.LoginUser);
            BtnDelete.Visible = QueuerDAO.GetInstance().CheckPower(this.GetType().ToString(), "04", SelfVars.LoginUser);
        }

        private void FrmMine_List_KeyUp(object sender, KeyEventArgs e)
        {
        }

        void LoadData(CmcsMine entity, DevComponents.AdvTree.Node node)
        {
            if (entity == null || node == null) return;

            foreach (CmcsMine item in Dbers.GetInstance().SelfDber.Entities<CmcsMine>("where ParentId=:ParentId order by Sequence asc", new { ParentId = entity.Id }))
            {
                DevComponents.AdvTree.Node newNode = CreateNode(item);
                node.Nodes.Add(newNode);
                LoadData(item, newNode);
            }
        }

        DevComponents.AdvTree.Node CreateNode(CmcsMine entity)
        {
            DevComponents.AdvTree.Node node = new DevComponents.AdvTree.Node(entity.Name + ((entity.Valid == "��Ч") ? "" : "(��Ч)"));
            node.Tag = entity;
            node.Expanded = true;
            return node;
        }

        private void advTree1_NodeDoubleClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            //advTree1_NodeClick(sender, e);
        }

        private void advTree1_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            Return();
        }
        void Return()
        {
            if (advTree1.SelectedNode.Parent == null) return;
            this.Output = (advTree1.SelectedNode.Tag as CmcsMine);
            addCmcsMine(Output);
        }

        void addCmcsMine(CmcsMine item)
        {
            txt_Name.Text = item.Name;
            txt_ReMark.Text = item.ReMark;
            dbi_Sequence.Text = item.Sequence.ToString();
            chb_IsUse.Checked = (item.Valid == "��Ч");
        }
    }
}