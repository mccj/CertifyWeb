using AutoGeneratorService;
using Microsoft.AspNetCore.Mvc;
using OneNetIotService.Server.EntityAutoGenerator.AutoGenerator;
using SqlSugar;

namespace CertifyWeb.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AcmeInfoController(IServiceProvider serviceProvider) : BaseCrudAppService<OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.AcmeInfoRecord, OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.AcmeInfoCreateInput, OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.AcmeInfoUpdateInput, OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.AcmeInfoDetailOutput, OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.AcmeInfoSearchInput>(serviceProvider)
    {
        protected internal override ISugarQueryable<AcmeInfoRecord> GetSearchQueryable(ISugarQueryable<AcmeInfoRecord> queryable, AcmeInfoSearchInput? search)
        {
            return queryable;
        }
    }
}
