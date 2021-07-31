﻿using AutoMapper;
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
          
                //var companies = _repository.Company.GetAllCompanies(trackChanges: false);
                //var companiesDTO = _mapper.Map<IEnumerable<CompanyDTO>>(companies);
                //return Ok(companiesDTO);

                throw new Exception("Exception");

        }
    }
}