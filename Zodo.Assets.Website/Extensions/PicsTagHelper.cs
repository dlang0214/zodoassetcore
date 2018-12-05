using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Zodo.Assets.Website
{
    public class PicsTagHelper: TagHelper
    {
        public string Val { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Content.Clear();
            output.Attributes.Add("class", "uploader-thumbs");

            var html = "";
            if (!string.IsNullOrWhiteSpace(Val))
            {
                var urls = Val.Split(',');
                foreach (var url in urls)
                {
                    html += "<div class='uploader-thumb'>";
                    html += "<img src='" + url + "' />";
                    html += "</div>";
                }
            }
            else
            {
                html += "<div class='empty'>暂无图片</div>";
            }
            output.Content.AppendHtml(html);
        }
    }
}
