using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;

namespace CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS.Entities
{

    /// <summary>
    /// 人工存查样信息 
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("FulTbSCSampleBag")]
    public class SampleInputBags : EntityBase1
    {
        /// <summary>
        /// 样品编码
        /// </summary>
        public String SampleCode { get; set; }
        /// <summary>
        /// 样品类型
        /// </summary>
        public String SampleType { get; set; }
        /// <summary>
        /// 存样人
        /// </summary>
        public String SaveUserName { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public String SupplierName { get; set; }
        /// <summary>
        /// 批次编号
        /// </summary>
        public String BatchNo { get; set; }
        /// <summary>
        ///其他：人工存查信息
        ///1：智能存查样信息
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 存放位置
        /// </summary>
        public String CunFangWeiZhi { get; set; }
    }
}
