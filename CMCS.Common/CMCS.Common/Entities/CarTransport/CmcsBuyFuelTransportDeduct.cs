﻿// 此代码由 NhGenerator v1.0.9.0 工具生成。

using System;
using System.Collections;
using CMCS.Common.Entities.Sys; 

namespace CMCS.Common.Entities.CarTransport
{
    /// <summary>
    /// 汽车智能化-入厂煤运输记录
    /// </summary>
    [Serializable]
    [CMCS.DapperDber.Attrs.DapperBind("CmcsTbBuyFuelTransportDeduct")]
    public class CmcsBuyFuelTransportDeduct : EntityBase1
    {
        private string _TransportId;
        /// <summary>
        /// 入厂运输记录Id
        /// </summary>
        public virtual string TransportId { get { return _TransportId; }  set { _TransportId = value; }}

        private string _DeductType;
        /// <summary>
        /// 扣吨类型
        /// </summary>
        public virtual string DeductType { get { return _DeductType; }  set { _DeductType = value; }}

        private Decimal _DeductWeight;
        /// <summary>
        /// 扣吨(吨)
        /// </summary>
        public virtual Decimal DeductWeight { get { return _DeductWeight; }  set { _DeductWeight = value; }}

        private string _Remark;
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get { return _Remark; }  set { _Remark = value; }}

    }
}
