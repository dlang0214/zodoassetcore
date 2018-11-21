using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Senparc.Weixin.Work.AdvancedAPIs;
using Senparc.Weixin.Work.AdvancedAPIs.MailList;
using Senparc.Weixin.Work.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using Zodo.Assets.Website.Options;

namespace Zodo.Assets.Website.Extensions
{
    public class WeixinDeptUtil
    {
        private static List<DepartmentList> _depatments = null;
        private static object obj = new object();

        private static void Init()
        {
            var token = AccessTokenContainer.TryGetToken(WeixinWorkOptions.CorpId, WeixinWorkOptions.Secret);
            var result = MailListApi.GetDepartmentList(token);
            if (result.errcode != Senparc.Weixin.ReturnCode_Work.请求成功)
            {
                throw new Exception("部门列表接口获取失败");
            }
            else
            {
                _depatments = result.department;
            }
        }

        public static List<DepartmentList> All()
        {
            if (_depatments == null)
            {
                lock (obj)
                {
                    Init();
                }
            }
            return _depatments;
        }

        public static DepartmentList Get(int id)
        {
            return All().Where(a => a.id == id).SingleOrDefault();
        }

        public static void Clear()
        {
            _depatments = null;
        }

        public static void Reset()
        {
            _depatments = null;
            Init();
        }
    }
}
