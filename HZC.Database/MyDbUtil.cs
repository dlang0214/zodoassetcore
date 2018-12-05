using Dapper;
using HZC.Infrastructure;
using HZC.SearchUtil;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace HZC.Database
{
    public class MyDbUtil
    {
        private readonly string _connectionString;
        private readonly string _paramPrefix;

        #region 构造方法
        public MyDbUtil(string connStr, DbTypes dbType = DbTypes.SqlServer)
        {
            _connectionString = ConfigurationManager.AppSettings[connStr];
            _paramPrefix = DbTypeUtil.GetDbParamPrefix(dbType);
        }

        public MyDbUtil(DbTypes dbType = DbTypes.SqlServer)
        {
            _connectionString = ConfigurationManager.AppSettings["DefaultConnectionString"];
            _paramPrefix = DbTypeUtil.GetDbParamPrefix(dbType);
        }
        #endregion

        #region 获取一个SqlConnection
        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
        #endregion

        #region 获取查询的sql语句
        public string GetQuerySql(string cols, string table, string where, string orderby, int? top = null)
        {
            var sb = new StringBuilder();
            sb.Append("SELECT ");
            if (top.HasValue)
            {
                sb.Append("TOP ")
                    .Append(top.Value)
                    .Append(" ");
            }
            sb.Append(cols);
            sb.Append(" FROM ");
            sb.Append(table);
            sb.Append(" WHERE ");
            sb.Append(where);
            if (!string.IsNullOrWhiteSpace(orderby))
            {
                sb.Append(" ORDER BY ");
                sb.Append(orderby);
            }
            return sb.ToString();
        }

        public string GetPagingQuerySql(string cols, string tables, string condition, string orderby, int index, int size)
        {
            if (index == 1)
            {
                var sql = string.Format(
                    @"SELECT TOP {4} {0} FROM {1} WHERE {2} ORDER BY {3};SELECT " + _paramPrefix + "RecordCount=COUNT(0) FROM {1} WHERE {2}",
                    cols,
                    tables,
                    condition,
                    orderby,
                    size
                );

                return sql;
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append("FROM ").Append(tables);

                if (!string.IsNullOrWhiteSpace(condition))
                {
                    sb.Append(" WHERE ").Append(condition);
                }

                if (string.IsNullOrWhiteSpace(orderby))
                {
                    throw new Exception("分页列表必须指定orderby字段");
                }

                var sql = string.Format(
                    @"  WITH PAGEDDATA AS
					    (
						    SELECT TOP 100 PERCENT {0}, ROW_NUMBER() OVER (ORDER BY {1}) AS FLUENTDATA_ROWNUMBER
						    {2}
					    )
					    SELECT *
					    FROM PAGEDDATA
					    WHERE FLUENTDATA_ROWNUMBER BETWEEN {3} AND {4};
                        SELECT {7}RecordCount=COUNT(0) FROM {5} WHERE {6}",
                    cols,
                    orderby,
                    sb,
                    (index - 1) * size + 1,
                    index * size,
                    tables,
                    condition,
                    _paramPrefix
                );
                return sql;
            }
        }
        #endregion

        #region 获取所有数据
        public IEnumerable<T> FetchBySql<T>(string sql, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Query<T>(sql, param);
            }
        }

        public IEnumerable<T> Fetch<T>(MySearchUtil util, string table = "", string cols = "*")
        {
            if (string.IsNullOrWhiteSpace(table))
            {
                table = GetTableName(typeof(T));
            }

            var where = util.ConditionClaus;
            var orderby = util.OrderByClaus;
            var param = util.Parameters;

            var sql = GetQuerySql(cols, table, where, orderby);
            using (var conn = GetConnection())
            {
                return conn.Query<T>(sql, param);
            }
        }

        public IEnumerable<dynamic> FetchBySql(string sql, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Query(sql, param);
            }
        }

        public IEnumerable<dynamic> Fetch(MySearchUtil util, string table, string cols = "*", int? top = null)
        {
            var where = util.ConditionClaus;
            var orderby = util.OrderByClaus;
            var param = util.Parameters;

            var sql = GetQuerySql(cols, table, where, orderby);
            using (var conn = GetConnection())
            {
                return conn.Query(sql, param);
            }
        }
        #endregion

        #region 分页获取数据
        public PageList<T> Query<T>(MySearchUtil util, int pageIndex, int pageSize, string table = "", string cols = "*")
        {
            if (string.IsNullOrWhiteSpace(table))
            {
                table = GetTableName(typeof(T));
            }

            var where = util.ConditionClaus;
            var orderby = util.OrderByClaus;
            var param = util.PageListParameters;

            var sql = GetPagingQuerySql(cols, table, where, orderby, pageIndex, pageSize);
            using (var conn = GetConnection())
            {
                var list = conn.Query<T>(sql, param);
                var total = param.Get<int>("RecordCount");
                return new PageList<T>
                {
                    Body = list,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    RecordCount = total
                };
            }
        }

        public PageList<dynamic> Query(MySearchUtil util, int pageIndex, int pageSize, string table, string cols = "*")
        {
            var where = util.ConditionClaus;
            var orderby = util.OrderByClaus;
            var param = util.PageListParameters;

            var sql = GetPagingQuerySql(cols, table, where, orderby, pageIndex, pageSize);
            using (var conn = GetConnection())
            {
                var list = conn.Query(sql, param);
                var total = param.Get<int>("RecordCount");
                return new PageList<dynamic>
                {
                    Body = list,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    RecordCount = total
                };
            }
        }
        #endregion

        #region 加载实体
        /// <summary>
        /// 加载一个动态类型实体
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public dynamic LoadBySql(string sql, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Query(sql, param).FirstOrDefault();
            }
        }

        /// <summary>
        /// 使用sql语句加载实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T LoadBySql<T>(string sql, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Query<T>(sql, param).FirstOrDefault();
            }
        }

        /// <summary>
        /// 通过id加载实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public T Load<T>(int id, string cols = "*")
        {
            using (var conn = GetConnection())
            {
                var sql = "SELECT " + cols + " FROM [" + GetTableName(typeof(T)) + "] WHERE Id=" + _paramPrefix + "id";
                return conn.Query<T>(sql, new { id }).SingleOrDefault();
            }
        }

        /// <summary>
        /// 指定条件加载实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="util"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public T Load<T>(MySearchUtil util, string cols = "*")
        {
            using (var conn = GetConnection())
            {
                var sql = "SELECT TOP 1 " + cols + " FROM [" + GetTableName(typeof(T)) + "] WHERE " +
                    util.ConditionClaus + (string.IsNullOrWhiteSpace(util.OrderByClaus) ? "" : " ORDER BY " + util.OrderByClaus);
                return conn.Query<T>(sql, util.Parameters).SingleOrDefault();
            }
        }

        /// <summary>
        /// 加载实体及其导航属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TP1"></typeparam>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        /// <param name="splitOn"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T LoadJoin<T, TP1>(string sql, Func<T, TP1, T> func, string splitOn, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Query(sql, func, param, splitOn: splitOn).SingleOrDefault();
            }
        }

        /// <summary>
        /// 加载实体及其导航属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TP1"></typeparam>
        /// <typeparam name="TP2"></typeparam>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        /// <param name="splitOn"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T LoadJoin<T, TP1, TP2>(string sql, Func<T, TP1, TP2, T> func, string splitOn, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Query(sql, func, param, splitOn: splitOn).SingleOrDefault();
            }
        }

        /// <summary>
        /// 加载实体及其导航属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TP1"></typeparam>
        /// <typeparam name="TP2"></typeparam>
        /// <typeparam name="TP3"></typeparam>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        /// <param name="splitOn"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T LoadJoin<T, TP1, TP2, TP3>(string sql, Func<T, TP1, TP2, TP3, T> func, string splitOn, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Query(sql, func, param, splitOn: splitOn).SingleOrDefault();
            }
        }

        /// <summary>
        /// 加载实体及其导航属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TP1"></typeparam>
        /// <typeparam name="TP2"></typeparam>
        /// <typeparam name="TP3"></typeparam>
        /// <typeparam name="TP4"></typeparam>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        /// <param name="splitOn"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T LoadJoin<T, TP1, TP2, TP3, TP4>(string sql, Func<T, TP1, TP2, TP3, TP4, T> func, string splitOn, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Query(sql, func, param, splitOn: splitOn).SingleOrDefault();
            }
        }

        /// <summary>
        /// 加载实体及其导航属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TP1"></typeparam>
        /// <typeparam name="TP2"></typeparam>
        /// <typeparam name="TP3"></typeparam>
        /// <typeparam name="TP4"></typeparam>
        /// <typeparam name="TP5"></typeparam>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        /// <param name="splitOn"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T LoadJoin<T, TP1, TP2, TP3, TP4, TP5>(string sql, Func<T, TP1, TP2, TP3, TP4, TP5, T> func, string splitOn, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Query(sql, func, param, splitOn: splitOn).SingleOrDefault();
            }
        }


        /// <summary>
        /// 加载实体及其导航属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T LoadWith<T, T1>(string sql, Func<T, List<T1>, T> func, Object param = null)
        {
            using (var conn = GetConnection())
            {
                using (var multiReader = conn.QueryMultiple(sql, param))
                {
                    var entity = multiReader.Read<T>().SingleOrDefault();
                    var sub1S = multiReader.Read<T1>().ToList();
                    func(entity, sub1S);
                    return entity;
                }
            }
        }

        public T LoadWith<T, T1, T2>(string sql, Func<T, List<T1>, List<T2>, T> func, Object param = null)
        {
            using (var conn = GetConnection())
            {
                using (var multiReader = conn.QueryMultiple(sql, param))
                {
                    var entity = multiReader.Read<T>().SingleOrDefault();
                    var sub1S = multiReader.Read<T1>().ToList();
                    var sub2S = multiReader.Read<T2>().ToList();
                    func(entity, sub1S, sub2S);
                    return entity;
                }
            }
        }

        public T LoadWith<T, T1, T2, T3>(string sql, Func<T, List<T1>, List<T2>, List<T3>, T> func, Object param = null)
        {
            using (var conn = GetConnection())
            {
                using (var multiReader = conn.QueryMultiple(sql, param))
                {
                    var entity = multiReader.Read<T>().SingleOrDefault();
                    var sub1S = multiReader.Read<T1>().ToList();
                    var sub2S = multiReader.Read<T2>().ToList();
                    var sub3S = multiReader.Read<T3>().ToList();
                    func(entity, sub1S, sub2S, sub3S);
                    return entity;
                }
            }
        }

        public T LoadWith<T, T1, T2, T3, T4>(string sql, Func<T, List<T1>, List<T2>, List<T3>, List<T4>, T> func, object param = null)
        {
            using (var conn = GetConnection())
            {
                using (var multiReader = conn.QueryMultiple(sql, param))
                {
                    var entity = multiReader.Read<T>().SingleOrDefault();
                    var sub1S = multiReader.Read<T1>().ToList();
                    var sub2S = multiReader.Read<T2>().ToList();
                    var sub3S = multiReader.Read<T3>().ToList();
                    var sub4S = multiReader.Read<T4>().ToList();
                    func(entity, sub1S, sub2S, sub3S, sub4S);
                    return entity;
                }
            }
        }

        public T LoadWith<T, T1, T2, T3, T4, T5>(string sql, Func<T, List<T1>, List<T2>, List<T3>, List<T4>, List<T5>, T> func, object param = null)
        {
            using (var conn = GetConnection())
            {
                using (var multiReader = conn.QueryMultiple(sql, param))
                {
                    var entity = multiReader.Read<T>().SingleOrDefault();
                    var sub1S = multiReader.Read<T1>().ToList();
                    var sub2S = multiReader.Read<T2>().ToList();
                    var sub3S = multiReader.Read<T3>().ToList();
                    var sub4S = multiReader.Read<T4>().ToList();
                    var sub5S = multiReader.Read<T5>().ToList();
                    func(entity, sub1S, sub2S, sub3S, sub4S, sub5S);
                    return entity;
                }
            }
        }
        #endregion

        #region 获取数量
        public int GetCount<T>(MySearchUtil util = null, string tableName = "")
        {
            string condition;
            DynamicParameters param = null;

            if (util != null)
            {
                condition = util.ConditionClaus;
                param = util.Parameters;
            }
            else
            {
                condition = "1=1";
            }

            if (string.IsNullOrWhiteSpace(tableName))
            {
                tableName = GetTableName(typeof(T));
            }
            using (var conn = GetConnection())
            {
                return conn.ExecuteScalar<int>($"SELECT COUNT(0) FROM [{tableName}] WHERE " + condition, param);
            }
        }
        #endregion

        #region 添加实体
        public int Create<T>(T t)
        {
            using (var conn = GetConnection())
            {
                var sql = MyContainer.Get(typeof(T)).InsertSql;
                return conn.ExecuteScalar<int>(sql, t);
            }
        }

        public int Create<T>(List<T> ts)
        {
            if (ts.Count == 0) return 0;
            using (var conn = GetConnection())
            {
                conn.Open();
                var sql = MyContainer.Get(typeof(T)).InsertSql;
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var rows = conn.Execute(sql, ts, trans, 30, CommandType.Text);
                        trans.Commit();
                        return rows;
                    }
                    catch (DataException)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        #endregion

        #region 修改实体
        public int Update<T>(T t)
        {
            using (var conn = GetConnection())
            {
                var sql = MyContainer.Get(typeof(T)).UpdateSql;
                return conn.Execute(sql, t);
            }
        }

        public int Update<T>(List<T> ts)
        {
            if (ts.Count == 0) return 0;
            using (var conn = GetConnection())
            {
                conn.Open();
                var sql = MyContainer.Get(typeof(T)).UpdateSql;
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var rows = conn.Execute(sql, ts.ToArray(), trans, 30, CommandType.Text);
                        trans.Commit();
                        return rows;
                    }
                    catch (DataException)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        #endregion

        #region 修改部分字段
        public int Update<T>(KeyValuePairList cols, MySearchUtil mcu = null)
        {
            return Update<T>(cols.ToArray(), mcu);
        }

        public int Update<T>(KeyValuePair<string, object>[] cols, MySearchUtil mcu = null)
        {
            using (var conn = GetConnection())
            {
                if (mcu == null)
                {
                    mcu = new MySearchUtil();
                }

                var _params = mcu.Parameters;
                var updateCols = new List<string>();
                var idx = 0;
                foreach (var kv in cols)
                {
                    var paramName = "@c" + idx;
                    updateCols.Add(kv.Key + "=" + paramName);
                    _params.Add(paramName, kv.Value);
                    idx++;
                }
                var table = GetTableName(typeof(T));
                var sql = $"UPDATE [" + table + "] SET " + string.Join(",", updateCols) + " WHERE " + mcu.ConditionClaus;
                return conn.Execute(sql, _params);
            }
        }

        public int Update(KeyValuePairList cols, string tableName, MySearchUtil mcu = null)
        {
            return Update(cols.ToArray(), tableName, mcu);
        }

        public int Update(KeyValuePair<string, object>[] cols, string table, MySearchUtil mcu = null)
        {
            using (var conn = GetConnection())
            {
                if (mcu == null)
                {
                    mcu = new MySearchUtil();
                }

                var _params = mcu.Parameters;
                var updateCols = new List<string>();
                var idx = 0;
                foreach (var kv in cols)
                {
                    var paramName = _paramPrefix + "c" + idx;
                    updateCols.Add(kv.Key + "=" + paramName);
                    _params.Add(paramName, kv.Value);
                    idx++;
                }
                var sql = "UPDATE [" + table + "] SET " + string.Join(",", updateCols) + " WHERE " + mcu.ConditionClaus;
                return conn.Execute(sql, _params);
            }
        }
        #endregion

        #region 删除实体
        public int Delete<T>(int id, string tableName = "")
        {
            using (var conn = GetConnection())
            {
                if (string.IsNullOrWhiteSpace(tableName))
                {
                    tableName = GetTableName(typeof(T));
                }
                return conn.Execute(string.Format(@"DELETE [{0}] WHERE Id=" + _paramPrefix + "id", tableName), new {id });
            }
        }

        public int Delete<T>(int[] ids, string tableName = "")
        {
            using (var conn = GetConnection())
            {
                if (string.IsNullOrWhiteSpace(tableName))
                {
                    tableName = GetTableName(typeof(T));
                }

                return conn.Execute(string.Format(@"DELETE [{0}] WHERE Id in @ids", tableName), new {ids });
            }
        }

        public int Delete<T>(MySearchUtil mcu, string tableName = "")
        {
            using (var conn = GetConnection())
            {
                if (mcu == null)
                {
                    mcu = new MySearchUtil();
                }

                if (string.IsNullOrWhiteSpace(tableName))
                {
                    tableName = GetTableName(typeof(T));
                }

                return conn.Execute(string.Format(@"DELETE [{0}] WHERE {1}", tableName, mcu.ConditionClaus), mcu.Parameters);
            }
        }

        public int Remove<T>(int id, string tableName = "")
        {
            using (var conn = GetConnection())
            {
                if (string.IsNullOrWhiteSpace(tableName))
                {
                    tableName = GetTableName(typeof(T));
                }
                return conn.Execute(string.Format(@"UPDATE [{0}] SET IsDel=1 WHERE Id=" + _paramPrefix + "id", tableName), new {id });
            }
        }

        public int Remove<T>(int[] ids, string tableName = "")
        {
            using (var conn = GetConnection())
            {
                if (string.IsNullOrWhiteSpace(tableName))
                {
                    tableName = GetTableName(typeof(T));
                }
                return conn.Execute(string.Format(@"UPDATE [{0}] SET IsDel=1 WHERE Id in @ids", tableName), new {ids });
            }
        }

        public int Remove<T>(MySearchUtil mcu, string tableName = "")
        {
            using (var conn = GetConnection())
            {
                if (mcu == null)
                {
                    mcu = new MySearchUtil();
                }

                if (string.IsNullOrWhiteSpace(tableName))
                {
                    tableName = GetTableName(typeof(T));
                }

                return conn.Execute(string.Format(@"UPDATE [{0}] SET IsDel=1 WHERE ", tableName, mcu.ConditionClaus), mcu.Parameters);
            }
        }
        #endregion

        #region 执行sql操作
        /// <summary>
        /// 执行sql语句，返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int Execute(string sql, Object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Execute(sql, param);
            }
        }

        /// <summary>
        /// 执行sql并获取第一行第一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string sql, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.ExecuteScalar<T>(sql, param);
            }
        }

        /// <summary>
        /// 执行存储过程，返回受影响的行数
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int ExecuteProc(string procName, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Execute(procName, param, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// 执行存储过程，返回第一行第一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T ExecuteProc<T>(string procName, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.ExecuteScalar<T>(procName, param, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// 以事务的方式执行一组sql语句
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public bool ExecuteTran(string[] sqls)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var s in sqls)
                        {
                            conn.Execute(s, null, tran);
                        }
                        tran.Commit();
                        return true;
                    }
                    catch
                    {
                        tran.Rollback();
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 执行一组带参数的sql语句
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public bool ExecuteTran(KeyValuePairList sqls)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var s in sqls)
                        {
                            conn.Execute(s.Key, s.Value, tran);
                        }
                        tran.Commit();
                        return true;
                    }
                    catch
                    {
                        tran.Rollback();
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 执行sql，返回多个列表
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>> MultiQuery<T1, T2>(string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var conn = GetConnection())
            {
                using (var multi = conn.QueryMultiple(sql, param, commandType: commandType))
                {
                    var list1 = multi.Read<T1>();
                    var list2 = multi.Read<T2>();

                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(list1, list2);
                }
            }
        }

        /// <summary>
        /// 执行sql，返回多个列表
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> MultiQuery<T1, T2, T3>(string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var conn = GetConnection())
            {
                using (var multi = conn.QueryMultiple(sql, param, commandType: commandType))
                {
                    var list1 = multi.Read<T1>();
                    var list2 = multi.Read<T2>();
                    var list3 = multi.Read<T3>();

                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>(list1, list2, list3);
                }
            }
        }

        /// <summary>
        /// 执行sql，返回多个列表
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> MultiQuery<T1, T2, T3, T4>(string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var conn = GetConnection())
            {
                using (var multi = conn.QueryMultiple(sql, param, commandType: commandType))
                {
                    var list1 = multi.Read<T1>();
                    var list2 = multi.Read<T2>();
                    var list3 = multi.Read<T3>();
                    var list4 = multi.Read<T4>();

                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>(list1, list2, list3, list4);
                }
            }
        }

        /// <summary>
        /// 执行sql，返回多个列表
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> MultiQuery<T1, T2, T3, T4, T5>(string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var conn = GetConnection())
            {
                using (var multi = conn.QueryMultiple(sql, param, commandType: commandType))
                {
                    var list1 = multi.Read<T1>();
                    var list2 = multi.Read<T2>();
                    var list3 = multi.Read<T3>();
                    var list4 = multi.Read<T4>();
                    var list5 = multi.Read<T5>();

                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>(list1, list2, list3, list4, list5);
                }
            }
        }
        #endregion

        #region 其他
        public string GetCommonInsertSql<T>()
        {
            try
            {
                return MyContainer.Get(typeof(T)).InsertSql;
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetCommonUpdateSql<T>()
        {
            try
            {
                return MyContainer.Get(typeof(T)).UpdateSql;
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion

        #region 私有方法
        private string GetTableName(Type type)
        {
            var entityInfo = MyContainer.Get(type);
            return entityInfo.TableName;
        }
        #endregion
    }

    public class KeyValuePairList : List<KeyValuePair<string, object>>
    {
        public static KeyValuePairList New()
        {
            return new KeyValuePairList();
        }

        public KeyValuePairList Add(string column, object value)
        {
            Add(new KeyValuePair<string, object>(column, value));
            return this;
        }
    }
}
