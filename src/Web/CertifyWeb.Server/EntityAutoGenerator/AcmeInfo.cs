using AutoGeneratorModes;
using C = AutoGeneratorModes.AutoGeneratorConst;

namespace OneNetIotService.Server.EntityAutoGenerator;

/// <summary>
/// Acme 记录
/// </summary>
[AutoCodeGenerator.AutoCodeClassModes(Id = C.DtoId, Suffix = C.DtoSuffix)]
[AutoCodeGenerator.AutoCodeClassModes(Id = C.EntityId, Suffix = C.EntitySuffix, InterfaceTypes = [typeof(IEntity<long>)])]
[AutoCodeGenerator.AutoCodeClassModes(Id = C.CreateId, InheritStr = nameof(AcmeInfo) + C.DtoSuffix, Suffix = C.CreateSuffix, SummaryPrefix = C.CreateSummaryPrefix)]
[AutoCodeGenerator.AutoCodeClassModes(Id = C.UpdateId, InheritStr = nameof(AcmeInfo) + C.DtoSuffix, Suffix = C.UpdateSuffix, SummaryPrefix = C.UpdateSummaryPrefix)]
[AutoCodeGenerator.AutoCodeClassModes(Id = C.DetailId, InheritStr = nameof(AcmeInfo) + C.DtoSuffix, Suffix = C.DetailSuffix, SummaryPrefix = C.DetailSummaryPrefix)]
[AutoCodeGenerator.AutoCodeClassModes(Id = C.SearchId, ToNullable = true, Suffix = C.SearchSuffix, SummaryPrefix = C.SearchSummaryPrefix)]
public interface AcmeInfo : BaseEntity<long>
{
    /// <summary>
    /// DirectoryUri
    /// </summary>
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    public string DirectoryUri { get; set; }
    /// <summary>
    /// Email
    /// </summary>
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    public string Email { get; set; }
    /// <summary>
    /// TermsOfServiceAgreed
    /// </summary>
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    public bool? TermsOfServiceAgreed { get; set; }
    /// <summary>
    /// EabKeyId
    /// </summary>
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    public string? EabKeyId { get; set; }
    /// <summary>
    /// EabKey
    /// </summary>
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    public string? EabKey { get; set; }
    /// <summary>
    /// EabKeyAlg
    /// </summary>
    [AutoCodeGenerator.AutoCodeProperty(Ids = [C.EntityId, C.DtoId, C.SearchId])]
    public string? EabKeyAlg { get; set; }
}

