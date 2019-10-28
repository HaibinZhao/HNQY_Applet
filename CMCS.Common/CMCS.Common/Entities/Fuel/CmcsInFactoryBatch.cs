using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.DAO;
using CMCS.Common.Entities.Sys;

namespace CMCS.Common.Entities.Fuel
{
    /// <summary>
    /// 入厂煤批次表
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("fultbinfactorybatch")]
    public class CmcsInFactoryBatch : EntityBase1
    {

        /// <summary>
        /// 入厂批次号
        /// </summary>
        public virtual String Batch { get; set; }

        /// <summary>
        /// 实际到达时间
        /// </summary>
        public virtual DateTime FactArriveDate { get; set; }

        /// <summary>
        /// 接车人员
        /// </summary>
        public virtual String Runner { get; set; }

        /// <summary>
        /// 接车时间
        /// </summary>
        public virtual DateTime RunDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual String Remark { get; set; }

        /// <summary>
        /// 量审核
        /// </summary>
        public virtual Int32 IsCheck { get; set; }

        /// <summary>
        /// 化验审核
        /// </summary>
        public virtual Int32 IsFinish_BM { get; set; }

        /// <summary>
        /// 批次类型 汽车、火车、船运
        /// </summary>
        public virtual String BatchType { get; set; }

        /// <summary>
        /// 父批次
        /// </summary>
        public virtual string ParentRemoveBatchId { get; set; }

        /// <summary>
        /// 供煤单位
        /// </summary>
        public virtual string SupplierId { get; set; }


        /// <summary>
        /// 托运单位
        /// </summary>
        public virtual string SendSupplierId { get; set; }


        /// <summary>
        /// 发站 多对一
        /// </summary>
        public virtual string StationId { get; set; }

        /// <summary>
        /// 矿点 多对一
        /// </summary>
        public virtual string MineId { get; set; }

        /// <summary>
        /// 关联：煤种
        /// </summary>
        public virtual string FuelKindId { get; set; }

        /// <summary>
        /// 关联：煤种名称
        /// </summary>
        public virtual string FuelKindName { get; set; }

        /// <summary>
        /// 关联：运输单位
        /// </summary>
        public virtual string TransportCompanyId { get; set; }

        /// <summary>
        /// 是否汽车智能化创建
        /// </summary>
        public virtual Int32 IsCTAutoCreate { get; set; }

        /// <summary>
        /// 关联：调运计划ID
        /// </summary>
        public virtual string LMYBId { get; set; }

        /// <summary>
        /// 调运计划
        /// </summary>
        [CMCS.DapperDber.Attrs.DapperIgnore]
        public virtual CmcsLMYB TheLMYB
        {
            get { return CommonDAO.GetInstance().SelfDber.Get<CmcsLMYB>(this.LMYBId); }
        }
    }
}
