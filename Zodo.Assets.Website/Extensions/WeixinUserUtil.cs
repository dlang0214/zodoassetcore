using Senparc.Weixin.Work.AdvancedAPIs;
using Senparc.Weixin.Work.AdvancedAPIs.MailList;
using Senparc.Weixin.Work.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using Zodo.Assets.Website.Options;

namespace Zodo.Assets.Website.Extensions
{
    public static class WeixinUserUtil
    {
        private static List<GetMemberResult> _members;
        private static readonly object obj = new object();
        
        private static void Init()
        {
            //var token = CommonApi.GetToken(WeixinWorkOptions.CorpId, WeixinWorkOptions.ContactsSecret); 
            var token = AccessTokenContainer.TryGetToken(WeixinWorkOptions.CorpId, WeixinWorkOptions.Secret);
            var result = MailListApi.GetDepartmentMemberInfo(token, 1, 1);
            if (result.errcode != Senparc.Weixin.ReturnCode_Work.请求成功)
            {
                throw new Exception("部门列表接口获取失败");
            }
            else
            {
                _members = result.userlist;
            }
        }

        public static List<GetMemberResult> All()
        {
            if (_members != null) return _members;

            lock (obj)
            {
                Init();
            }
            return _members;
        }

        public static GetMemberResult Get(string id)
        {
            return All().SingleOrDefault(a => a.userid == id);
        }

        public static void Clear()
        {
            _members = null;
        }

        public static void Reset()
        {
            _members = null;
            Init();
        }
    }
}
