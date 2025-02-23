namespace TechLibrary.Frontend.Models;

public class BookResponse
{
    public Pagination Pagination { get; set; }
    public List<Book> Books { get; set; }
}