using Contracts.Repositoryinterfaces;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository.RepositoryImplementations
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext):base(repositoryContext)
        {

        }

        public IEnumerable<Company> GetAllCompanies(bool trackChanges)
       => FindAll(trackChanges)
            .OrderBy(c => c.Name)
            .ToList();

        public Company GetCompany(Guid companyId, bool trackchanges) =>
            FindByCondition(c => c.Id.Equals(companyId), trackchanges)
            .SingleOrDefault();

        public void CreateCompany(Company company) => Create(company);

        public IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trachChanges) =>
            FindByCondition(x => ids.Contains(x.Id), trachChanges)
            .ToList();

        public void DeleteCompany(Company company)
        {
            Delete(company);
        }
    }
}
