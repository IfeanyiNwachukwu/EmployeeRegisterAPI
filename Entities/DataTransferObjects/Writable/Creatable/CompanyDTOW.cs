using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects.Writable
{
    public class CompanyDTOW
    {
        [Required(ErrorMessage = "Company name is a required field.")]
        [MaxLength(60, ErrorMessage = "Maximum length for the name is 60 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Company address is a required field.")]
        [MaxLength(150, ErrorMessage = "Maximum length for the name is 150 characters")]
        public string Address { get; set; }
        public string Country { get; set; }

        // To create children rexources(employees) together with the parent(company)
        public IEnumerable<EmployeeDTOW> Employees { get; set; }
    }
}
