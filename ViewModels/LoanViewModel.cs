using Exercise4.Models;

namespace Exercise4.ViewModels;

public class LoanViewModel
{
    public IEnumerable<Loan> Loans { get; set; } = new List<Loan>();
    public Loan NewLoan { get; set; } = new();
    public Loan Loan { get; set; } = new();
}