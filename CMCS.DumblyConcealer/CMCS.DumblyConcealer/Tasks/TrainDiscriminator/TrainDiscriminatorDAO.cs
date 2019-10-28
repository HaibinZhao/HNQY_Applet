using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.DumblyConcealer.Enums;
using CMCS.Common;
using System.IO;
using CMCS.Common.Entities;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using CMCS.Common.Entities.TrainInFactory;

namespace CMCS.DumblyConcealer.Tasks.TrainDiscriminator
{
    /// <summary>
    /// 火车车号识别业务
    /// </summary>
    public class TrainDiscriminatorDAO
    {
        private static TrainDiscriminatorDAO instance;

        private static String MachineCode_CHSB = GlobalVars.MachineCode_HCRCCHSB;

        public static TrainDiscriminatorDAO GetInstance()
        {
            if (instance == null)
            {

                instance = new TrainDiscriminatorDAO();
            }
            return instance;
        }

        private TrainDiscriminatorDAO()
        {

        }

        /// <summary>
        /// 同步报文
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int Save(List<CmcsTrainCarriagePass> cmcstraincarriagepass, Action<string, eOutputType> output)
        {
            int res = 0;
            try
            {
                foreach (var item in cmcstraincarriagepass)
                {

                    CmcsTrainCarriagePass item1 = Dbers.GetInstance().SelfDber.Entity<CmcsTrainCarriagePass>("where TrainNumber=:TrainNumber and PassTime=:PassTime", new { TrainNumber = item.TrainNumber, PassTime = item.PassTime });
                    if (item1 == null)
                    {
                        res += Dbers.GetInstance().SelfDber.Insert(item);
                    }
                    else
                    {
                        item1.Direction = item.Direction;
                        item1.MachineCode = item.MachineCode;
                        item1.TrainNumber = item.TrainNumber;
                        item1.PassTime = item.PassTime;
                        item1.DataFlag = item.DataFlag;
                        res += Dbers.GetInstance().SelfDber.Update<CmcsTrainCarriagePass>(item1);
                    }
                }
            }
            catch (Exception ex)
            {
                output(string.Format("保存数据失败,原因:{0}", ex.Message), eOutputType.Error);
            }
            output(string.Format("同步车号识别数据 {0} 条（集中管控 ）", res), eOutputType.Normal);
            return res;
        }




    }
}
