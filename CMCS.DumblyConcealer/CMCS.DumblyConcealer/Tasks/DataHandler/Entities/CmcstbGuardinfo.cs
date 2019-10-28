using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;

namespace CMCS.DumblyConcealer.Tasks.DataHandler.Entities
{
    /// <summary>
    /// 门禁
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("cmcstbguardinfo")]
    public class CmcstbGuardinfo : EntityBase1
    {
        private string f_READDATE;
        /// <summary>
        /// 刷卡时间
        /// </summary>
        public string F_READDATE
        {
            get { return f_READDATE; }
            set { f_READDATE = value; }
        }

        private string f_CONSUMERID;
        /// <summary>
        /// 用户id
        /// </summary>
        public string F_CONSUMERID
        {
            get { return f_CONSUMERID; }
            set { f_CONSUMERID = value; }
        }

        private string f_CONSUMERNAME;
        /// <summary>
        /// 用户名
        /// </summary>
        public string F_CONSUMERNAME
        {
            get { return f_CONSUMERNAME; }
            set { f_CONSUMERNAME = value; }
        }

        private string f_READERID;
        /// <summary>
        /// 门id
        /// </summary>
        public string F_READERID
        {
            get { return f_READERID; }
            set { f_READERID = value; }
        }

        private string f_READERNAME;
        /// <summary>
        /// 门名称
        /// </summary>
        public string F_READERNAME
        {
            get { return f_READERNAME; }
            set { f_READERNAME = value; }
        }
    }
}
