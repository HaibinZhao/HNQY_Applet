using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys; 

namespace CMCS.Common.Entities.BaseInfo
{
    /// <summary>
    /// 基础信息-煤种
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("fultbfuelkind")]
    public class CmcsFuelKind : EntityBase1
    {
        private string _FuelCode;
        /// <summary>
        /// 编码
        /// </summary>
        public string FuelCode
        {
            get { return _FuelCode; }
            set { _FuelCode = value; }
        }

        private string _FuelName;
        /// <summary>
        /// 名称
        /// </summary>
        public string FuelName
        {
            get { return _FuelName; }
            set { _FuelName = value; }
        }


        private string _ParentId;
        /// <summary>
        /// 父Id
        /// </summary>
        public string ParentId
        {
            get { return _ParentId; }
            set { _ParentId = value; }
        }
        /// <summary>
        /// 是否有效
        /// </summary>
        public virtual String IsUse { get; set; }

        public int Sequence { get;set; }

        public string ReMark { get; set; }
        public string Valid { get; set; }
    } 
}
