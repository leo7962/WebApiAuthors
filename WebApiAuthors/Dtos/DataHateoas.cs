namespace WebApiAuthors.Dtos;

public class DataHateoas
{
    public DataHateoas(string link, string description, string method)
    {
        Link = link;
        Description = description;
        Method = method;
    }

    public string Link { get; }
    public string Description { get; }
    public string Method { get; }
}
