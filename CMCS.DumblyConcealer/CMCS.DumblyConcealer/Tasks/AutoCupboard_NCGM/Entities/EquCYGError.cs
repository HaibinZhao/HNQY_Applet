using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;

namespace CMCS.DumblyConcealer.Tasks.AutoCupboard_NCGM.Entities
{
    [CMCS.DapperDber.Attrs.DapperBind("InfTbCYGError")]
    public class EquCYGError : EntityBase2
    {
        public Decimal ErrorCode { get; set; }
        public String ErrorDescribe { get; set; }
        public DateTime Err_Time { get; set; }
        public Decimal DataFlag { get; set; }
    }
}
