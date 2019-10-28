using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;

namespace CMCS.Common.Entities.AutoCupboard
{
    /// <summary>
    /// 集中管控设备
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("CMCSTBCYGCONTROLCMD")]
    public class CmcsCYGControlCMD : EntityBase1
    {
        public virtual DateTime PlanDate { get; set; }
        public virtual String Bill { get; set; }
        public virtual String OperPerson { get; set; }
        public virtual String OperType { get; set; }
        public virtual Decimal CodeNumber { get; set; }
        public virtual DateTime UpdateTime { get; set; }
        public virtual Decimal DataFlag { get; set; }
        public virtual String ReMark { get; set; }
        public virtual String StartPlace { get; set; }
        public virtual String Place { get; set; }
        public virtual String Status { get; set; }
        public virtual String CanWorking { get; set; }
        public virtual String MakeCode { get; set; }
    }
}
