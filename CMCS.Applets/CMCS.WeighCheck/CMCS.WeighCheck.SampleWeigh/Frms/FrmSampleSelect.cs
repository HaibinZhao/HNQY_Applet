using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CMCS.Common.DAO;
using CMCS.WeighCheck.SampleWeigh.Frms.SampleWeigth;
using DevComponents.DotNetBar.Controls;
using DevComponents.DotNetBar.Metro;
using DevComponents.DotNetBar.SuperGrid;
using CMCS.WeighCheck.DAO;

namespace CMCS.WeighCheck.SampleWeigh.Frms
{
    public partial class FrmSampleSelect : MetroForm
    {
        CZYHandlerDAO cZYHandlerDAO = CZYHandlerDAO.GetInstance();
        public FrmSampleWeigth _Form1;
        bool isExit = true;
        List<SampleInfo> listSampleInfo = new List<SampleInfo>();

        /// <summary>
        /// 当前日期
        /// </summary>
        DateTime CurrentDay = DateTime.Now;


        public FrmSampleSelect()
        {
            InitializeComponent();
        }

        private void Frm_Batch_Select_Load(object sender, EventArgs e)
        {
            _Form1 = this.Owner as FrmSampleWeigth;
            dtInputStart.Value = dtInputEnd.Value = DateTime.Now;

            BindData();
        }

        private void BindData()
        {
            listSampleInfo.Clear();

            DataTable dt = cZYHandlerDAO.GetSampleInfo(DateTime.Parse(dtInputStart.Text), DateTime.Parse(dtInputEnd.Text).AddDays(1), this.dtInputSampleCode.Text);
            if (dt != null)
            {
                foreach (DataRow drSample in dt.Rows)
                {
                    listSampleInfo.Add(new SampleInfo()
                    {
                        Id = drSample["Id"].ToString(),
                        Batch = drSample["Batch"].ToString(),
                        BatchId = drSample["BatchId"].ToString(),
                        //SupplierName = drSample["SupplierName"].ToString(),
                        SupplierName = "****",
                        MineName = drSample["MineName"].ToString(),
                        KindName = drSample["KindName"].ToString(),
                        StationName = drSample["StationName"].ToString(),
                        FactarriveDate = DateTime.Parse(drSample["FactarriveDate"].ToString()),
                        SampleCode = drSample["SampleCode"].ToString(),
                        SamplingDate = DateTime.Parse(drSample["SamplingDate"].ToString()),
                        SamplingType = drSample["SamplingType"].ToString()
                    });
                }
            }

            superGridControl1.PrimaryGrid.DataSource = listSampleInfo;
        }

        private void lvwInfactoryBatch_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewEx lvw = sender as ListViewEx;
            if (lvw != null)
            {
                ListViewItem item = lvw.GetItemAt(e.X, e.Y);

            }
        }

        private void Frm_SupplierUnit_Selet_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isExit)
                _Form1.CurrentSampleInfo = null;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(dtInputStart.Text))
                return;
            if (string.IsNullOrEmpty(dtInputEnd.Text))
                return;


            BindData();
        }

        private void btnPreDay_Click(object sender, EventArgs e)
        {
            CurrentDay = CurrentDay.AddDays(-1);
            ShowDateTime();
            BindData();
        }

        private void btnNextDay_Click(object sender, EventArgs e)
        {
            CurrentDay = CurrentDay.AddDays(1);
            ShowDateTime();
            BindData();
        }

        private void btnToday_Click(object sender, EventArgs e)
        {
            CurrentDay = DateTime.Now;
            ShowDateTime();
            BindData();
        }

        #region DateTime
        private void ShowDateTime()
        {
            dtInputStart.Value = dtInputEnd.Value = DateTime.Parse(CurrentDay.ToString("yyyy-MM-dd"));
        }
        #endregion

        private void superGridControl1_BeginEdit(object sender, DevComponents.DotNetBar.SuperGrid.GridEditEventArgs e)
        {
            // 取消编辑模式
            e.Cancel = true;
        }

        private void superGridControl1_CellDoubleClick(object sender, DevComponents.DotNetBar.SuperGrid.GridCellDoubleClickEventArgs e)
        {
            GridRow gridRow = (sender as SuperGridControl).PrimaryGrid.ActiveRow as GridRow;
            if (gridRow == null) return;

            SampleInfo entity = (gridRow.DataItem as SampleInfo);
            if (entity != null)
            {
                _Form1.CurrentSampleInfo = entity;
                isExit = false;
                this.Close();
            }
        }
    }

    /// <summary>
    /// 临时类-采样单
    /// </summary>
    public class SampleInfo
    {
        public string Id { get; set; }
        public string BatchId { get; set; }
        public string Batch { get; set; }
        public string SupplierName { get; set; }
        public string MineName { get; set; }
        public string KindName { get; set; }
        public string StationName { get; set; }
        public DateTime FactarriveDate { get; set; }
        public int TransportNumber { get; set; }
        public string SampleCode { get; set; }
        public DateTime SamplingDate { get; set; }
        public string SamplingType { get; set; }
    }
}
