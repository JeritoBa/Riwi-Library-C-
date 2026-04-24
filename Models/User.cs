using System.ComponentModel.DataAnnotations;

namespace Exercise4.Models;

public class User
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Please enter the name")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters")]
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "Please enter the last name")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Last name must be between 3 and 50 characters")]
    public string LastName { get; set; }
    
    [Required(ErrorMessage = "Please enter the email address")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Please enter the phone number")]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    public string Phone { get; set; }
    
    [Required(ErrorMessage = "Please enter the address")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Please enter a valid address")]
    public string Address { get; set; }
    
    [Required(ErrorMessage = "Please enter the emergency phone number")]
    [Phone(ErrorMessage = "Please enter a valid emergency phone number")]
    public string EmergencyPhone { get; set; }
    
    // Navegation List
    public List<Loan> Loans { get; set; } = new List<Loan>();
}