using AutoGeneratorModes;
using Certify.ACME.Anvil.Acme.Resource;
using C = AutoGeneratorModes.AutoGeneratorConst;

namespace OneNetIotService.Server.EntityAutoGenerator;

/// <summary>
/// 域名 申请 记录
/// </summary>
[AutoCodeGenerator.AutoCodeClassModes(Id = C.DtoId, Suffix = C.DtoSuffix)]
[AutoCodeGenerator.AutoCodeClassModes(Id = C.EntityId, Suffix = C.EntitySuffix, InterfaceTypes = [typeof(IEntity<long>)])]
[AutoCodeGenerator.AutoCodeClassModes(Id = C.CreateId, InheritStr = nameof(DomainOrder) + C.DtoSuffix, Suffix = C.CreateSuffix, SummaryPrefix = C.CreateSummaryPrefix)]
[AutoCodeGenerator.AutoCodeClassModes(Id = C.UpdateId, InheritStr = nameof(DomainOrder) + C.DtoSuffix, Suffix = C.UpdateSuffix, SummaryPrefix = C.UpdateSummaryPrefix)]
[AutoCodeGenerator.AutoCodeClassModes(Id = C.DetailId, InheritStr = nameof(DomainOrder) + C.DtoSuffix, Suffix = C.DetailSuffix, SummaryPrefix = C.DetailSummaryPrefix)]
[AutoCodeGenerator.AutoCodeClassModes(Id = C.SearchId, ToNullable = true, Suffix = C.SearchSuffix, SummaryPrefix = C.SearchSummaryPrefix)]
public interface DomainOrder : BaseEntity<long>
{
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    public string AcmeTitle { get; set; }
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    public string Domains { get; set; }
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    public string DirectoryUri { get; set; }
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    public string OrderUrl { get; set; }
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    public System.DateTimeOffset? OrderExpires { get; set; }
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    public OrderStatus? OrderStatus { get; set; }
    /// <summary>
    /// 备注
    /// </summary>
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    public string? Description { get; set; }
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    public string? PrivateKey { get; set; }
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    public string? Certificate { get; set; }
}
