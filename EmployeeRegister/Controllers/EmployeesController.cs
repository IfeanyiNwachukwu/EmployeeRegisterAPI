using AutoMapper;
using Contracts;
using EmployeeRegister.Filters.ActionFilters;
using Entities.DataTransferObjects.ReadOnly;
using Entities.DataTransferObjects.Writable;
using Entities.DataTransferObjects.Writable.Updatable;
using Entities.Models;
using Entities.RequestFeatures.EntityRequestParameters;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeRegister.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public EmployeesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet()]
        public async  Task<IActionResult> GetEmployeesForCompany(Guid companyId,[FromQuery] EmployeeParameters employeeParameters)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if(company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database");
                return NotFound();
            }

            var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId,employeeParameters, trackChanges: false);

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(employeesFromDb.MetaData));
            
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDTO>>(employeesFromDb);
            
            return Ok(employeesDto);
        }

        [HttpGet("{id}", Name = "GetEmployeeForCompany")]
       
        public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database");
                return NotFound();
            }
            var employeeDb = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: false);
            if(employeeDb == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            var employee = _mapper.Map<EmployeeDTO>(employeeDb);
            return Ok(employee);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeDTOW model)
        {
            
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if(company == null)
            {
                _logger.LogInfo($"company with id: {companyId} doesn't exist in the database");
                return NotFound();
            }
            var employeeEntity = _mapper.Map<Employee>(model);  // turning our model to an employee type
            _repository.Employee.CreateEmployeeForCompany(companyId,employeeEntity);
            await _repository.SaveAsync();

            var employeeToReturn = _mapper.Map<EmployeeDTO>(employeeEntity);  // destination | source _mapper.Map<TDestination>(TSource object)


            return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = employeeToReturn.Id }, employeeToReturn);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
        [ServiceFilter(typeof(ValidateEmployeeForCompanyExists))]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id)
        {
            
          var employeeForCompany = HttpContext.Items["employee"] as Employee;
         
            _repository.Employee.DeleteEmployee(employeeForCompany);
           await  _repository.SaveAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateEmployeeForCompanyExists))]
        public async Task<IActionResult> UpdateemployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeUDTOW model)
        {

            var employeeEntity = HttpContext.Items["employee"] as Employee;
         _mapper.Map(model, employeeEntity); // source | destination... everything in model is copied into employeeentity
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        [ServiceFilter(typeof(ValidateEmployeeForCompanyExists))]
        public async Task<IActionResult> PartiallyUpdatedEmployeeForCompany(Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeUDTOW> patchDoc)
        {
            if(patchDoc == null)
            {
                _logger.LogError("patchDoc object sent from client is null");
                return BadRequest("patchDoc object is null");
            }
            var employeeentity = HttpContext.Items["employee"] as Employee;
           var employeeToPatch = _mapper.Map<EmployeeUDTOW>(employeeentity);
            
            TryValidateModel(employeeToPatch);
            
            patchDoc.ApplyTo(employeeToPatch);
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }

            _mapper.Map(employeeToPatch, employeeentity);
            await _repository.SaveAsync();

            return NoContent();
        }

      
    }
}
