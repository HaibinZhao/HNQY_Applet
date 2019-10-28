using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;

namespace CMCS.DumblyConcealer.Tasks.AutoCupboard_NCGM.Entities
{
    /// <summary>
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("INFTBCYGSIGNAL")]
    public class EquCYGSignal : EntityBase2
    {
        public String TagName { get; set; }
        public String TagValue { get; set; }
        public DateTime UpdateTime { get; set; }
        public Decimal DataFlag { get; set; }
        public String Remark { get; set; }

    }
}
