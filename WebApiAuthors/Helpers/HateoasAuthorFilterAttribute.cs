using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiAuthors.Dtos;
using WebApiAuthors.Services;

namespace WebApiAuthors.Helpers;

public class HateoasAuthorFilterAttribute : HateosFilterAttribute
{
    private readonly GenerateLink _generateLink;

    public HateoasAuthorFilterAttribute(GenerateLink generateLink)
    {
        _generateLink = generateLink;
    }

    public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var maytInclude = MayIncludeHateoas(context);

        if (!maytInclude)
        {
            await next();
            return;
        }

        var result = context.Result as ObjectResult;
        var autorDto = result.Value as AuthorDto;

        if (autorDto == null)
        {
            var authorDtos = result.Value as List<AuthorDto> ??
                             throw new ArgumentException("Se esperaba una instancia del AuthorDto o List<AuthorDto>");

            authorDtos.ForEach(async author => await _generateLink.GenerateLinks(author));
            result.Value = authorDtos;
        }
        else
        {
            await _generateLink.GenerateLinks(autorDto);
        }

        await next();
    }
}
