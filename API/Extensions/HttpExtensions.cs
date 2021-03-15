using System.Text.Json;
using API.Helpers;
using Microsoft.AspNetCore.Http;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        //adding pagination headrs on to our response
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, 
            int itemsPerPage, int totalItems, int totalPages)
            {
                //Create pagination header
                var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);

                //for camelCase writing in headers->Pagination
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                //add pagination to our response header
                //serialize this, bc response headrs when we add this takes key and the string value
                response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));
                //bc. we're adding custome header, need also to add course headeron to this to make this header avalieable
                response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
            }
    }
}