using Microsoft.Extensions.Caching.Distributed;
using Senparc.Weixin.Work.AdvancedAPIs;
using Senparc.Weixin.Work.AdvancedAPIs.MailList;
using Senparc.Weixin.Work.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using Zodo.Assets.Website.Options;

namespace Zodo.Assets.Website.Extensions
{
    public class WeixinUserUtil2
    {
        private readonly CacheHelper _cacheHelper;

        public WeixinUserUtil2(IDistributedCache cache)
        {
            _cacheHelper = new CacheHelper(cache);
        }

        public List<GetMemberResult> All()
        {
            var result = _cacheHelper.Get<List<GetMemberResult>>("WeixinUsers");
            return result ?? Init();
        }

        public GetMemberResult Get(string id)
        {
            return All().FirstOrDefault(d => d.userid == id);
        }

        public void Reset()
        {
            Init();
        }

        private List<GetMemberResult> Init()
        {
            var token = AccessTokenContainer.TryGetToken(WeixinWorkOptions.CorpId, WeixinWorkOptions.Secret);
            var result = MailListApi.GetDepartmentMemberInfo(token, 1, 1);
            if (result.errcode != Senparc.Weixin.ReturnCode_Work.请求成功)
            {
                throw new Exception("用户列表接口获取失败");
            }
            else
            {
                _cacheHelper.Set("WeixinUsers", result.userlist);
                return result.userlist;
            }
        }
    }
}
