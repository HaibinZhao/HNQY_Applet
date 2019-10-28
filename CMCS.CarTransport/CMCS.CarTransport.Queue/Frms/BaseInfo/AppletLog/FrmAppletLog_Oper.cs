using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using CMCS.Common;
using CMCS.Common.Entities.CarTransport;
using DevComponents.DotNetBar.Controls;
using DevComponents.Editors;
using CMCS.CarTransport.Queue.Utilities;
using CMCS.CarTransport.DAO;
using CMCS.Common.Enums;
using CMCS.Common.Entities.Sys;

namespace CMCS.CarTransport.Queue.Frms.BaseInfo.AppletLog
{
    public partial class FrmAppletLog_Oper : DevComponents.DotNetBar.Metro.MetroForm
    {
        String id = String.Empty;
        bool edit = false;
        CmcsAppletLog cmcsAppletLog;
        public FrmAppletLog_Oper()
        {
            InitializeComponent();
            edit = true;
        }
        public FrmAppletLog_Oper(CmcsAppletLog pId, bool pEdit)
        {
            InitializeComponent();
            cmcsAppletLog = pId;
            edit = pEdit;
        }


        private void FrmAppletLog_Oper_Load(object sender, EventArgs e)
        {
            txtTitle.Text=cmcsAppletLog.Title;
            txt_Content.Text = cmcsAppletLog.Content;
        }
    }
}