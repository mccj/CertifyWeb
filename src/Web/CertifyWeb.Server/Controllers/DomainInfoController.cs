using AutoGeneratorService;
using Microsoft.AspNetCore.Mvc;
using OneNetIotService.Server.EntityAutoGenerator.AutoGenerator;
using SqlSugar;

namespace CertifyWeb.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DomainInfoController(IServiceProvider serviceProvider) : BaseCrudAppService<OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DomainInfoRecord, OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DomainInfoCreateInput, OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DomainInfoUpdateInput, OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DomainInfoDetailOutput, OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DomainInfoSearchInput>(serviceProvider)
    {
        protected internal override ISugarQueryable<DomainInfoRecord> GetSearchQueryable(ISugarQueryable<DomainInfoRecord> queryable, DomainInfoSearchInput? search)
        {
            return queryable;
        }
    }
}
