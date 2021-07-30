using Contracts.Repositoryinterfaces;
using Entities;
using Entities.Models;

namespace Repository.RepositoryImplementations
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext):base(repositoryContext)
        {

        }
    }
}
