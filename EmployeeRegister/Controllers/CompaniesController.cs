using AutoMapper;
using Contracts;
using EmployeeRegister.ModelBinders;
using Entities.DataTransferObjects.ReadOnly;
using Entities.DataTransferObjects.Writable;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeRegister.Controllers
{
    [Route("api/companies")]
    [ApiController]
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
        [HttpGet]
        public IActionResult GetCompanies()
        {

            var companies = _repository.Company.GetAllCompanies(trackChanges: false);
            var companiesDTO = _mapper.Map<IEnumerable<CompanyDTO>>(companies);
            return Ok(companiesDTO);

            //throw new Exception("Exception");

        }

        [HttpGet("{id}",Name = "CompanyById")]
        public IActionResult GetCompany(Guid id)
        {
            var company = _repository.Company.GetCompany(id, trackChanges: false);
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

        [HttpPost]
        public IActionResult CreateCompany([FromBody] CompanyDTOW model)
        {
            if(model == null)
            {
                _logger.LogError("CompanyCreationDto object sent from client is null.");
                return BadRequest("CompanyForCreationDto object is null");
            }
            var companyEntity = _mapper.Map<Company>(model); //model is the object source

            _repository.Company.CreateCompany(companyEntity);
            _repository.Save();

            var companyToReturn = _mapper.Map<CompanyDTO>(companyEntity);

            return CreatedAtRoute("CompanyById", new { id = companyToReturn.Id }, companyToReturn);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public IActionResult GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            if(ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }
            var companyEntities = _repository.Company.GetByIds(ids, trackChanges: false);

            if(ids.Count() != companyEntities.Count())  // ids submitted is not equal to the companies found
            {
                _logger.LogError("some Ids are not valid in the collection");
                return NotFound();
            }

            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDTO>>(companyEntities);
            return Ok(companiesToReturn);
        }

        [HttpPost("collection")]
        public IActionResult CreateCompanyCollection([FromBody] IEnumerable<CompanyDTOW> models)
        {
            if(models == null)
            {
                _logger.LogError("Company collection sent from client is null");
                return BadRequest("Company collection is null");
            }
            var companyEntities = _mapper.Map<IEnumerable<Company>>(models);

            foreach(var company in companyEntities)
            {
                _repository.Company.CreateCompany(company);
            }
            _repository.Save();

            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDTO>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));

            return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCompany(Guid id)
        {
            var company = _repository.Company.GetCompany(id, trackChanges: false);
            if(company == null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _repository.Company.DeleteCompany(company);
            _repository.Save();

            return NoContent();
        }
   
    }
}
