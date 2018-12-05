using Newtonsoft.Json.Converters;

namespace HZC.Infrastructure
{
    /// <summary>
    /// JSON.NET的日期格式转换
    /// 用法：[JsonConverter(typeof(DateFormatConverter))]
    /// </summary>
    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter()
        {
            base.DateTimeFormat = "yyyy-MM-dd";
        }
    }

    /// <summary>
    /// JSON.NET的日期时间格式转换
    /// /// 用法：[JsonConverter(typeof(DateTimeFormatConverter))]
    /// </summary>
    public class DateTimeFormatConverter : IsoDateTimeConverter
    {
        public DateTimeFormatConverter()
        {
            base.DateTimeFormat = "yyyy-MM-dd HH:mm";
        }
    }
}
