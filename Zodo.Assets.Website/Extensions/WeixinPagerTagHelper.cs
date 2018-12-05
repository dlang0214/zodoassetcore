using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace Zodo.Assets.Website
{
    public class WeixinPagerTagHelper : TagHelper
    {
        public int Total { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string BaseUrl { get; set; }

        public string Query { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.Add("class", "weui-flex");

            var pages = Total / PageSize + (Total % PageSize == 0 ? 0 : 1);

            var sb = new StringBuilder();

            PageIndex = PageIndex <= 0 ? 1 : PageIndex;
            PageIndex = PageIndex > pages ? pages : PageIndex;

            if (PageIndex > 1)
            {
                sb.Append("<div class=\"weui-flex__item\" style=\"text-align: left\"><a href=\"" + string.Format(@"{0}{1}{2}", BaseUrl, PageIndex - 1, Query) + "\" class=\"weui-btn weui-btn_mini weui-btn_default\">上一页</a></div>");
            }
            else
            {
                sb.Append("<div class=\"weui-flex__item\" style=\"text-align: left\"><a href=\"javascript:;\" class=\"weui-btn weui-btn_mini weui-btn_default weui-btn_plain-disabled\">上一页</a></div>");
            }

            sb.Append("<div class=\"weui-flex__item\" style=\"text-align: center\">" + string.Format("{0}/{1}", PageIndex, pages) + "</div>");

            if (PageIndex < pages)
            {
                sb.Append("<div class=\"weui-flex__item\" style=\"text-align: right\"><a href=\"" + string.Format(@"{0}{1}{2}", BaseUrl, PageIndex + 1, Query) + "\" class=\"weui-btn weui-btn_mini weui-btn_default\">下一页</a></div>");
            }
            else
            {
                sb.Append("<div class=\"weui-flex__item\" style=\"text-align: right\"><a href=\"javascript:;\" class=\"weui-btn weui-btn_mini weui-btn_default weui-btn_plain-disabled\">下一页</a></div>");
            }

            output.Content.AppendHtml(sb.ToString());
        }
    }
}
