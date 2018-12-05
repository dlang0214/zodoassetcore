using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zodo.Assets.Website
{
    public static class SelectListItemExtension
    {
        /// <summary>
        /// 将文本数组转换为selectlistitem列表
        /// </summary>
        /// <param name="data">数组元素</param>
        /// <param name="value">选中值</param>
        /// <param name="firstOption">说明项，注意生成的option的val为空</param>
        /// <returns></returns>
        public static List<SelectListItem> ToSelectList(this string[] data, string value = null, string firstOption = null)
        {
            var result = new List<SelectListItem>();

            if (!string.IsNullOrWhiteSpace(firstOption))
            {
                result.Add(new SelectListItem { Text = firstOption, Value = string.Empty });
            }

            result.AddRange(data.Select(d => new SelectListItem {Text = d, Value = d, Selected = d == value}));

            return result;
        }

        /// <summary>
        /// List转换为SelectListItemList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">数据源</param>
        /// <param name="valueProp">value字段（属性）名称</param>
        /// <param name="textProp">text字段（属性）名称</param>
        /// <returns></returns>
        public static List<SelectListItem> ToSelectList<T>(this List<T> data, string valueProp, string textProp)
        {
            var items = new List<SelectListItem>();

            if (data == null || data.Count == 0)
            {
                return items;
            }

            var props = typeof(T).GetProperties();
            var vProp = props.SingleOrDefault(p => p.Name == valueProp);
            var tProp = props.SingleOrDefault(p => p.Name == textProp);

            if (vProp == null || tProp == null)
            {
                throw new ArgumentException("指定的字段名不存在");
            }
            
            foreach (var t in data)
            {
                var text = tProp.GetValue(t).ToString();
                var value = vProp.GetValue(t).ToString();
                
                items.Add(new SelectListItem { Value = value, Text = text });
            }
            return items;
        }

        /// <summary>
        /// List转换为SelectListItemList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">数据源</param>
        /// <param name="valueProp">value字段（属性）名称</param>
        /// <param name="textProp">text字段（属性）名称</param>
        /// <param name="selectedValue">选中值</param>
        /// <returns></returns>
        public static List<SelectListItem> ToSelectList<T>(this List<T> data, string valueProp, string textProp, string selectedValue)
        {
            if (data == null || data.Count == 0)
            {
                return new List<SelectListItem>();
            }

            var props = typeof(T).GetProperties();
            var vProp = props.SingleOrDefault(p => p.Name == valueProp);
            var tProp = props.SingleOrDefault(p => p.Name == textProp);

            if (vProp == null || tProp == null)
            {
                throw new ArgumentException("指定的字段名不存在");
            }

            var items = new List<SelectListItem>();
            foreach (var t in data)
            {
                var text = tProp.GetValue(t).ToString();
                var value = vProp.GetValue(t).ToString();
                items.Add(selectedValue == value
                    ? new SelectListItem {Value = value, Text = text, Selected = true}
                    : new SelectListItem {Value = value, Text = text});
            }
            return items;
        }
    }
}
