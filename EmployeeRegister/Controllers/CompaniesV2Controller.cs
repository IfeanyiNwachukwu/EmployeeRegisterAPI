using Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EmployeeRegister.Controllers
{
    /// <summary>
    /// Api Version is indicated in a query string
    /// </summary>
    [ApiVersion("2.0",Deprecated =true)]
    [Route("api/companies")]
    [ApiController]
    public class CompaniesV2Controller : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        public CompaniesV2Controller(IRepositoryManager repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges: false);
            return Ok(companies);
        }
        /// <summary>
        /// API version is indicated in the route name
        /// </summary>
        /// <returns></returns>
        [ApiVersion("2.0")]
        [HttpGet]
        [Route("{v:apiversion}/reload")]
        public async Task<IActionResult> GetCompaniesReload()
        {
            var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges: false);
            return Ok(companies);
        }

        /// <summary>
        /// Here the version is sent in the API header
        /// </summary>
        /// <returns></returns>
        //[ApiVersion("2.0")]
        [HttpGet]
        [Route("reloaded")]
        public async Task<IActionResult> GetCompaniesReloaded()
        {
            var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges: false);
            return Ok(companies);
        }
    }
}
