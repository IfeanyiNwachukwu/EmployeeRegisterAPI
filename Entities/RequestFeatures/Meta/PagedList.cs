using System;
using System.Collections.Generic;
using System.Linq;

namespace Entities.RequestFeatures.Meta
{
    /// <summary>
    /// An extended List
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T> : List<T>
    {
        public MetaData MetaData { get; set; }
        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            MetaData = new MetaData
            {
                TotalCount = count,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)  // say 20 items(i.e count) and you want to display 4.   Total pages  20/4 = 5 pages
            };

            AddRange(items);
        }
        /// <summary>
        /// Say we need to get the results of the third page of our website counting 20(page size) as the number of results we want.That would mean we will have to skip the first((3 - 1) * 20) = 40  results,then take the next 20 and return them to the caller
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PagedList<T> ToPagedlist(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

    }
}
