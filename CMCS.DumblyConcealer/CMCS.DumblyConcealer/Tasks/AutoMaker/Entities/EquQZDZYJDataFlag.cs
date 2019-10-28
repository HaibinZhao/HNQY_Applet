using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities;
using CMCS.Common.Entities.Sys;

namespace CMCS.DumblyConcealer.Tasks.AutoMaker.Entities
{
    /// <summary>
    /// 全自动制样机接口表 - 心跳信号表
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("DataFlag")]
    public class EquQZDZYJDataFlag
    {
        private string _DataFlag;
        /// <summary>
        /// 标识符
        /// </summary>
        [CMCS.DapperDber.Attrs.DapperPrimaryKey]
        public string DataFlag
        {
            get { return _DataFlag; }
            set { _DataFlag = value; }
        }
    }
}
