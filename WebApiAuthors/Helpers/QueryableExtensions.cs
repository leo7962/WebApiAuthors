using WebApiAuthors.Dtos;

namespace WebApiAuthors.Helpers;

public static class QueryableExtensions
{
    public static IQueryable<T> Page<T>(this IQueryable<T> queryable, PaginationDto paginationDto)
    {
        return queryable.Skip((paginationDto.Page - 1) * paginationDto.RecordsPerPage)
            .Take(paginationDto.RecordsPerPage);
    }
}