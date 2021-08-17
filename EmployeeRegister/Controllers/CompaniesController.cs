using AutoMapper;
using Contracts;
using EmployeeRegister.Filters.ActionFilters;
using EmployeeRegister.ModelBinders;
using Entities.DataTransferObjects.ReadOnly;
using Entities.DataTransferObjects.Writable;
using Entities.DataTransferObjects.Writable.Updatable;
using Entities.Models;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeRegister.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/companies")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
/*    [ResponseCache(CacheProfileName = "120SecondsDuration")] *///this cache rule applies to every action inside this controller except the ones that already have the ResponseCache attribute
    public class CompaniesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompaniesController(IRepositoryManager repository,ILoggerManager logger,IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        /// <summary>
        /// Gets the list of all companies
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetCompanies"), Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetCompanies()
        {

            var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges: false);
            var companiesDTO = _mapper.Map<IEnumerable<CompanyDTO>>(companies);
            return Ok(companiesDTO);

            //throw new Exception("Exception");

        }
        
        [HttpGet("{id}",Name = "CompanyById")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge =60)]
        [HttpCacheValidation(MustRevalidate =false)]
        [ResponseCache(Duration = 60)]
        public async  Task<IActionResult> GetCompany(Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(id, trackChanges: false);
            if(company == null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            else
            {
                var companyDto = _mapper.Map<CompanyDTO>(company);
                return Ok(companyDto);
            }
        }

        /// <summary>
        /// Creates a newly created company
        /// </summary>
        /// <param name="model"></param>
        /// <returns>A newly created company</returns>
        /// <response code="201">Returns the newly created Item</response>
        /// <response code="400">If the Item is null</response>
        /// <response code="422">If the model is invalid</response>
        [HttpPost(Name ="CreateCompany")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyDTOW model)
        {

            var companyEntity = _mapper.Map<Company>(model); //model is the object source

            _repository.Company.CreateCompany(companyEntity);
            await _repository.SaveAsync();

            var companyToReturn = _mapper.Map<CompanyDTO>(companyEntity);

            return CreatedAtRoute("CompanyById", new { id = companyToReturn.Id }, companyToReturn);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            if(ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }
            var companyEntities = await _repository.Company.GetByIdAsAsync(ids, trackChanges: false);

            if(ids.Count() != companyEntities.Count())  // ids submitted is not equal to the companies found
            {
                _logger.LogError("some Ids are not valid in the collection");
                return NotFound();
            }

            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDTO>>(companyEntities);
            return Ok(companiesToReturn);
        }

        [HttpPost("collection")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyDTOW> models)
        {
           
            var companyEntities = _mapper.Map<IEnumerable<Company>>(models);

            foreach(var company in companyEntities)
            {
                _repository.Company.CreateCompany(company);
            }
            await _repository.SaveAsync();

            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDTO>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));

            return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            var company = (Company)HttpContext.Items["company"];

            _repository.Company.DeleteCompany(company);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyUDTOW model)
        {

            //var companyEntity = await _repository.Company.GetCompanyAsync(id, trackChanges: true);
            var companyEntity = HttpContext.Items["company"];

            _mapper.Map(model, companyEntity);
            await _repository.SaveAsync();

            return NoContent();
        }
        /// <summary>
        /// Available options is returned in the allow Header of the response
        /// </summary>
        /// <returns></returns>
        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }

        // NOTE : GetCompanies and CreateCompanies are the only actions on the root URI level (api/companies)
    }
}
