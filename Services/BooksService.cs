using Microsoft.EntityFrameworkCore;

namespace Exercise4.Services;

using Exercise4.Enums;
using Exercise4.Data;
using Exercise4.Models;
using Exercise4.Responses;

public class BooksService
{
    // DB Initializing
    private readonly MysqlDbContext _context;

    public BooksService(MysqlDbContext context)
    {
        _context = context;
    }
    
    // UTILS
    public string? SharesISBN(Book book)
    {
        // Searching a book with the same title
        var bookFound =  _context.Books.FirstOrDefault(b => b.Title == book.Title);

        // Checking that there's at least one book with same title
        if (bookFound == null)
        {
            return null;
        }

        // If the book has same author, editorial and year published, must share ISBN
        if (bookFound.Author == book.Author && bookFound.Editorial == book.Editorial &&
            bookFound.YearPublished == book.YearPublished)
        {
            return bookFound.ISBN;
        }

        return null;
    }
    
    // GET
    public ServiceResponse<IEnumerable<Book>> GetAllBooks()
    {
        var books = _context.Books.ToList();

        return new ServiceResponse<IEnumerable<Book>>
        {
            Success = true,
            Data = books
        };
    }
    public ServiceResponse<Book> GetBookById(int id)
    {
        // Check if it exists
        var bookFound = _context.Books.Include(b => b.Loans).ThenInclude(l => l.User).FirstOrDefault(b => b.Id == id);

        if (bookFound == null)
        {
            return new ServiceResponse<Book>
            {
                Success = false,
                Message = "Book not found."
            };
        }
        
        // Returning response
        return new ServiceResponse<Book>
        {
            Success = true,
            Data = bookFound
        };
    }
    public ServiceResponse<IEnumerable<Book>> GetBooksByState(BookState state)
    {
        var books = _context.Books.Where(b => b.State == state).ToList();

        return new ServiceResponse<IEnumerable<Book>>
        {
            Success = true,
            Data = books
        };
    }
    public ServiceResponse<IEnumerable<Book>> GetBooksByGender(BookGender gender)
    {
        var books = _context.Books.Where(b => b.Gender == gender).ToList();

        return new ServiceResponse<IEnumerable<Book>>
        {
            Success = true,
            Data = books
        };
    }

    // POST
    public ServiceResponse<Book> AddBook(Book book)
    {
        // Checking if that book is already registered and it has the same isbn
        string? sharesISBN = this.SharesISBN(book);

        Console.WriteLine($"SHARE: {sharesISBN}, NEW: {book.ISBN}");
        
        if (sharesISBN != null && book.ISBN != sharesISBN)
        {
            Console.WriteLine("HELLOOOOOOOOOOOOO");
            return new ServiceResponse<Book>
            {
                Success = false,
                Message = $"Book already exists. Must share ISBN '{sharesISBN}'"
            };
        }
        
        // Adding book
        _context.Books.Add(book);
        var response = _context.SaveChanges();
        
        // Returning response
        if (response > 0)
        {
            return new ServiceResponse<Book>
            {
                Success = true,
                Data = book
            };
        }

        return new ServiceResponse<Book>
        {
            Success = false,
            Message = "Book could not be created. Database Error"
        };
    }
    
    // UPDATE
    public ServiceResponse<Book> UpdateBook(Book book)
    {
        // Checking that the book exists 
        var bookFound = _context.Books.FirstOrDefault(b => b.Id == book.Id);

        if (bookFound == null)
        {
            return new ServiceResponse<Book>
            {
                Success = false,
                Message = "Book not found."
            };
        }
        
        // Updating book
        bookFound.Description = book.Description;
        bookFound.Gender = book.Gender;
        bookFound.NumberOfPages = book.NumberOfPages;
        bookFound.State = book.State;
        
        var response = _context.SaveChanges();
        
        // Returning response
        if (response > 0)
        {
            return new ServiceResponse<Book>
            {
                Success = true,
                Data = book
            };
        }

        return new ServiceResponse<Book>
        {
            Success = false,
            Message = "Book could not be updated. Database Error"
        };
    }
    
    // DELETE
    public ServiceResponse<Book> DeleteBook(int id)
    {
        // Checking that the book exists
        var bookFound = _context.Books.Include(b => b.Loans).FirstOrDefault(b => b.Id == id);

        if (bookFound == null)
        {
            return new ServiceResponse<Book>
            {
                Success = false,
                Message = "Book not found."
            };
        }
        
        // Checking that the book hasn't active loans
        var activeLoans = bookFound.Loans.Any(l => l.State != LoanState.Returned);

        if (activeLoans)
        {
            return new ServiceResponse<Book>
            {
                Success = false,
                Message = "Book has active loans."
            };
        }
        
        // Deleting book
        _context.Books.Remove(bookFound);
        var response = _context.SaveChanges();
        
        // Returning response
        if (response > 0)
        {
            return new ServiceResponse<Book>
            {
                Success = true,
                Data = bookFound
            };
        }

        return new ServiceResponse<Book>
        {
            Success = false,
            Message = "Book could not be deleted. Database Error"
        };
    }
}