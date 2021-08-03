using Contracts.Repositoryinterfaces;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.RepositoryImplementations
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext):base(repositoryContext)
        {

        }

        public async  Task<IEnumerable<Company>> GetAllCompanies(bool trackChanges)
       => await FindAll(trackChanges)
            .OrderBy(c => c.Name)
            .ToListAsync();

        public async Task<Company> GetCompany(Guid companyId, bool trackchanges) =>
          await  FindByCondition(c => c.Id.Equals(companyId), trackchanges)
            .SingleOrDefaultAsync();

        public  void CreateCompany(Company company) =>  Create(company);

        public async Task<IEnumerable<Company>> GetByIds(IEnumerable<Guid> ids, bool trachChanges) =>
            await FindByCondition(x => ids.Contains(x.Id), trachChanges)
            .ToListAsync();

        public void DeleteCompany(Company company)
        {
            Delete(company);
        }
    }
}
