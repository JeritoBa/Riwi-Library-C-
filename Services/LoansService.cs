using Microsoft.EntityFrameworkCore;

namespace Exercise4.Services;

using Exercise4.Enums;
using Exercise4.Data;
using Exercise4.Models;
using Exercise4.Responses;
using Exercise4.Services;

public class LoansService
{
    // DB Initializing
    private readonly MysqlDbContext _context;
    private readonly EmailService _emailService;

    public LoansService(MysqlDbContext context,  EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }
    
    // UTILS
    public bool CheckOverdueLoans()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        _context.Loans
            .Where(l => l.ReturnedDate == null && l.ReturnDate < today)
            .ExecuteUpdate(l => l.SetProperty(loan => loan.State, LoanState.Overdue));
        var response = _context.SaveChanges();

        if (response > 0)
        {
            return true;
        }

        return false;
    }
    
    // GET
    public ServiceResponse<IEnumerable<Loan>> GetAllLoans()
    {
        // Checking if there's any overdue loan
        CheckOverdueLoans();
        
        var loans = _context.Loans
            .Include(l => l.User)
            .Include(l => l.Book)
            .ToList();

        return new ServiceResponse<IEnumerable<Loan>>
        {
            Data = loans,
            Success = true
        };
    }
    public ServiceResponse<Loan> GetLoanById(int id)
    {
        // Finding loan
        var loanFound = _context.Loans
            .Include(l => l.User)
            .Include(l => l.Book)
            .FirstOrDefault(l => l.Id == id);
        
        // Checking that the loan exists
        if (loanFound != null)
        {
            return new ServiceResponse<Loan>
            {
                Data = loanFound,
                Success = true
            };
        }

        return new ServiceResponse<Loan>
        {
            Success = false,
            Message = "Loan not found."
        };
    }
    public ServiceResponse<IEnumerable<Loan>> GetLoansByState(LoanState state)
    {
        var loans = _context.Loans.Include(l => l.User).Where(l => l.State == state).ToList();

        return new ServiceResponse<IEnumerable<Loan>>
        {
            Data = loans,
            Success = true
        };
    }
    
    // POST
    public ServiceResponse<Loan> AddLoan(Loan loan)
    {
        // Getting User by email
        
        
        // Getting book by title
        
        // Getting user by email and Checking that exists
        var userFound = _context.Users.Include(u => u.Loans).FirstOrDefault(u => (u.Email.ToLower()) == (loan.User.Email.ToLower()));

        if (userFound == null)
        {
            return new ServiceResponse<Loan>
            {
                Success = false,
                Message = "User must be registered"
            };
        }
        
        // Checking that the user doesn't have any active loan
        //var userHasLoaned = _context.Loans.Any(l => l.UserId == loan.UserId && l.State != LoanState.Returned);
        var userHasLoaned = userFound.Loans.Any(l => l.State != LoanState.Returned);

        if (userHasLoaned)
        {
            return new ServiceResponse<Loan>
            {
                Success = false,
                Message = "User already has an active loan"
            };
        }

        loan.UserId = userFound.Id; // Adding foreign key
        loan.User = null; /// Nulling user object, for avoid to EF creating new entity
        
        // Getting book by title and Checking that exists
        //var bookFound = _context.Books.FirstOrDefault(b => b.Id == loan.BookId);
        var bookFound = _context.Books.FirstOrDefault(b => (b.Title.ToLower()) == (loan.Book.Title).ToLower());
        
        if (bookFound == null)
        {
            return new ServiceResponse<Loan>
            {
                Success = false,
                Message = $"Book '{loan.Book.Title}' not found."
            };
        }
        
        // Checking that are any book available
        var bookAvailable = _context.Books.FirstOrDefault(b => b.ISBN == bookFound.ISBN && b.State == BookState.Available);
        
        if (bookAvailable == null)
        {
            return new ServiceResponse<Loan>
            {
                Success = false,
                Message = $"There isn't any book '{bookFound.Title}' with availability"
            };
        }

        loan.BookId = bookAvailable.Id; // Adding foreign key
        loan.Book = null; /// Nulling book object, for avoid to EF creating new entity
        
        // Creating return date
        loan.ReturnDate = loan.LoanDate.AddDays(30);
        
        // Creating new loan
        _context.Loans.Add(loan);

        // Changing state of book
        bookAvailable.State = BookState.Loaned;
        
        var response = _context.SaveChanges();
        
        // Returning response
        if (response > 0)
        {
            // Sending email
            string body = @$"
                <h2>Hello {userFound.Name} {userFound.LastName}</h2>
                <p>We are happy to advise you that the book {bookFound.Title} was loaned in a succesful way</p>
                <p>Remember you have until {loan.ReturnDate} for return the book</p>
                <p>Thank you for visit our library! Enjoy the reading :)</p>
            ";
            
            _emailService.SendEmail(userFound.Email, "Book loaned succesfully! - JGallego Library", body);
            
            return new ServiceResponse<Loan>
            {
                Success = true,
                Data = loan
            };
        }

        return new ServiceResponse<Loan>
        {
            Success = false,
            Message = "Database Error. Failed Creation"
        };
    }
    
    // PUT
    public ServiceResponse<Loan> UpdateLoan(Loan loan)
    {
        // Checking the loan exists
        var loanFound = _context.Loans.Include(l => l.Book).FirstOrDefault(l => l.Id == loan.Id);

        if (loanFound == null)
        {
            return new ServiceResponse<Loan>
            {
                Success = false,
                Message = "Loan not found."
            };
        }
        
        // Checking that if the state is returned, there's the returned date
        if (loan.State == LoanState.Returned && loan.ReturnedDate == null)
        {
            return new ServiceResponse<Loan>
            {
                Success = false,
                Message = "You must put the returned date for finish the loan."
            };
        }
        
        // Updating it
        loanFound.State = loan.State;
        loanFound.ReturnDate = loan.ReturnDate;
        loanFound.ReturnedDate = loan.ReturnedDate;
        loanFound.LoanDate = loan.LoanDate;
        
        // Changing state of book if loan was returned
        if (loan.State == LoanState.Returned)
        {
            loanFound.Book.State = BookState.Available;
        }
        
        var response = _context.SaveChanges();
        
        // Returning response
        if (response > 0)
        {
            return new ServiceResponse<Loan>
            {
                Success = true,
                Data = loan
            };
        }

        return new ServiceResponse<Loan>
        {
            Success = false,
            Message = "Failed in Database"
        };
    }
    
    // DELETE
    public ServiceResponse<Loan> DeleteLoan(int id)
    {
        // Checking the loan exist
        var loanFound = _context.Loans.Include(l => l.Book).FirstOrDefault(l => l.Id == id);

        if (loanFound == null)
        {
            return new ServiceResponse<Loan>
            {
                Success = false,
                Message = "Loan not found."
            };
        }
        
        // Changing book state if the loan wasn't different to returned
        if (loanFound.State != LoanState.Returned)
        {
            loanFound.Book.State = BookState.Available;
        }
        
        // Deleting loan
        _context.Loans.Remove(loanFound);
        
        var response = _context.SaveChanges();

        // Checking response and returning it
        if (response > 0)
        {
            return new ServiceResponse<Loan>
            {
                Success = true
            };
        }

        return new ServiceResponse<Loan>
        {
            Success = false,
            Message = "Error in Database"
        };
    }
    
    // BUSINESS LOGIC
    public ServiceResponse<Loan> PostponeLoan(int id)
    {
        // Checking the loan exists
        var loanFound = _context.Loans.FirstOrDefault(l => l.Id == id);

        if (loanFound == null)
        {
            return new ServiceResponse<Loan>
            {
                Success = false,
                Message = "Loan not found."
            };
        }
        
        // Updating state and dates
        DateOnly today = DateOnly.FromDateTime(DateTime.Now); 
        
        loanFound.State = LoanState.Postponed;
        loanFound.LoanDate = today;
        loanFound.ReturnDate = today.AddDays(30);
        
        // Returning result
        var response = _context.SaveChanges();

        if (response > 0)
        {
            return new ServiceResponse<Loan>
            {
                Success = true,
                Data = loanFound
            };
        }

        return new ServiceResponse<Loan>
        {
            Success = false,
            Message = "Error in Database"
        };
    }
    public ServiceResponse<Loan> ReturnLoan(int id)
    {
       // Checking that the loan exists
       var loanFound =  _context.Loans.Include(l => l.Book).FirstOrDefault(l => l.Id == id);

       if (loanFound == null)
       {
           return new ServiceResponse<Loan>
           {
               Success = false,
               Message = "Loan not found."
           };
       }
       
       // Updating book state and loan properties
       loanFound.Book.State = BookState.Available;
       loanFound.State = LoanState.Returned;
       loanFound.ReturnedDate = DateOnly.FromDateTime(DateTime.Now);
       
       // Saving changes and returning response
       var response = _context.SaveChanges();

       if (response > 0)
       {
           return new ServiceResponse<Loan>
           {
               Success = true,
               Data = loanFound
           };
       }

       return new ServiceResponse<Loan>
       {
           Success = false,
           Message = "Error in Database"
       };
    }
}