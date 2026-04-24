using Exercise4.Models;

namespace Exercise4.ViewModels;

public class BookViewModel
{
    public IEnumerable<Book> Books { get; set; } = new List<Book>();
    public Book NewBook { get; set; } = new();
    public Book Book { get; set; } = new();
}