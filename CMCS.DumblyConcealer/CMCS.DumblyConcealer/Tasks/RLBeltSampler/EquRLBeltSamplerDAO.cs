using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.DAO;
using CMCS.DumblyConcealer.Enums;
using CMCS.DapperDber.Dbs.SqlServerDb;

namespace CMCS.DumblyConcealer.Tasks.RLBeltSampler
{
    public class EquRLBeltSamplerDAO
    {

        private EquRLBeltSamplerDAO(string machineCode, SqlServerDapperDber equDber)
        {
            this.MachineCode = machineCode;
            this.EquDber = equDber;
        }


        CommonDAO commonDAO = CommonDAO.GetInstance();

        /// <summary>
        /// 第三方数据库访问对象
        /// </summary>
        SqlServerDapperDber EquDber;
        /// <summary>
        /// 设备编码
        /// </summary>
        string MachineCode;
        /// <summary>
        /// 是否处于故障状态
        /// </summary>
        bool IsHitch = false;
        /// <summary>
        /// 上一次上位机心跳值
        /// </summary>
        string PrevHeartbeat = string.Empty;


    }
}
