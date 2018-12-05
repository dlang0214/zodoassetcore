namespace Zodo.Assets.Application
{
    public class AccountListDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

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

        /// <summary>
        /// 所在部门名称
        /// </summary>
        public string DeptName { get; set; }
    }
}
