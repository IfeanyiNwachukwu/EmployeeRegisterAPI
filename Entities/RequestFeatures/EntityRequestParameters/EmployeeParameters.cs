using Entities.RequestFeatures.RequestFeatureAbstraction;

namespace Entities.RequestFeatures.EntityRequestParameters
{
    public class EmployeeParameters : RequestParameters
    {
        public EmployeeParameters()
        {
            OrderBy = "name";
        }
        public uint MinAge { get; set; }
        public uint MaxAge { get; set; } = int.MaxValue;

        public bool ValidAgeRange => MaxAge > MinAge;

        public string SearchTerm { get; set; }

        //uint is used b/c we only wan positive age values
    }
}
