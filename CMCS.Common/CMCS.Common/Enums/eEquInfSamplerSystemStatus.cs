using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCS.Common.Enums
{
	/// <summary>
	/// 第三方设备接口 - 采样机系统状态
	/// </summary>
	public enum eEquInfSamplerSystemStatus
	{
		#region 公共
		正在运行 = 1,
		发生故障 = 2,
		就绪待机 = 3,
		正在卸样 = 4,
		#endregion

		#region 入厂皮带采样机
		就绪待机2,
		未就绪
		#endregion
	}
}
