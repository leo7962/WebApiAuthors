namespace WebApiAuthors.Dtos;

public class RecursesCollection<T>: Recurse where T: Recurse
{
    public List<T> Values { get; set; }
}
