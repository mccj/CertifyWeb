using AutoGeneratorService;
using Microsoft.AspNetCore.Mvc;
using OneNetIotService.Server.EntityAutoGenerator.AutoGenerator;
using SqlSugar;

namespace CertifyWeb.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DomainOrderController(IServiceProvider serviceProvider) : BaseCrudAppService<OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DomainOrderRecord, OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DomainOrderCreateInput, OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DomainOrderUpdateInput, OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DomainOrderDetailOutput, OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DomainOrderSearchInput>(serviceProvider)
    {
        protected internal override ISugarQueryable<DomainOrderRecord> GetSearchQueryable(ISugarQueryable<DomainOrderRecord> queryable, DomainOrderSearchInput? search)
        {
            return queryable;
        }
    }
}
