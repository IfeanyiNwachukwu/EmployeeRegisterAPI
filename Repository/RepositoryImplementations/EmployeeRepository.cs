﻿using Contracts.Repositoryinterfaces;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.RepositoryImplementations
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext):base(repositoryContext)
        {

        }

        public async Task<IEnumerable<Employee>> GetEmployees(Guid companyId, bool trackChanges)
        {
          return await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
                .OrderBy(e => e.Name).ToListAsync();
        }

        public async Task<Employee> GetEmployee(Guid companyId, Guid id, bool trackChanges)
        {
            return await FindByCondition(c => c.CompanyId.Equals(companyId) && c.Id.Equals(id),trackChanges)
                .SingleOrDefaultAsync();
        }

        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public void DeleteEmployee(Employee employee)
        {
            Delete(employee);
        }
    }
}
