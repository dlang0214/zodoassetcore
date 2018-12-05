using HZC.Infrastructure;
using HZC.SearchUtil;
using System.Collections.Generic;
using System.Linq;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public class MenuService : BaseService<Menu>
    {
        public List<MenuDto> FetchDto()
        {
            return db.FetchBySql<MenuDto>("SELECT Id,Name,ParentId,Icon,Url,Sort FROM Base_Menu WHERE IsDel=0").ToList();
        }

        public override string ValidCreate(Menu entity, IAppUser user)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                return "菜单名称不能为空";
            }

            if (string.IsNullOrWhiteSpace(entity.Icon))
            {
                return "菜单图标不能为空";
            }

            return string.Empty;
        }

        public override string ValidDelete(Menu entity, IAppUser user)
        {
            var count = db.GetCount<Menu>(MySearchUtil.New()
                .AndEqual("IsDel", false)
                .AndEqual("ParentId", entity.Id));
            return count > 0 ? "下级菜单不为空，禁止删除" : string.Empty;
        }

        public override string ValidUpdate(Menu entity, IAppUser user)
        {
            return ValidCreate(entity, user);
        }
    }
}
