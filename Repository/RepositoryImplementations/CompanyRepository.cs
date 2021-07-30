﻿using Contracts.Repositoryinterfaces;
using Entities;
using Entities.Models;
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
    }
}
