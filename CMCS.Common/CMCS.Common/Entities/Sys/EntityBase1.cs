using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
using CMCS.DapperDber.Attrs;
using System.Reflection;

namespace CMCS.Common.Entities.Sys
{
    [Serializable]
    public class EntityBase1
    {
        public EntityBase1()
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUser = GlobalVars.LoginUser != null ? GlobalVars.LoginUser.UserAccount : "admin";
            this.OperDate = this.CreateDate;
            this.OperUser = this.CreateUser;
            this.CreateUserId = "-2";
            this.CreateUserDeptId = "-1";
            this.CreateUserDeptCode = "00";
        }

        [DapperPrimaryKey]
        public string Id { get; set; }
        public DateTime OperDate { get; set; }
        public string OperUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public string CreateUserId { get; set; }
        public string CreateUserDeptId { get; set; }
        public string CreateUserDeptCode { get; set; }



        /// <summary>
        /// 更新EntityBase实体的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param> 
        public void UpdateEntityBase<T>(T t, string updateUserAccount) where T : EntityBase1
        {
            Type type = t.GetType();
            PropertyInfo piOperUser = type.GetProperty("OperUser", typeof(string));
            if (piOperUser != null)
                piOperUser.SetValue(t, updateUserAccount, null);
            PropertyInfo piOperDate = type.GetProperty("OperDate", typeof(DateTime));
            if (piOperDate != null)
                piOperDate.SetValue(t, DateTime.Now, null);
        }
    }
}
