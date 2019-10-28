using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCS.DumblyConcealer.Tasks.AutoCupboard_NCGM.Entities
{
    /// <summary>
    /// 南昌光明存样柜接口表 - 上位机运行状态表
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("DATAFLAG")]
    public class EquCYGDataFlag
    {
        private string _DataFlag;
        /// <summary>
        /// 心跳变化值
        /// </summary>
        [CMCS.DapperDber.Attrs.DapperPrimaryKey]
        public string DataFlag
        {
            get { return _DataFlag; }
            set { _DataFlag = value; }
        }
    }
}
