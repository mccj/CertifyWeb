using SqlSugar;

namespace WeiXin.Application.AppService
{
    public static class SqlSugarRepositoryExtensions
    {
        public static Task<DbResult<T>> UseTranAsync<TEntity, T>(this SimpleClient<TEntity> sqlSugarRepository, Func<Task<T>> action, Action<Exception>? errorCallBack = null) where TEntity : class, new()
        {
            return sqlSugarRepository.Context.Ado.UseTranAsync(action, errorCallBack);
        }

        public static void InitTable<TEntity>(this SimpleClient<TEntity> sqlSugarRepository) where TEntity : class, new()
        {
            var diffString = sqlSugarRepository.Context.CodeFirst.GetDifferenceTables<TEntity>().ToDiffString();
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"InitTable:{diffString}");
            Console.ResetColor();
            sqlSugarRepository.Context.CodeFirst//.SetStringDefaultLength(200)
                .InitTables<TEntity>();
        }
    }
}
