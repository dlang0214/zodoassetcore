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
    public class WeixinDeptUtil2
    {
        private CacheHelper _cacheHelper;

        public WeixinDeptUtil2(IDistributedCache cache)
        {
            _cacheHelper = new CacheHelper(cache);
        }

        public List<DepartmentList> All()
        {
            var result = _cacheHelper.Get<List<DepartmentList>>("WeixinDepartments");
            if (result == null)
            {
                return Init();
            }
            return result;
        }

        public DepartmentList Get(int id)
        {
            return All().Where(d => d.id == id).FirstOrDefault();
        }

        public void Reset()
        {
            Init();
        }

        private List<DepartmentList> Init()
        {
            var token = AccessTokenContainer.TryGetToken(WeixinWorkOptions.CorpId, WeixinWorkOptions.Secret);
            var result = MailListApi.GetDepartmentList(token);
            if (result.errcode != Senparc.Weixin.ReturnCode_Work.请求成功)
            {
                throw new Exception("部门列表接口获取失败");
            }
            else
            {
                //_cache.Set("WeixinDepartments", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result.department)));
                _cacheHelper.Set<List<DepartmentList>>("WeixinDepartments", result.department);
                return result.department;
            }
        }
    }
}
