using System.ComponentModel.DataAnnotations;
using Exercise4.Enums;

namespace Exercise4.Models;

public class Loan
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Please enter the loan date")]
    public DateOnly LoanDate { get; set; } // Just dd/mm/yy without hour
    
    [Required(ErrorMessage = "Please enter the return date")]
    public DateOnly ReturnDate { get; set; }
    
    public DateOnly? ReturnedDate { get; set; }
    
    public LoanState State { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }
    
    // Navegation Objects
    public User User { get; set; } 
    public Book Book { get; set; } 
}