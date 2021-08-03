using AutoMapper;
using Contracts;
using Entities.DataTransferObjects.ReadOnly;
using Entities.DataTransferObjects.Writable;
using Entities.DataTransferObjects.Writable.Updatable;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

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
        public IActionResult GetEmployeesForCompany(Guid companyId)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if(company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database");
                return NotFound();
            }

            var employeesFromDb = _repository.Employee.GetEmployees(companyId, trackChanges: false);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDTO>>(employeesFromDb);
            return Ok(employeesDto);
        }

        [HttpGet("{id}", Name = "GetEmployeeForCompany")]
        public IActionResult GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database");
                return NotFound();
            }
            var employeeDb = _repository.Employee.GetEmployee(companyId, id, trackChanges: false);
            if(employeeDb == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            var employee = _mapper.Map<EmployeeDTO>(employeeDb);
            return Ok(employee);
        }

        [HttpPost]
        public IActionResult CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeDTOW model)
        {
            if(model == null)
            {
                _logger.LogError("Employee for creation DTO sent from model is null");
                return BadRequest("Employee for creation DTO  is null");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the Employee Creation DTO");
                return UnprocessableEntity(ModelState);
            }
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if(company == null)
            {
                _logger.LogInfo($"company with id: {companyId} doesn't exist in the database");
                return NotFound();
            }
            var employeeEntity = _mapper.Map<Employee>(model);  // turning our model to an employee type
            _repository.Employee.CreateEmployeeForCompany(companyId,employeeEntity);
            _repository.SaveAsync();

            var employeeToReturn = _mapper.Map<EmployeeDTO>(employeeEntity);  // destination | source _mapper.Map<TDestination>(TSource object)


            return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = employeeToReturn.Id }, employeeToReturn);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEmployeeForCompany(Guid companyId, Guid id)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if(company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exixt in the database.");
                return NotFound();
            }
            var employeeForCompany = _repository.Employee.GetEmployee(companyId, id, trackChanges: false);
            if(employeeForCompany == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _repository.Employee.DeleteEmployee(employeeForCompany);
            _repository.SaveAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateemployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeUDTOW model)
        {
            if(model == null)
            {
                _logger.LogError("Employye Update DTO is null");
                return BadRequest("Employye Update DTO is null");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("DTO for updating emplooyee sent from client is null");
                return UnprocessableEntity(ModelState);
            }
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if(company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }
            var employeeEntity = _repository.Employee.GetEmployee(companyId, id, trackChanges: true);
            if(employeeEntity == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            _mapper.Map(model, employeeEntity); // source | destination... everything in model is copied into employeeentity
            _repository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdatedEmployeeForCompany(Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeUDTOW> patchDoc)
        {
            if(patchDoc == null)
            {
                _logger.LogError("patchDoc object sent from client is null");
                return BadRequest("patchDoc object is null");
            }

            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if(company == null)
            {
                _logger.LogInfo($"company with id: {companyId} doesn't exist in the database");
                return NotFound();
            }

            var employeeentity = _repository.Employee.GetEmployee(companyId, id, trackChanges: true);
            if(employeeentity == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't eist in the database");
                return NotFound();
            }

            var employeeToPatch = _mapper.Map<EmployeeUDTOW>(employeeentity);
            
            TryValidateModel(employeeToPatch);
            
            patchDoc.ApplyTo(employeeToPatch);
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }

            _mapper.Map(employeeToPatch, employeeentity);
            _repository.SaveAsync();

            return NoContent();
        }
    }
}
