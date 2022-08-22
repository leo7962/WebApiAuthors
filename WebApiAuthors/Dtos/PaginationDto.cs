namespace WebApiAuthors.Dtos;

public class PaginationDto
{
    private const int MaxQuantityPerPage = 50;
    private int _recordsPerPage = 10;
    public int Page { get; set; } = 1;

    public int RecordsPerPage
    {
        get => _recordsPerPage;
        set => _recordsPerPage = value > MaxQuantityPerPage ? MaxQuantityPerPage : value;
    }
}