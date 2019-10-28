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

namespace CMCS.CarTransport.Queue.Frms.BaseInfo.GoodsType
{
    public partial class FrmGoodsType_List : DevComponents.DotNetBar.Metro.MetroForm
    {
        /// <summary>
        /// 窗体唯一标识符
        /// </summary>
        public static string UniqueKey = "FrmGoodsType_List";
        /// <summary>
        /// 选中的实体
        /// </summary>
        public CmcsGoodsType Output;

        public FrmGoodsType_List()
        {
            InitializeComponent();
        }

        private void FrmGoodsType_List_Shown(object sender, EventArgs e)
        {
            advTree1.Nodes.Clear();

            CmcsGoodsType rootEntity = Dbers.GetInstance().SelfDber.Entity<CmcsGoodsType>("where ParentId is null");
            DevComponents.AdvTree.Node rootNode = CreateNode(rootEntity);

            LoadData(rootEntity, rootNode);

            advTree1.Nodes.Add(rootNode);
            addCmcsGoodsType(rootEntity);
            CMCS.CarTransport.Queue.Utilities.Helper.ControlReadOnly(this);
        }

        private void FrmGoodsType_List_KeyUp(object sender, KeyEventArgs e)
        {
        }

        void LoadData(CmcsGoodsType entity, DevComponents.AdvTree.Node node)
        {
            if (entity == null || node == null) return;

            foreach (CmcsGoodsType item in Dbers.GetInstance().SelfDber.Entities<CmcsGoodsType>("where ParentId=:ParentId order by OrderNumber asc", new { ParentId = entity.Id }))
            {
                DevComponents.AdvTree.Node newNode = CreateNode(item);
                node.Nodes.Add(newNode);
                LoadData(item, newNode);
            }
        }

        DevComponents.AdvTree.Node CreateNode(CmcsGoodsType entity)
        {
            DevComponents.AdvTree.Node node = new DevComponents.AdvTree.Node(entity.GoodsName + (entity.IsValid==1 ? "" : "(无效)"));
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
            this.Output = (advTree1.SelectedNode.Tag as CmcsGoodsType);
            addCmcsGoodsType(Output);
        }

        void addCmcsGoodsType(CmcsGoodsType item)
        {
            txt_GoodsName.Text = item.GoodsName;
            txt_Remark.Text = item.Remark;
            dbi_OrderNumber.Text = item.OrderNumber.ToString();
            chb_IsUse.Checked = (item.IsValid==1);
        }
    }
}