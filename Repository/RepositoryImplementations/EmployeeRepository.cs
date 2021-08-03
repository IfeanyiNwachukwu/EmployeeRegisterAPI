using Contracts.Repositoryinterfaces;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository.RepositoryImplementations
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext):base(repositoryContext)
        {

        }

        public IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges)
        {
          return  FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
                .OrderBy(e => e.Name);
        }

        public Employee GetEmployee(Guid companyId, Guid id, bool trackChanges)
        {
            return FindByCondition(c => c.CompanyId.Equals(companyId) && c.Id.Equals(id),trackChanges)
                .SingleOrDefault();
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
