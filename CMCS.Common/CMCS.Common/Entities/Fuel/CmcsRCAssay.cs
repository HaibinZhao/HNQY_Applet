using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;

namespace CMCS.Common.Entities.Fuel
{
    /// <summary>
    /// 入厂煤化验表
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("fultbrchyassay")]
    public class CmcsRCAssay : EntityBase1
    {
        private string _InFactoryBatchId;

        /// <summary>
        /// 关联批次id
        /// </summary>
        public string InFactoryBatchId
        {
            get { return _InFactoryBatchId; }
            set { _InFactoryBatchId = value; }
        }

        private String _AssayType;
        /// <summary>
        /// 化验类型
        /// </summary>
        public virtual String AssayType { get { return _AssayType; } set { _AssayType = value; } }

        private String _BillNumber;
        /// <summary>
        /// 化验编码
        /// </summary>
        public virtual String BillNumber { get { return _BillNumber; } set { _BillNumber = value; } }

        private String _MakeId;
        /// <summary>
        /// 制样记录Id
        /// </summary>
        public virtual String MakeId { get { return _MakeId; } set { _MakeId = value; } }
        private String _AssayPle;
        /// <summary>
        /// 化验人
        /// </summary>
        public virtual String AssayPle { get { return _AssayPle; } set { _AssayPle = value; } }

        private DateTime _AssayDate;
        /// <summary>
        /// 化验时间
        /// </summary>
        public virtual DateTime AssayDate { get { return _AssayDate; } set { _AssayDate = value; } }

        private String _SendPle;
        /// <summary>
        /// 制样送样人
        /// </summary>
        public virtual String SendPle { get { return _SendPle; } set { _SendPle = value; } }

        private String _GetPle;
        /// <summary>
        /// 化验收样人
        /// </summary>
        public virtual String GetPle { get { return _GetPle; } set { _GetPle = value; } }

        private DateTime _GetDate;
        /// <summary>
        /// 化验收样时间
        /// </summary>
        public virtual DateTime GetDate { get { return _GetDate; } set { _GetDate = value; } }

        private string _FuelQualityId;

        /// <summary>
        /// 关联煤质id
        /// </summary>
        public string FuelQualityId
        {
            get { return _FuelQualityId; }
            set { _FuelQualityId = value; }
        }
        private string _Remark;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get { return _Remark; }
            set { _Remark = value; }
        }

        private String _AssayCategory;
        /// <summary>
        /// 化验类别 入厂化验 入炉化验
        /// </summary>
        public virtual String AssayCategory { get { return _AssayCategory; } set { _AssayCategory = value; } }


        private String _ISCHECK;
        /// <summary>
        /// 是否审核
        /// </summary>
        public virtual String ISCHECK { get { return _ISCHECK; } set { _ISCHECK = value; } }

    }
}
