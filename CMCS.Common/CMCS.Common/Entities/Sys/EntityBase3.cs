using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.DapperDber.Attrs;

namespace CMCS.Common.Entities.Sys
{
    [Serializable]
    public class EntityBase3
    {
        public EntityBase3()
        {
            this.Id = Guid.NewGuid().ToString();
        }
        [DapperPrimaryKey]
        public string Id { get; set; } 
    }
}
