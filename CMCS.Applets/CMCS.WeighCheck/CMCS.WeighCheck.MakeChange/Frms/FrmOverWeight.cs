using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CMCS.Common;
using CMCS.Common.DAO;
using CMCS.WeighCheck.MakeChange.Frms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using DevComponents.Editors;
using CMCS.Common.Entities.Fuel;

namespace CMCS.WeighCheck.MakeChange.Frms
{
    public partial class FrmOverWeight : DevComponents.DotNetBar.Metro.MetroForm
    {
        string MakeDetailId;
        public FrmOverWeight(string makeDetailId)
        {
            InitializeComponent();
            MakeDetailId = makeDetailId;
        }

        private void FrmOverWeight_Load(object sender, EventArgs e)
        {
            IList<CmcsRCMakeDetail> list = new List<CmcsRCMakeDetail>();
            list.Add(Dbers.GetInstance().SelfDber.Get<CmcsRCMakeDetail>(MakeDetailId));
            superGridControl1.PrimaryGrid.DataSource = list;
        }

    }
}