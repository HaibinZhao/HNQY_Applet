using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCS.DumblyConcealer.Tasks.AssayDevice.Entities
{
    public class AssayTemp
    {
        public string AssayCode { get; set; }
        public string FinishStatus { get; set; }
        public string CheckStatus { get; set; }
        public DateTime AssayDate { get; set; }
    }
}
