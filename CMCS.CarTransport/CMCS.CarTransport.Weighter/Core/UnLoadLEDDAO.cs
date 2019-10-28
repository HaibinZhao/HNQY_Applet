using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.CarTransport;
using LED.YB14;
using CMCS.Common.Utilities;
using System.IO;
using CMCS.Common.DAO;

namespace CMCS.CarTransport.Weighter.Core
{
    public class UnLoadLEDDAO
    {
        private static UnLoadLEDDAO instance;

        public static UnLoadLEDDAO GetInstance()
        {
            if (instance == null)
            {
                instance = new UnLoadLEDDAO();
            }

            return instance;
        }

        CommonDAO commonDAO = CommonDAO.GetInstance();

        #region LED1控制卡

        /// <summary>
        /// LED1控制卡屏号
        /// </summary>
        int LED1nScreenNo = 2;
        /// <summary>
        /// LED动态区域号1
        /// </summary>
        int LED1DYArea_ID1 = 0;
        /// <summary>
        /// LED动态区域号2
        /// </summary>
        int LED1DYArea_ID2 = 1;
        /// <summary>
        /// LED动态区域号3
        /// </summary>
        int LED1DYArea_ID3 = 2;

        /// <summary>
        /// LED1更新标识
        /// </summary>
        bool LED1m_bSendBusy = false;

        private bool _LED1ConnectStatus;
        /// <summary>
        /// LED1连接状态
        /// </summary>
        public bool LED1ConnectStatus
        {
            get
            {
                return _LED1ConnectStatus;
            }

            set
            {
                _LED1ConnectStatus = value;

            }
        }

        /// <summary>
        /// LED1显示内容文本1
        /// </summary>
        string LED1TempFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UnLoadLed1TempFile.txt");

        /// <summary>
        /// LED1显示内容文本2
        /// </summary>
        string LED2TempFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UnLoadLed2TempFile.txt");

        /// <summary>
        /// LED1显示内容文本3
        /// </summary>
        string LED3TempFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UnLoadLed3TempFile.txt");

        /// <summary>
        /// LED1上一次显示内容
        /// </summary>
        string LED1PrevLedFileContent = string.Empty;

        /// <summary>
        /// 更新LED1动态区域
        /// </summary>
        /// <param name="value1">第一行内容</param>
        /// <param name="value2">第二行内容</param>
        /// <param name="value3">第三行内容</param>
        private void UpdateLed1Show(string value1 = "", string value2 = "", string value3 = "")
        {
            if (!this.LED1ConnectStatus) return;
            if (this.LED1PrevLedFileContent == value1 + value2 + value3) return;

            string ledContent = GenerateFillLedContent12(value1);
            File.WriteAllText(this.LED1TempFile, ledContent, Encoding.UTF8);

            ledContent = GenerateFillLedContent12(value2);
            File.WriteAllText(this.LED2TempFile, ledContent, Encoding.UTF8);

            ledContent = GenerateFillLedContent12(value3);
            File.WriteAllText(this.LED3TempFile, ledContent, Encoding.UTF8);

            if (LED1m_bSendBusy == false)
            {
                LED1m_bSendBusy = true;

                int nResult = YB14DynamicAreaLeder.SendDynamicAreaInfoCommand(this.LED1nScreenNo, this.LED1DYArea_ID1);
                nResult = YB14DynamicAreaLeder.SendDynamicAreaInfoCommand(this.LED1nScreenNo, this.LED1DYArea_ID2);
                nResult = YB14DynamicAreaLeder.SendDynamicAreaInfoCommand(this.LED1nScreenNo, this.LED1DYArea_ID3);
                if (nResult != YB14DynamicAreaLeder.RETURN_NOERROR) Log4Neter.Error("更新LED动态区域", new Exception(YB14DynamicAreaLeder.GetErrorMessage("SendDynamicAreaInfoCommand", nResult)));

                LED1m_bSendBusy = false;
            }

            this.LED1PrevLedFileContent = value1 + value2;
        }

        /// <summary>
        /// 生成12字节的文本内容
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GenerateFillLedContent12(string value)
        {
            int length = Encoding.Default.GetByteCount(value);
            if (length < 12) return value + "".PadRight(12 - length, ' ');

            return value;
        }
        #endregion

        /// <summary>
        /// 更新卸煤沟LED
        /// </summary>
        /// <param name="carNumber">车牌号</param>
        /// <param name="isAdd">是否显示</param>
        public bool UpdateLEDShow(CmcsUnLoadLED unLoadLED)
        {
            if (unLoadLED == null) return false;
            string carNumber1 = string.Empty, carNumber2 = string.Empty;
            if (!string.IsNullOrEmpty(unLoadLED.CarNumber))
            {
                string[] carNumbers = unLoadLED.CarNumber.Split('|');
                carNumber1 = carNumbers[0];
                if (carNumbers.Length == 2) carNumber2 = carNumbers[1];
            }

            #region LED控制卡

            string led1SocketIP = unLoadLED.IP;

            if (!string.IsNullOrEmpty(led1SocketIP))
            {
                if (YB14DynamicAreaLeder.PingReplyTest(led1SocketIP))
                {
                    //初始化之前先重置屏幕区域
                    //YB14DynamicAreaLeder.SendDeleteDynamicAreasCommand(this.LED1nScreenNo, 1, string.Format("{0},{1},{2}", this.LED1DYArea_ID1, this.LED1DYArea_ID2, this.LED1DYArea_ID3));
                    YB14DynamicAreaLeder.DeleteScreen(this.LED1nScreenNo);

                    int nResult = YB14DynamicAreaLeder.AddScreen(YB14DynamicAreaLeder.CONTROLLER_BX_5E1, this.LED1nScreenNo, YB14DynamicAreaLeder.SEND_MODE_NETWORK, 192, 64, 1, 1, "", 0, led1SocketIP, 5005, "");
                    if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
                    {
                        //屏幕分为3个区域 左 64*64 右上 128*32 右下 128*32
                        #region 左
                        nResult = YB14DynamicAreaLeder.AddScreenDynamicArea(this.LED1nScreenNo, this.LED1DYArea_ID1, 0, 10, 1, "", 0, 0, 0, 64, 64, 255, 0, 255, 7, 6, 1);
                        if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
                        {
                            nResult = YB14DynamicAreaLeder.AddScreenDynamicAreaFile(this.LED1nScreenNo, this.LED1DYArea_ID1, this.LED1TempFile, 0, "宋体", 48, 0, 120, 1, 3, 0);
                            if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
                            {
                                // 初始化成功
                                this.LED1ConnectStatus = true;
                            }
                            else
                            {
                                this.LED1ConnectStatus = false;
                                Log4Neter.Error("初始化卸煤沟LED控制卡", new Exception(YB14DynamicAreaLeder.GetErrorMessage("AddScreenDynamicAreaFile", nResult)));
                            }
                        }
                        #endregion

                        #region 右上
                        nResult = YB14DynamicAreaLeder.AddScreenDynamicArea(this.LED1nScreenNo, this.LED1DYArea_ID2, 0, 10, 1, "", 0, 64, 0, 128, 32, 255, 0, 255, 7, 6, 1);
                        if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
                        {
                            nResult = YB14DynamicAreaLeder.AddScreenDynamicAreaFile(this.LED1nScreenNo, this.LED1DYArea_ID2, this.LED2TempFile, 0, "宋体", 22, 0, 120, 1, 3, 0);
                            if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
                            {
                                // 初始化成功
                                this.LED1ConnectStatus = true;
                            }
                            else
                            {
                                this.LED1ConnectStatus = false;
                                Log4Neter.Error("初始化卸煤沟LED控制卡", new Exception(YB14DynamicAreaLeder.GetErrorMessage("AddScreenDynamicAreaFile", nResult)));
                            }
                        }
                        #endregion

                        #region 右下
                        nResult = YB14DynamicAreaLeder.AddScreenDynamicArea(this.LED1nScreenNo, this.LED1DYArea_ID3, 0, 10, 1, "", 0, 64, 32, 128, 32, 255, 0, 255, 7, 6, 1);

                        if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
                        {
                            nResult = YB14DynamicAreaLeder.AddScreenDynamicAreaFile(this.LED1nScreenNo, this.LED1DYArea_ID3, this.LED3TempFile, 0, "宋体", 22, 0, 120, 1, 3, 0);
                            if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
                            {
                                // 初始化成功
                                this.LED1ConnectStatus = true;
                            }
                            else
                            {
                                this.LED1ConnectStatus = false;
                                Log4Neter.Error("初始化卸煤沟LED控制卡", new Exception(YB14DynamicAreaLeder.GetErrorMessage("AddScreenDynamicAreaFile", nResult)));
                            }
                        }
                        else
                        {
                            this.LED1ConnectStatus = false;
                            Log4Neter.Error("初始化卸煤沟LED控制卡", new Exception(YB14DynamicAreaLeder.GetErrorMessage("AddScreenDynamicArea", nResult)));
                        }
                        #endregion
                    }
                    else
                    {
                        this.LED1ConnectStatus = false;
                        Log4Neter.Error("初始化卸煤沟LED控制卡", new Exception(YB14DynamicAreaLeder.GetErrorMessage("AddScreen", nResult)));
                    }
                }
                else
                {
                    this.LED1ConnectStatus = false;
                    Log4Neter.Error("初始化卸煤沟LED控制卡", new Exception("网络连接失败"));
                }
            }

            #endregion

            if (this.LED1ConnectStatus)
            {
                UpdateLed1Show(unLoadLED.UnLoadName, !string.IsNullOrEmpty(carNumber1) ? carNumber1 : "等待车辆", !string.IsNullOrEmpty(carNumber2) ? carNumber2 : "等待车辆");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 发送LED显示
        /// </summary>
        /// <param name="carNumber">车号</param>
        /// <param name="IsGross">是否重磅</param>
        /// <returns></returns>
        public string SendUnLoadLED(string carNumber, bool IsGross)
        {
            CmcsUnLoadLED unLoadLED = null;
            /*目前为：车辆按照8个卸煤沟顺序排列，如果8个卸煤沟都有车辆 则显示在右下方为待进入卸煤沟车辆 车辆回皮后清除显示  后期需要加入批次功能，
             * 同一批次进入同一卸煤沟*/
            if (IsGross)//重车 添加车号显示  
            {
                unLoadLED = commonDAO.SelfDber.Entity<CmcsUnLoadLED>(string.Format("where CarNumber like '%{0}%' order by UnLoadNumber ", carNumber));//已经存在
                if (unLoadLED != null) return unLoadLED.UnLoadName;
                unLoadLED = commonDAO.SelfDber.Entity<CmcsUnLoadLED>("where IsUse=0 order by UnLoadNumber ");
                if (unLoadLED == null) unLoadLED = commonDAO.SelfDber.Entity<CmcsUnLoadLED>("where nvl(length(CarNumber),0)<8 order by UnLoadNumber ");
                if (unLoadLED == null) return "";
                if (!string.IsNullOrEmpty(unLoadLED.CarNumber))
                    unLoadLED.CarNumber += "|" + carNumber;
                else
                    unLoadLED.CarNumber = carNumber;
                unLoadLED.IsUse = 1;
                if (commonDAO.SelfDber.Update(unLoadLED) > 0 && UpdateLEDShow(unLoadLED))
                    return unLoadLED.UnLoadName;
            }
            else//轻车移除车号显示
            {
                unLoadLED = commonDAO.SelfDber.Entity<CmcsUnLoadLED>(string.Format("where CarNumber like '%{0}%' order by UnLoadNumber ", carNumber));
                if (unLoadLED == null) return "";
                if (!string.IsNullOrEmpty(unLoadLED.CarNumber))
                    unLoadLED.CarNumber = unLoadLED.CarNumber.Replace(carNumber, "").Replace("|", "");
                if (string.IsNullOrEmpty(unLoadLED.CarNumber)) unLoadLED.IsUse = 0;
                if (commonDAO.SelfDber.Update(unLoadLED) > 0 && UpdateLEDShow(unLoadLED))
                    return unLoadLED.UnLoadName;
            }

            return "";
        }
    }
}
