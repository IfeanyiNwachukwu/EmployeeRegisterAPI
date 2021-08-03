using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects.Writable.Updatable
{
    public class CompanyUDTOW
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }

        // To create children rexources(employees) together with the parent(company)
        public IEnumerable<EmployeeDTOW> Employees { get; set; }
    }
}
