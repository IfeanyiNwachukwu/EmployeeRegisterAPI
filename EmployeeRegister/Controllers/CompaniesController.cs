using Contracts;
using Entities.DataTransferObjects.ReadOnly;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeRegister.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;

        public CompaniesController(IRepositoryManager repository,ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult GetCompanies()
        {
            try
            {
                var companies = _repository.Company.GetAllCompanies(trackChanges: false);
                var companiesDTO = companies.Select(c => new CompanyDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    FullAddress = string.Join(' ', c.Address, c.Country)
                }).ToList();
                return Ok(companiesDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong in the {nameof(GetCompanies)} action {ex}");
                return StatusCode(500, "Internal server error");
            }

        }
    }
}
