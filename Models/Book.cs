using System.ComponentModel.DataAnnotations;

namespace Exercise4.Models;

using Exercise4.Enums;

public class Book
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Please enter the book ISBN")]
    [StringLength(13, MinimumLength = 10, ErrorMessage = "Please enter a valid book ISBN")]
    public string ISBN { get; set; }
    
    [Required(ErrorMessage = "Please enter the book title")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Please enter a valid book title")]
    public string Title { get; set; }
    
    [Required(ErrorMessage = "Please enter the book description")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Please enter a valid book description")]
    public string Description { get; set; }
    
    [Required(ErrorMessage = "Please enter the book author")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "Please enter a valid book author")]
    public string Author { get; set; }
    
    [Required(ErrorMessage = "Please enter the book editorial")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Please enter a valid book editorial")]
    public string Editorial { get; set; }
    
    [Required(ErrorMessage = "Please enter the book gender")]
    public BookGender Gender { get; set; }
    
    [Required(ErrorMessage = "Please enter the book year published")]
    public int YearPublished { get; set; }
    
    [Required(ErrorMessage = "Please enter the book number of pages")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Solo números")]
    public int NumberOfPages { get; set; }
    
    [Required(ErrorMessage = "Please enter the book state")]
    public BookState State { get; set; }

    // Navegation List
    public List<Loan> Loans { get; set; } = new List<Loan>();
}