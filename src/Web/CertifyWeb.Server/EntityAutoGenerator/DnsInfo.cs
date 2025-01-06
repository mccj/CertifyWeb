using AutoGeneratorModes;
using C = AutoGeneratorModes.AutoGeneratorConst;

namespace OneNetIotService.Server.EntityAutoGenerator;

/// <summary>
/// 逸青设备数据记录
/// </summary>
//[AutoCodeGenerator.AutoCodeClassModes(Id = C.InfluxDBId, Suffix = C.InfluxDBSuffix, Attributes = [C.InfluxDBMeasurement + "(\"YiQingDeviceCommand\")"])]
[AutoCodeGenerator.AutoCodeClassModes(Id = C.DtoId, Suffix = C.DtoSuffix)]
[AutoCodeGenerator.AutoCodeClassModes(Id = C.EntityId, Suffix = C.EntitySuffix, InterfaceTypes = [typeof(IEntity<long>)])]
//[AutoCodeGenerator.AutoCodeClassModes(Id = C.CreateId, InheritStr = nameof(YiQingDeviceCommand) + C.DtoSuffix, Suffix = C.CreateSuffix, SummaryPrefix = C.CreateSummaryPrefix)]
//[AutoCodeGenerator.AutoCodeClassModes(Id = C.UpdateId, InheritStr = nameof(YiQingDeviceCommand) + C.DtoSuffix, Suffix = C.UpdateSuffix, SummaryPrefix = C.UpdateSummaryPrefix)]
[AutoCodeGenerator.AutoCodeClassModes(Id = C.DetailId, InheritStr = nameof(AcmeInfo) + C.DtoSuffix, Suffix = C.DetailSuffix, SummaryPrefix = C.DetailSummaryPrefix)]
[AutoCodeGenerator.AutoCodeClassModes(Id = C.SearchId, ToNullable = true, Suffix = C.SearchSuffix, SummaryPrefix = C.SearchSummaryPrefix)]
public interface DnsInfo : BaseEntity<long>
{
    /// <summary>
    /// 产品编号(来自OneNet)
    /// </summary>
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.InfluxDBId], Attributes = [C.InfluxDBColumnTag])]
    public string? ExecuteProductId { get; set; }
    /// <summary>
    /// 设备编号(来自OneNet)
    /// </summary>
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.InfluxDBId], Attributes = [C.InfluxDBColumnTag])]
    public string? ExecuteDeviceId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.InfluxDBId], Attributes = [C.InfluxDBColumnTag])]
    public string? ExecuteIMEI { get; set; } 
    /// <summary>
    /// 产品编号(来自OneNet)
    /// </summary>
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.InfluxDBId], Attributes = [C.InfluxDBColumnTag])]
    public string? TriggerProductId { get; set; }
    /// <summary>
    /// 设备编号(来自OneNet)
    /// </summary>
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.InfluxDBId], Attributes = [C.InfluxDBColumnTag])]
    public string? TriggerDeviceId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.InfluxDBId], Attributes = [C.InfluxDBColumnTag])]
    public string? TriggerIMEI { get; set; }
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.InfluxDBId], Attributes = [C.InfluxDBColumnTag])]
    public decimal? 设置开度 { get; set; }
    /// <summary>
    /// 执行备注
    /// </summary>
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.InfluxDBId], Attributes = [C.InfluxDBColumnTag])]
    public string? Remarks { get; set; }
    /// <summary>
    /// 时间
    /// </summary>
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId])]
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.SearchId], PropertyType = typeof(SearchRangeForStructValue<DateTime>))]
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.InfluxDBId], Attributes = [C.InfluxDBColumnTimestamp])]
    public DateTime Time { get; set; }
}

