using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS.Entities
{
    [CMCS.DapperDber.Attrs.DapperBind("WARNING_INFO")]
    public class Warining_Info
    {
        [CMCS.DapperDber.Attrs.DapperPrimaryKey]
        public Int32 REC_NO { get; set; }
        public Int32 DeviceCode { get; set; }
        public Int32 WarningCode { get; set; }
        public String DeviceName { get; set; }
        public String WarningDesc { get; set; }
        public DateTime CreateTime { get; set; }
        public Decimal RDFLAG { get; set; }
    }
}
