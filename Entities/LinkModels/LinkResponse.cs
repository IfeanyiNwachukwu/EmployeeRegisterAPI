using Entities.Models;
using System.Collections.Generic;

namespace Entities.LinkModels
{/// <summary>
/// Response sent to client after all links have been generated
/// </summary>
    public class LinkResponse
    {
        public bool HasLinks { get; set; }
        public List<Entity> ShapedEntities { get; set; }
        public LinkCollectionWrapper<Entity> LinkedEntities { get; set; }

        public LinkResponse()
        {
            LinkedEntities = new LinkCollectionWrapper<Entity>(); // returns this when there are links
            ShapedEntities = new List<Entity>();  // returns this, when there are no url links
        }

    }
}
