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

    private static List<BookAuthor> MapBooksAuthos(BookCreatedDto bookCreatedDto, Book book)
    {
        var result = new List<BookAuthor>();
        if (bookCreatedDto.AuthorIds == null) return result;

        result.AddRange(bookCreatedDto.AuthorIds.Select(authorId => new BookAuthor { AuthorId = authorId }));

        return result;
    }

    private static List<AuthorDto> MapAuthorDto(Book book, BookDto bookDto)
    {
        var result = new List<AuthorDto>();

        if (book.BooksAuthors == null) return result;

        result.AddRange(book.BooksAuthors.Select(bookAuthor =>
            new AuthorDto { Id = bookAuthor.AuthorId, Name = bookAuthor.Author.Name }));

        return result;
    }

    private static List<BookDto> MapAuthorDtoBook(Author author, AuthorDto authorDto)
    {
        var result = new List<BookDto>();
        if (author.BooksAuthors == null) return result;

        result.AddRange(author.BooksAuthors.Select(bookAuthor =>
            new BookDto { Id = bookAuthor.BookId, Title = bookAuthor.Book.Title }));

        return result;
    }
}