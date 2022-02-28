using AutoMapper;
using WebApiAuthors.Dtos;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Helpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<AuthorCreatedDto, Author>();
        CreateMap<Author, AuthorDto>();
        CreateMap<Author, AuthorDtoWithBooks>().ForMember(x => x.Books, y => y.MapFrom(MapAuthorDtoBook));

        CreateMap<BookCreatedDto, Book>().ForMember(x => x.BooksAuthors, y => y.MapFrom(MapBooksAuthos));
        CreateMap<Book, BookDto>();
        CreateMap<Book, BookDtoWithAuthors>().ForMember(x => x.Authors, y => y.MapFrom(MapAuthorDto));
        CreateMap<BookPatchDto, Book>().ReverseMap();

        CreateMap<CommentCreatedDto, Comment>();
        CreateMap<Comment, CommentDto>();
    }

    private List<BookAuthor> MapBooksAuthos(BookCreatedDto bookCreatedDto, Book book)
    {
        var result = new List<BookAuthor>();
        if (bookCreatedDto.AuthorIds == null) return result;

        foreach (var authorId in bookCreatedDto.AuthorIds) result.Add(new BookAuthor {AuthorId = authorId});

        return result;
    }

    private List<AuthorDto> MapAuthorDto(Book book, BookDto bookDto)
    {
        var result = new List<AuthorDto>();

        if (book.BooksAuthors == null) return result;

        foreach (var bookAuthor in book.BooksAuthors)
            result.Add(new AuthorDto
            {
                Id = bookAuthor.AuthorId,
                Name = bookAuthor.Author.Name
            });

        return result;
    }

    private List<BookDto> MapAuthorDtoBook(Author author, AuthorDto authorDto)
    {
        var result = new List<BookDto>();
        if (author.BooksAuthors == null) return result;

        foreach (var bookAuthor in author.BooksAuthors)
            result.Add(new BookDto
            {
                Id = bookAuthor.BookId,
                Title = bookAuthor.Book.Title
            });

        return result;
    }
}