using System.Collections.Generic;

namespace Entities.LinkModels
{
    /// <summary>
    /// class that will contain all our links
    /// </summary>
    public class LinkResourceBase
    {
        public LinkResourceBase()
        {

        }
        public List<Link> Links { get; set; } = new List<Link>();
    }
}
