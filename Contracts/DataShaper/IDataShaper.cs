using Entities.Models;
using System.Collections.Generic;

namespace Contracts.DataShaper
{
    public interface IDataShaper<T>
    {
        IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fieldsString);

        ShapedEntity ShapeData(T entity, string fieldString);

        // ExpandoObject is an object whose members can only be dynamically added or removed at run time

    }
}
