using AutoGeneratorModes;
using C = AutoGeneratorModes.AutoGeneratorConst;

namespace OneNetIotService.Server.EntityAutoGenerator;

/// <summary>
/// DNS 记录
/// </summary>
[AutoCodeGenerator.AutoCodeClassModes(Id = C.DtoId, Suffix = C.DtoSuffix)]
[AutoCodeGenerator.AutoCodeClassModes(Id = C.EntityId, Suffix = C.EntitySuffix, InterfaceTypes = [typeof(IEntity<long>)])]
[AutoCodeGenerator.AutoCodeClassModes(Id = C.CreateId, InheritStr = nameof(DnsInfo) + C.DtoSuffix, Suffix = C.CreateSuffix, SummaryPrefix = C.CreateSummaryPrefix)]
[AutoCodeGenerator.AutoCodeClassModes(Id = C.UpdateId, InheritStr = nameof(DnsInfo) + C.DtoSuffix, Suffix = C.UpdateSuffix, SummaryPrefix = C.UpdateSummaryPrefix)]
[AutoCodeGenerator.AutoCodeClassModes(Id = C.DetailId, InheritStr = nameof(DnsInfo) + C.DtoSuffix, Suffix = C.DetailSuffix, SummaryPrefix = C.DetailSummaryPrefix)]
[AutoCodeGenerator.AutoCodeClassModes(Id = C.SearchId, ToNullable = true, Suffix = C.SearchSuffix, SummaryPrefix = C.SearchSummaryPrefix)]
public interface DnsInfo : BaseEntity<long>
{
    /// <summary>
    /// AcmeId
    /// </summary>
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    public string ProviderType { get; set; }
    /// <summary>
    /// Credentials
    /// </summary>
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    public Dictionary<string, string> Credentials { get; set; }
    /// <summary>
    /// Parameters
    /// </summary>
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    public Dictionary<string, string> Parameters { get; set; }
    /// <summary>
    /// 备注
    /// </summary>
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    public string? Description { get; set; }
}

