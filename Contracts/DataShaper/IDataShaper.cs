using System.Collections.Generic;
using System.Dynamic;

namespace Contracts.DataShaper
{
    public interface IDataShaper<T>
    {
        IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string fieldsString);

        ExpandoObject ShapeData(T entity, string fieldString);

        // ExpandoObject is an object whose members can only be dynamically added or removed at run time

    }
}
