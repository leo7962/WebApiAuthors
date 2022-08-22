using Microsoft.EntityFrameworkCore;

namespace WebApiAuthors.Helpers;

public static class HttpContextExtensions
{
    public static async Task InsertParametersPaginationHeader<T>(this HttpContext httpContext, IQueryable<T> queryable)
    {
        if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

        var quantity = await queryable.CountAsync();
        httpContext.Response.Headers.Add("cantidadTotalRegistros", quantity.ToString());
    }
}