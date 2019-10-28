using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar.Controls;
using DevComponents.Editors;
using DevComponents.DotNetBar;
using System.Drawing;

namespace CMCS.CarTransport.Queue.Utilities
{
    public class Helper : DevComponents.DotNetBar.Metro.MetroForm
    {

        public static void ControlReadOnly(Control ctl)
        {

            foreach (Control item in ctl.Controls)
            {
                if (item.Controls.Count > 0)
                {
                    ControlReadOnly(item);
                }
                else if (item is TextBoxX)
                {
                    ((TextBoxX)item).ReadOnly = true;
                    ((Control)item).Enter += new EventHandler(Helper_Enter);
                }
                else if (item is IntegerInput)
                {
                    ((IntegerInput)item).IsInputReadOnly = true;
                    ((Control)item).Enter += new EventHandler(Helper_Enter);
                }
                else if (item is DoubleInput)
                {
                    ((DoubleInput)item).IsInputReadOnly = true;
                    ((Control)item).Enter += new EventHandler(Helper_Enter);
                }
                else if (item is CheckBoxX)
                {
                    ((CheckBoxX)item).Enabled = false;
                    ((Control)item).Enter += new EventHandler(Helper_Enter);
                }
                else if (item is ComboBoxEx)
                {
                    ((ComboBoxEx)item).DisabledBackColor = ((ComboBoxEx)item).BackColor;
                    ((ComboBoxEx)item).DisabledForeColor = ((ComboBoxEx)item).ForeColor;
                    ((ComboBoxEx)item).Enabled = false;
                    ((Control)item).Enter += new EventHandler(Helper_Enter);
                }
                else if (item is ButtonX)
                {
                    ((ButtonX)item).Enabled = false;
                    ((Control)item).Enter += new EventHandler(Helper_Enter);
                }
                
            }
        }

        static void Helper_Enter(object sender, EventArgs e)
        {
        }
    }
}
