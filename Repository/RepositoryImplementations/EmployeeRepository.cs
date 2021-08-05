using Contracts.Repositoryinterfaces;
using Entities;
using Entities.Models;
using Entities.RequestFeatures.EntityRequestParameters;
using Entities.RequestFeatures.Meta;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.RepositoryImplementations
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext):base(repositoryContext)
        {

        }

        /// <summary>
        /// Method was found to be ideal for small sets of data
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeParameters"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId,EmployeeParameters employeeParameters, bool trackChanges)
        {
             var employees = await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
                  .OrderBy(e => e.Name)
                   .ToListAsync();

            return  PagedList<Employee>
              .ToPagedlist(source:employees,pageNumber: employeeParameters.PageNumber,pageSize: employeeParameters.PageSize);

        }
        /// <summary>
        /// method was found to be faster when tested on tables with millions of rowa
        /// </summary>
        public async Task<PagedList<Employee>> GetEmployeesFasterAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        {
            var employees = await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
                .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
                .Take(employeeParameters.PageSize)
                .ToListAsync();

            var count = await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
                .CountAsync();

            return new PagedList<Employee>(employees, count, employeeParameters.PageNumber, employeeParameters.PageSize);
        }

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
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
