using SqlSugar;

public static class SqlSugarHelper
{
    public static ISqlSugarClient GetSqlSugarClient(DbType dbType, string connectionString)
    {

        //var connectionString = "Data Source=192.168.x.x;Initial Catalog=xxx;User ID=sa;PWD=xxx";
        if (string.IsNullOrWhiteSpace(connectionString)) throw new Exception("连接字符串不能空");

        var _client = SqlSugarUtility.GetScopeSqlSugarClient<IgnoreAttribute>(dbType, connectionString, true, db =>
        {
            //(A)全局生效配置点，一般AOP和程序启动的配置扔这里面 ，所有上下文生效
            //调试SQL事件，可以删掉
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
#if DEBUG
                //Console.WriteLine(sql);//输出sql,查看执行sql 性能无影响

                //5.0.8.2 获取无参数化 SQL  对性能有影响，特别大的SQL参数多的，调试使用
                var sql2 = UtilMethods.GetSqlString(db.CurrentConnectionConfig.DbType, sql, pars);
                Console.WriteLine(sql2);//输出sql,查看执行sql 性能无影响
                System.Diagnostics.Debug.WriteLine(sql2);
#endif
            };
            db.Aop.OnError = (ex) =>
            {
                Console.WriteLine(ex.Message);//输出sql,查看执行sql 性能无影响
                System.Diagnostics.Debug.Fail(ex.Message);
            };
            db.Aop.OnDiffLogEvent = (diff) =>
            {

            };
        });
        return _client;
    }
}
public class IgnoreAttribute : Attribute { }