using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using DevComponents.DotNetBar;
using CMCS.Common;
using DevComponents.DotNetBar.SuperGrid;
using CMCS.Common.Entities;
using CMCS.CarTransport.DAO;
using CMCS.Common.DAO;
using CMCS.Common.Entities.CarTransport;

namespace CMCS.CarTransport.WeightNotesPrint.Frms
{
    public partial class FrmWeightCar_DeDuc : DevComponents.DotNetBar.Metro.MetroForm
    {

        String id = String.Empty;
        bool edit = false;

        CommonDAO commonDAO = CommonDAO.GetInstance();

        public FrmWeightCar_DeDuc()
        {
            InitializeComponent();
        }
        public FrmWeightCar_DeDuc(String pId, bool pEdit)
        {
            InitializeComponent();
            id = pId;
            edit = pEdit;
        }
        private void FrmSupplier_Oper_Load(object sender, EventArgs e)
        {
            this.MinimizeBox = false;

        }

        private void txtInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                CmcsBuyFuelTransport transport = Dbers.GetInstance().SelfDber.Get<CmcsBuyFuelTransport>(this.id);

                if (transport != null)
                {
                    decimal deducqty = 0;
                    if (decimal.TryParse(txtInput.Text, out deducqty))
                    {
                        //�۶�
                        transport.DeductWeight = deducqty;

                        if (Dbers.GetInstance().SelfDber.Update(transport) > 0)
                        {
                            //���������¼������
                            commonDAO.InsertWaitForHandleEvent("�������ܻ�_ͬ���볧ú�����¼������", transport.Id);

                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBoxEx.Show("��������ȷ�Ŀ۶�����", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }


                }

            }
        }
    }
}