using C = AutoGeneratorModes.AutoGeneratorConst;

namespace AutoGeneratorModes
{
    public interface BaseEntity<T>
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [AutoCodeGenerator.AutoCodeProperty(Id = C.EntityId, Attributes = [C.SqlSugarColumnPrimaryKey])]
        [AutoCodeGenerator.AutoCodeProperty(Ids = [C.UpdateId, C.DetailId])]
        public T Id { get; set; }
    }
    public interface BaseCreatedTimeEntity
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        [AutoCodeGenerator.AutoCodeProperty(Ids = new[] { C.EntityId, C.DetailId })]
        [AutoCodeGenerator.AutoCodeProperty(Ids = new[] { C.SearchId }, PropertyType = typeof(SearchRangeForStructValue<DateTime>))]
        public DateTime? CreatedTime { get; set; }
    }
    public interface BaseUpdatedTimeEntity
    {
        /// <summary>
        /// 更新时间
        /// </summary>
        [AutoCodeGenerator.AutoCodeProperty(Ids = new[] { C.EntityId, C.DetailId })]
        [AutoCodeGenerator.AutoCodeProperty(Ids = new[] { C.SearchId }, PropertyType = typeof(SearchRangeForStructValue<DateTime>))]
        public DateTime? UpdatedTime { get; set; }
    }
    public interface BaseCreatedUserEntity
    {
        ///// <summary>
        ///// 创建人
        ///// </summary>
        //[AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DetailId, C.SearchId], Suffix = "Id")]
        //[AutoCodeGenerator.AutoCodeProperty(Ids = [C.SearchId], Suffix = "Name", PropertyType = typeof(string))]
        //[AutoCodeGenerator.AutoCodeProperty(Ids = [C.DetailId], PropertyType = typeof(UserOutput))]
        //[AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId], PropertyType = typeof(SysUser), Attributes = [$"Navigate(NavigateType.ManyToOne, nameof({nameof(CreatedUser)}Id))"])]
        //public long? CreatedUser { get; set; }
    }
    public interface BaseUpdatedUserEntity
    {
        ///// <summary>
        ///// 创建人
        ///// </summary>
        //[AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DetailId, C.SearchId], Suffix = "Id")]
        //[AutoCodeGenerator.AutoCodeProperty(Ids = [C.SearchId], Suffix = "Name", PropertyType = typeof(string))]
        //[AutoCodeGenerator.AutoCodeProperty(Ids = [C.DetailId], PropertyType = typeof(UserOutput))]
        //[AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId], PropertyType = typeof(SysUser), Attributes = [$"Navigate(NavigateType.ManyToOne, nameof({nameof(UpdatedUser)}Id))"])]
        //public long? UpdatedUser { get; set; }
    }
    public interface BaseCreatedUserTimeEntity : BaseCreatedTimeEntity, BaseCreatedUserEntity
    {
    }
}