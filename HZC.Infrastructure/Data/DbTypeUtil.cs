using System;

namespace HZC.Infrastructure
{
    public static class DbTypeUtil
    {
        public static string GetDbParamPrefix(DbTypes dbType)
        {
            switch (dbType)
            {
                case DbTypes.SqlServer:
                    return "@";
                case DbTypes.Oracle:
                    return ":";
                case DbTypes.MySql:
                    return "?";
                default:
                    throw new Exception("不受支持的数据库类型");
            }
        }
    }
}
