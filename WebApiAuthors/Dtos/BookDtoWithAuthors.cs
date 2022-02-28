namespace WebApiAuthors.Dtos;

public class BookDtoWithAuthors : BookDto
{
    public List<AuthorDto> Authors { get; set; }
}