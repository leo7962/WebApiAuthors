namespace WebApiAuthors.Dtos;

public class DataHateoas
{
    public string Link { get; private set; }
    public string Description { get; private set; }
    public string Method { get; private set; }

    public DataHateoas(string link, string description, string method)
    {
        Link = link;
        Description = description;
        Method = method;
    }
}
