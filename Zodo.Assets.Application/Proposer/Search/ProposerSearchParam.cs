using HZC.SearchUtil;
using Zodo.Assets.Application;

namespace Zodo.Assets.Services
{
    public partial class ProposerSearchParam : ISearchParam
    {
        public string Key { get; set; }

        public int? ProposerId { get; set; }

        public int? ApproverId { get; set; }

        public MySearchUtil ToSearchUtil()
        {
            MySearchUtil util = MySearchUtil.New().OrderByDesc("Id");

            if (!string.IsNullOrWhiteSpace(Key))
            {
                // util.AndContains(new string[] { "Title", "Name" }, Key.Trim());
            }

            if (ProposerId.HasValue)
            {
                util.AndEqual("ProposerId", ProposerId.Value);
            }

            if (ApproverId.HasValue)
            {
                util.AndEqual("ApproverId", ApproverId.Value);
            }

            return util;
        }
    }
}
