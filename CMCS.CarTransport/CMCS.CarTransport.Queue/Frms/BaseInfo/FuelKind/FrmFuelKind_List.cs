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

namespace CMCS.CarTransport.Queue.Frms.BaseInfo.FuelKind
{
    public partial class FrmFuelKind_List : DevComponents.DotNetBar.Metro.MetroForm
    {
        /// <summary>
        /// 窗体唯一标识符
        /// </summary>
        public static string UniqueKey = "FrmFuelKind_List";
        /// <summary>
        /// 选中的实体
        /// </summary>
        public CmcsFuelKind Output;

        public FrmFuelKind_List()
        {
            InitializeComponent();
        }

        private void FrmFuelKind_List_Shown(object sender, EventArgs e)
        {
            advTree1.Nodes.Clear();

            CmcsFuelKind rootEntity = Dbers.GetInstance().SelfDber.Entity<CmcsFuelKind>("where ParentId is null");
            DevComponents.AdvTree.Node rootNode = CreateNode(rootEntity);

            LoadData(rootEntity, rootNode);

            advTree1.Nodes.Add(rootNode);
            addCmcsFuelKind(rootEntity);
            CMCS.CarTransport.Queue.Utilities.Helper.ControlReadOnly(this);
        }

        private void FrmFuelKind_List_KeyUp(object sender, KeyEventArgs e)
        {
        }

        void LoadData(CmcsFuelKind entity, DevComponents.AdvTree.Node node)
        {
            if (entity == null || node == null) return;

            foreach (CmcsFuelKind item in Dbers.GetInstance().SelfDber.Entities<CmcsFuelKind>("where ParentId=:ParentId order by Sequence asc", new { ParentId = entity.Id }))
            {
                DevComponents.AdvTree.Node newNode = CreateNode(item);
                node.Nodes.Add(newNode);
                LoadData(item, newNode);
            }
        }

        DevComponents.AdvTree.Node CreateNode(CmcsFuelKind entity)
        {
            DevComponents.AdvTree.Node node = new DevComponents.AdvTree.Node(entity.FuelName + ((entity.Valid == "有效") ? "" : "(无效)"));
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
            this.Output = (advTree1.SelectedNode.Tag as CmcsFuelKind);
            addCmcsFuelKind(Output);
        }

        void addCmcsFuelKind(CmcsFuelKind item)
        {
            txt_FuelName.Text = item.FuelName;
            txt_ReMark.Text = item.ReMark;
            dbi_Sequence.Text = item.Sequence.ToString();
            chb_IsUse.Checked = (item.Valid == "有效");
        }
    }
}