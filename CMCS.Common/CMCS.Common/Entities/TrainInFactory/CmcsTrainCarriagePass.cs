using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys; 

namespace CMCS.Common.Entities.TrainInFactory
{
    /// <summary>
    /// 火车车厢通过车号识别表
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("cmcstbtraincarriagepass")]
    public class CmcsTrainCarriagePass : EntityBase1
    {
        private string _MachineCode;
        /// <summary>
        /// 设备编号
        /// </summary>
        public string MachineCode
        {
            get { return _MachineCode; }
            set { _MachineCode = value; }
        }

        private string _TrainNumber;
        /// <summary>
        /// 车号
        /// </summary>
        public string TrainNumber
        {
            get { return _TrainNumber; }
            set { _TrainNumber = value; }
        }

        private DateTime _PassTime;
        /// <summary>
        /// 通过时间
        /// </summary>
        public DateTime PassTime
        {
            get { return _PassTime; }
            set { _PassTime = value; }
        }

        private string _Direction;
        /// <summary>
        /// 方向 进厂、出厂
        /// </summary>
        public string Direction
        {
            get { return _Direction; }
            set { _Direction = value; }
        }

        private int dataFlag;
        /// <summary>
        /// 标识符
        /// </summary>
        public int DataFlag
        {
            get { return dataFlag; }
            set { dataFlag = value; }
        }
    }
}
