using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;

            //if we have total count of 10 and our pageSize is 5, it work out that we have 2 pages
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;

            //range of the items inside the constructor
            //we have access to these items inside our pagedList
            AddRange(items);
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        
        //receiving our query
        //create static method that we can call from anywhere
        //return PagedList
        // pass into this, IQueryable<T>
        
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
                //2 queries: count and execute toListAsync()

            //how many items are left in query, database call
            var count = await source.CountAsync();

            //items that we're gona returning
            //skip over a set records, skip over pageNumber-1 * pageSize
            //if we run page no.1 (pageSize is 5); page no.1 - 1 gives us 0, than 0*5 gives us no record in Skip(),
            //and we're taking 5 Take(pageSize)
            //if we run page no.2; 2-1 = 1; 1*5=; on second page taking 5 records
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            //Creating new instance of our class
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}