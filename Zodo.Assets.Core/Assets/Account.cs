using HZC.Database;
using System.Collections.Generic;

namespace Zodo.Assets.Core
{
    [MyDataTable("Base_Account")]
    public class Account : Entity
    {
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 固定电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 所在部门
        /// </summary>
        public int DeptId { get; set; }

        #region 导航属性
        [MyDataField(Ignore = true)]
        public virtual Dept Dept { get; set; }

        [MyDataField(Ignore = true)]
        public virtual List<Asset> Assets { get; set; }
        #endregion
    }
}
