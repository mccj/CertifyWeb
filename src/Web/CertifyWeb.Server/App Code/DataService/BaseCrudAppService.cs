using Admin.NET.Core;
using AutoGeneratorModes;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Linq.Expressions;
using System.Reflection;
using WeiXin.Application.AppService;

namespace AutoGeneratorService;

/// <summary>
/// 
/// </summary>
/// <param name="serviceProvider"></param>
public abstract class BaseService(IServiceProvider serviceProvider)
{
    private readonly IMapper _mapper = serviceProvider.GetRequiredService<IMapper>();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDestination"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    protected TDestination? DependencyMap<TDestination>(object? source)
    {
        if (source == null) return default;
        if (_mapper != null) return _mapper.Map<TDestination>(source);
        else return source.Adapt<TDestination>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDestination"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    protected async Task<TDestination> DependencyMapAsync<TDestination>(object? source)
    {
        if (_mapper != null)
            return await _mapper.From(source).AdaptToTypeAsync<TDestination>();
        else return await source.BuildAdapter().AdaptToTypeAsync<TDestination>();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"></param>
    /// <returns></returns>
    protected ISugarQueryable<TEntity> HandleSplitTable<TEntity>(ISugarQueryable<TEntity> queryable)
    {
        if (typeof(TEntity).GetCustomAttributes<SplitTableAttribute>().Any())
        {
            return queryable.SplitTable();
        }
        else
        {
            return queryable;
        }
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TAdd"></typeparam>
/// <typeparam name="TUpdate"></typeparam>
/// <typeparam name="TGet"></typeparam>
public abstract class BaseCrudAppService<TEntity, TAdd, TUpdate, TGet>(IServiceProvider serviceProvider) : BaseService(serviceProvider) where TEntity : class, IEntity<long>, new()
{
    /// <summary>
    /// 
    /// </summary>
    protected internal readonly SimpleClient<TEntity> _repository = serviceProvider.GetRequiredService<SimpleClient<TEntity>>();

    /// <summary>
    /// 增加
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual Task<long> CreateAsync([FromBody] TAdd input) => createByTranAsync(input);

    /// <summary>
    /// 事务增加
    /// </summary>
    /// <param name="input"></param>
    /// <param name="beforeTranAction">事务前</param>
    /// <param name="beforeTranExecAction">执行前</param>
    /// <param name="afterTranExecAction">执行后</param>
    /// <param name="afterTranAction">事务后</param>
    /// <returns></returns>
    protected virtual async Task<long> createByTranAsync(TAdd input, Func<TEntity, Task>? beforeTranAction = null, Func<TEntity, Task>? beforeTranExecAction = null, Func<CreatUpdateTranResult<TEntity, long>, Task>? afterTranExecAction = null, Func<DbResult<CreatUpdateTranResult<TEntity, long>>, Task>? afterTranAction = null)
    {
        var entity = await DependencyMapAsync<TEntity>(input);
        if (entity == null) throw new ArgumentNullException(nameof(input));
        await (beforeTranAction?.Invoke(entity) ?? Task.CompletedTask);
        var dbResult = await _repository.UseTranAsync(async () =>
        {
            await (beforeTranExecAction?.Invoke(entity) ?? Task.CompletedTask);
            var result = await _repository.InsertReturnSnowflakeIdAsync(entity);
            var resultEx = new CreatUpdateTranResult<TEntity, long>(entity, result, result > 0);
            await (afterTranExecAction?.Invoke(resultEx) ?? Task.CompletedTask);
            return resultEx;
        });
        await (afterTranAction?.Invoke(dbResult) ?? Task.CompletedTask);
        if (dbResult.IsSuccess) return dbResult.Data.ResultValue;
        else throw dbResult.ErrorException;
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPut]
    public virtual Task<bool> UpdateAsync([FromBody] TUpdate input) => updateByTranAsync(input);

    /// <summary>
    /// 事务更新
    /// </summary>
    /// <param name="input"></param>
    /// <param name="ignoreUpdateColumns"></param>
    /// <param name="isIgnoreNull"></param>
    /// <param name="beforeTranAction">事务前</param>
    /// <param name="beforeTranExecAction">执行前</param>
    /// <param name="afterTranExecAction">执行后</param>
    /// <param name="afterTranAction">事务后</param>
    /// <returns></returns>
    protected virtual async Task<bool> updateByTranAsync(TUpdate input, Expression<Func<TEntity, object>>? ignoreUpdateColumns = null, bool isIgnoreNull = true, Func<TEntity, Task>? beforeTranAction = null, Func<TEntity, Task>? beforeTranExecAction = null, Func<CreatUpdateTranResult<TEntity, int>, Task>? afterTranExecAction = null, Func<DbResult<CreatUpdateTranResult<TEntity, int>>, Task>? afterTranAction = null)
    {
        var entity = await DependencyMapAsync<TEntity>(input);
        if (entity == null) throw new ArgumentNullException(nameof(input));
        await (beforeTranAction?.Invoke(entity) ?? Task.CompletedTask);
        var dbResult = await _repository.UseTranAsync(async () =>
        {
            await (beforeTranExecAction?.Invoke(entity) ?? Task.CompletedTask);
            var result = await _repository.AsUpdateable(entity).IgnoreNullColumns(isIgnoreNull).IgnoreColumnsIF(ignoreUpdateColumns != null, ignoreUpdateColumns).ExecuteCommandAsync();
            var resultEx = new CreatUpdateTranResult<TEntity, int>(entity, result, result > 0);
            await (afterTranExecAction?.Invoke(resultEx) ?? Task.CompletedTask);
            return resultEx;
        });
        await (afterTranAction?.Invoke(dbResult) ?? Task.CompletedTask);
        if (dbResult.IsSuccess) return dbResult.Data.Result;
        else throw dbResult.ErrorException;
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    public virtual Task<bool> DeleteAsync([FromQuery] long id) => deleteByTranAsync(id);

    /// <summary>
    /// 事务删除
    /// </summary>
    /// <param name="id"></param>
    /// <param name="beforeTranAction">事务前</param>
    /// <param name="beforeTranExecAction">执行前</param>
    /// <param name="afterTranExecAction">执行后</param>
    /// <param name="afterTranAction">事务后</param>
    /// <returns></returns>
    protected virtual async Task<bool> deleteByTranAsync(long id, Func<Task>? beforeTranAction = null, Func<Task>? beforeTranExecAction = null, Func<bool, Task>? afterTranExecAction = null, Func<DbResult<bool>, Task>? afterTranAction = null)
    {
        await (beforeTranAction?.Invoke() ?? Task.CompletedTask);
        var dbResult = await _repository.UseTranAsync(async () =>
        {
            await (beforeTranExecAction?.Invoke() ?? Task.CompletedTask);
            var result = await _repository.DeleteByIdAsync(id);
            await (afterTranExecAction?.Invoke(result) ?? Task.CompletedTask);
            return result;
        });
        await (afterTranAction?.Invoke(dbResult) ?? Task.CompletedTask);
        if (dbResult.IsSuccess) return dbResult.Data;
        else throw dbResult.ErrorException;
    }

    /// <summary>
    /// 批量删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    //[QueryParameters]
    [HttpDelete]
    public virtual Task<bool> DeleteManyAsync([FromQuery] long[] ids) => deleteManyByTranAsync(ids);

    /// <summary>
    /// 事务批量删除
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="beforeTranAction">事务前</param>
    /// <param name="beforeTranExecAction">执行前</param>
    /// <param name="afterTranExecAction">执行后</param>
    /// <param name="afterTranAction">事务后</param>
    /// <returns></returns>
    protected virtual async Task<bool> deleteManyByTranAsync(long[] ids, Func<Task>? beforeTranAction = null, Func<Task>? beforeTranExecAction = null, Func<bool, Task>? afterTranExecAction = null, Func<DbResult<bool>, Task>? afterTranAction = null)
    {
        await (beforeTranAction?.Invoke() ?? Task.CompletedTask);
        var dbResult = await _repository.UseTranAsync(async () =>
        {
            await (beforeTranExecAction?.Invoke() ?? Task.CompletedTask);
            var result = await _repository.DeleteByIdAsync(ids);
            await (afterTranExecAction?.Invoke(result) ?? Task.CompletedTask);
            return result;
        });
        await (afterTranAction?.Invoke(dbResult) ?? Task.CompletedTask);
        if (dbResult.IsSuccess) return dbResult.Data;
        else throw dbResult.ErrorException;
    }

    /// <summary>
    /// 详细
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public virtual async Task<TGet?> GetDetailAsync([FromQuery] long id)
    {
        var result = await _repository.AsQueryable().IncludesAllFirstLayer().FirstAsync(x => x.Id == id);
        var response = await DependencyMapAsync<TGet>(result);
        await ReSetResultObject(response);
        return response;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    protected internal virtual Task ReSetResultObject(TGet result) { return Task.CompletedTask; }

#if DEBUG
    /// <summary>
    /// 初始化表格
    /// </summary>
    [Obsolete("仅开发环境使用")]
    [HttpPatch]
    public virtual void InitTable()
    {
        _repository.InitTable();
    }
#endif
}

public abstract class BaseCrudAppReadOnlyService<TEntity, TGet, TSearch>(IServiceProvider serviceProvider) : BaseCrudAppReadOnlyService<TEntity, TGet, TGet, TSearch>(serviceProvider) where TEntity : class, IEntity<long>, new()
{
}
public abstract class BaseCrudAppReadOnlyService<TEntity, TGet, TList, TSearch>(IServiceProvider serviceProvider) : BaseService(serviceProvider) where TEntity : class, IEntity<long>, new()
{
    protected internal readonly SimpleClient<TEntity> _repository = serviceProvider.GetRequiredService<SimpleClient<TEntity>>();
    /// <summary>
    /// 列表
    /// </summary>
    /// <param name="search">搜索条件</param>
    /// <returns></returns>
    //[QueryParameters]
    [HttpGet]
    public virtual async Task<IEnumerable<TList>> GetListAsync([FromQuery] TSearch search)
    {
        var result = await GetSearchQueryable(HandleSplitTable(_repository.AsQueryable()), search).ToListAsync();
        var response = await DependencyMapAsync<TList[]>(result);
        await ReSetResultObject(response);
        return response;
    }

    /// <summary>
    /// 分页
    /// </summary>
    /// <param name="search">搜索条件</param>
    /// <param name="pageNo">当前页码</param>
    /// <param name="pageSize">页码容量</param>
    /// <returns></returns>
    //[QueryParameters]
    [HttpGet]
    public virtual async Task<SqlSugarPagedList<TList>> GetPageAsync([FromQuery] TSearch search, [FromQuery] int pageNo = 1, [FromQuery] int pageSize = 20)
    {
        var entities = await GetSearchQueryable(HandleSplitTable(_repository.AsQueryable()), search).ToPagedListAsync(pageNo, pageSize);
        var response = await DependencyMapAsync<SqlSugarPagedList<TList>>(entities);
        await ReSetResultObject(response.Items);
        return response;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="search"></param>
    /// <returns></returns>
    protected internal abstract ISugarQueryable<TEntity> GetSearchQueryable(ISugarQueryable<TEntity> queryable, TSearch? search);
    /// <summary>
    /// 详细
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public virtual async Task<TGet?> GetDetailAsync([FromQuery] long id)
    {
        var result = await HandleSplitTable(_repository.AsQueryable()).IncludesAllFirstLayer().FirstAsync(x => x.Id == id);
        var response = await DependencyMapAsync<TGet>(result);
        await ReSetResultObject(response);
        return response;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="lists"></param>
    protected internal virtual Task ReSetResultObject(TList[] lists) { return Task.FromResult(lists); }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    protected internal virtual Task ReSetResultObject(TGet result) { return Task.CompletedTask; }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TSearch"></typeparam>
/// <typeparam name="TAdd"></typeparam>
/// <typeparam name="TUpdate"></typeparam>
/// <typeparam name="TGet"></typeparam>
/// <typeparam name="TList"></typeparam>
public abstract class BaseCrudAppService<TEntity, TAdd, TUpdate, TGet, TList, TSearch>(IServiceProvider serviceProvider) : BaseCrudAppService<TEntity, TAdd, TUpdate, TGet>(serviceProvider) where TEntity : class, IEntity<long>, new()
{
    /// <summary>
    /// 列表
    /// </summary>
    /// <param name="search">搜索条件</param>
    /// <returns></returns>
    //[QueryParameters]
    [HttpGet]
    public virtual async Task<IEnumerable<TList>> GetListAsync([FromQuery] TSearch search)
    {
        var result = await GetSearchQueryable(_repository.AsQueryable(), search).ToListAsync();
        var response = await DependencyMapAsync<TList[]>(result);
        await ReSetResultObject(response);
        return response;
    }

    /// <summary>
    /// 分页
    /// </summary>
    /// <param name="search">搜索条件</param>
    /// <param name="pageNo">当前页码</param>
    /// <param name="pageSize">页码容量</param>
    /// <returns></returns>
    //[QueryParameters]
    [HttpGet]
    public virtual async Task<SqlSugarPagedList<TList>> GetPageAsync([FromQuery] TSearch search, [FromQuery] int pageNo = 1, [FromQuery] int pageSize = 20)
    {
        var entities = await GetSearchQueryable(_repository.AsQueryable(), search).ToPagedListAsync(pageNo, pageSize);
        var response = await DependencyMapAsync<SqlSugarPagedList<TList>>(entities);
        await ReSetResultObject(response.Items);
        return response;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="search"></param>
    /// <returns></returns>
    protected internal abstract ISugarQueryable<TEntity> GetSearchQueryable(ISugarQueryable<TEntity> queryable, TSearch? search);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="lists"></param>
    protected internal virtual Task ReSetResultObject(TList[] lists) { return Task.FromResult(lists); }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TAdd"></typeparam>
/// <typeparam name="TUpdate"></typeparam>
/// <typeparam name="TGet"></typeparam>
/// <typeparam name="TSearch"></typeparam>
/// <param name="serviceProvider"></param>
public abstract class BaseCrudAppService<TEntity, TAdd, TUpdate, TGet, TSearch>(IServiceProvider serviceProvider) : BaseCrudAppService<TEntity, TAdd, TUpdate, TGet, TGet, TSearch>(serviceProvider) where TEntity : class, IEntity<long>, new()
{
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TCount"></typeparam>
/// <param name="entity"></param>
/// <param name="resultValue"></param>
/// <param name="result"></param>
public class CreatUpdateTranResult<TEntity, TCount>(TEntity entity, TCount resultValue, bool result)
{
    /// <summary>
    /// Entity 模型
    /// </summary>
    public TEntity Entity { get; } = entity;
    /// <summary>
    /// 影响的行数
    /// </summary>
    public TCount ResultValue { get; } = resultValue;
    /// <summary>
    /// 是否完成更新
    /// </summary>
    public bool Result { get; } = result;
}