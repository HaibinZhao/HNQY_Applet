using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;

namespace CMCS.Common.Entities.Fuel
{
    /// <summary>
    /// 入厂煤采样表
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("cmcstbrcsampling")]
    public class CmcsRCSampling : EntityBase1
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

        private string _InFurnanceId;

        /// <summary>
        /// 关联入炉id
        /// </summary>
        public string INFURNACEID 
        {
            get { return _InFurnanceId; }
            set { _InFurnanceId = value; }
        }

        private DateTime _SamplingDate;

        /// <summary>
        /// 采样时间
        /// </summary>
        public DateTime SamplingDate
        {
            get { return _SamplingDate; }
            set { _SamplingDate = value; }
        }
        private string _SamplingPle;

        /// <summary>
        /// 采样人
        /// </summary>
        public string SamplingPle
        {
            get { return _SamplingPle; }
            set { _SamplingPle = value; }
        }
        private string _SamplingType;

        /// <summary>
        /// 采样方式
        /// </summary>
        public string SamplingType
        {
            get { return _SamplingType; }
            set { _SamplingType = value; }
        }
        private string _SampleCode;

        /// <summary>
        /// 采样码
        /// </summary>
        public string SampleCode
        {
            get { return _SampleCode; }
            set { _SampleCode = value; }
        }
        private string _BackupCode;

        /// <summary>
        /// 备用码
        /// </summary>
        public string BackupCode
        {
            get { return _BackupCode; }
            set { _BackupCode = value; }
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

        private String _CYRBC;
        /// <summary>
        /// 采样人班次
        /// </summary>
        public virtual String CYRBC { get { return _CYRBC; } set { _CYRBC = value; } }

        private String _SamplingCategory;
        /// <summary>
        /// 采样类别 入厂采样 入炉采样
        /// </summary>
        public virtual String SamplingCategory { get { return _SamplingCategory; } set { _SamplingCategory = value; } }

        private string _ParentId;

        /// <summary>
        /// 留样合样采样单Id
        /// </summary>
        public string ParentId
        {
            get { return _ParentId; }
            set { _ParentId = value; }
        }
    }
}
