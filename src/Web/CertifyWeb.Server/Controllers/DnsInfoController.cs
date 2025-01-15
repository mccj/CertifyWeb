using AutoGeneratorService;
using Microsoft.AspNetCore.Mvc;
using OneNetIotService.Server.EntityAutoGenerator.AutoGenerator;
using SqlSugar;

namespace CertifyWeb.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DnsInfoController(IServiceProvider serviceProvider) : BaseCrudAppService<OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DnsInfoRecord, OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DnsInfoCreateInput, OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DnsInfoUpdateInput, OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DnsInfoDetailOutput, OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DnsInfoSearchInput>(serviceProvider)
    {
        protected internal override ISugarQueryable<DnsInfoRecord> GetSearchQueryable(ISugarQueryable<DnsInfoRecord> queryable, DnsInfoSearchInput? search)
        {
            return queryable;
        }
    }
}
