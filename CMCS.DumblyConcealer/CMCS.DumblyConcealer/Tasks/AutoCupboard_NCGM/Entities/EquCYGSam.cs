using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities;

namespace CMCS.DumblyConcealer.Tasks.AutoCupboard_NCGM.Entities
{
    /// <summary>
    /// 南昌光明存样柜--iv.	实时样品表 
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("ZCQX200")]
    public class EquCYGSam
    {

        public String 柜号 { get; set; }
        public Decimal 柜码 { get; set; }
        public Decimal 柜子状态 { get; set; }
        public String 制样码 { get; set; }
        public String 样瓶编码 { get; set; }
        public String 瓶子类型 { get; set; }
        public String 操作人员代码 { get; set; }
        public DateTime 操作时间 { get; set; }
        public Decimal UTC_Date { get; set; }
    }
}
