using System;
using System.Collections.Generic;

namespace HZC.Infrastructure
{
    public static class ResultUtil
    {
        #region 操作成功
        public static Result Success(string msg = "")
        {
            return new Result((int)ResultCodes.操作成功, msg);
        }

        public static Result<T> Success<T>(T body, string msg = "")
        {
            return new Result<T>((int)ResultCodes.操作成功, body, msg);
        }
        #endregion

        #region 通用操作
        public static Result Do(ResultCodes code, string msg = "")
        {
            return new Result((int)code, msg);
        }

        public static Result<T> Do<T>(ResultCodes code, T body, string msg = "")
        {
            return new Result<T>((int)code, body, msg);
        }
        #endregion

        #region 系统异常
        public static Result Exception(Exception ex)
        {
            return new Result((int)ResultCodes.系统异常, ex.Message + ex.StackTrace);
        }

        public static Result<T> Exception<T>(Exception ex, T body)
        {
            return new Result<T>((int)ResultCodes.系统异常, body, ex.Message + ex.StackTrace);
        }
        #endregion

        #region 分页列表结果
        public static PageListResult<T> PageList<T>(PageList<T> body, string message = "")
        {
            return new PageListResult<T>
            {
                Code = 200,
                Message = message,
                PageIndex = body.PageIndex,
                PageSize = body.PageSize,
                RecordCount = body.RecordCount,
                Body = body.Body
            };
        }

        public static PageListResult<T> PageList<T>(IEnumerable<T> body, int recordCount, int pageIndex, int pageSize, string message = "")
        {
            return new PageListResult<T>
            {
                Code = 200,
                Message = message,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Body = body,
                RecordCount = recordCount
            };
        }
        #endregion
    }
}
