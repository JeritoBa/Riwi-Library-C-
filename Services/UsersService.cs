using Microsoft.EntityFrameworkCore;

namespace Exercise4.Services;

using Exercise4.Enums;
using Exercise4.Data;
using Exercise4.Models;
using Exercise4.Responses;
using Exercise4.Services;

public class UsersService
{
    // DB Initializing
    private readonly MysqlDbContext _context;
    private readonly EmailService _emailService;

    // Dependency Inyection
    public UsersService(MysqlDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }
    
    // UTILS
    
    // GET
    public ServiceResponse<IEnumerable<User>> GetAllUsers()
    {
        var users = _context.Users.ToList();

        return new ServiceResponse<IEnumerable<User>>
        {
            Success = true,
            Data = users
        };
    }
    public ServiceResponse<User> GetUserById(int id)
    {
        var userFound = _context.Users.Include(u => u.Loans).ThenInclude(l => l.Book).FirstOrDefault(u => u.Id == id);

        if (userFound == null)
        {
            return new ServiceResponse<User>
            {
                Success = false,
                Message = "User not found."
            };
        }

        return new ServiceResponse<User>
        {
            Success = true,
            Data = userFound
        };
    }
    
    // POST
    public ServiceResponse<User> AddUser(User user)
    {
        try
        {
// Checking that the email isn't already registered
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                return new ServiceResponse<User>
                {
                    Success = false,
                    Message = "User with that email already exists."
                };
            }
        
            // Checking that the phone number isn't already registered
            if (_context.Users.Any(u => u.Phone == user.Phone))
            {
                return new ServiceResponse<User>
                {
                    Success = false,
                    Message = "User with that Phone already exists."
                };
            }
        
            // Adding user
            _context.Users.Add(user);
            var response = _context.SaveChanges();
        
            // Returning result
            if (response > 0)
            {
                // Sending email
                string body = @$"
                      Thank you for registering in our system {user.Name} {user.LastName}
            ";
            
                _emailService.SendEmail(user.Email, "Account registered with success", body);
            
                return new ServiceResponse<User>
                {
                    Success = true,
                    Data = user
                };
            }

            return new ServiceResponse<User>
            {
                Success = false,
                Message = "User could not be added. Database Error"
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
    
    // PUT
    public ServiceResponse<User> UpdateUser(User user)
    {
        // Checking the user exists
        var userFound =  _context.Users.FirstOrDefault(u => u.Id == user.Id);

        if (userFound == null)
        {
            return new ServiceResponse<User>
            {
                Success = false,
                Message = "User not found."
            };
        }
        
        // Updating user info
        userFound.Name = user.Name;
        userFound.LastName = user.LastName;
        userFound.Phone = user.Phone;
        userFound.Email = user.Email;
        userFound.Address = user.Address;
        userFound.EmergencyPhone = user.EmergencyPhone;
        
        var response = _context.SaveChanges();
        
        // Returning response
        if (response > 0)
        {
            return new ServiceResponse<User>
            {
                Success = true,
                Data = user
            };
        }

        return new ServiceResponse<User>
        {
            Success = false,
            Message = "User could not be updated. Database Error"
        };
    }
    
    // DELETE
    public ServiceResponse<User> DeleteUser(int id)
    {
        // Checking the user exists
        var userFound = _context.Users.Include(u => u.Loans).FirstOrDefault(u => u.Id == id);

        if (userFound == null)
        {
            return new ServiceResponse<User>
            {
                Success = false,
                Message = "User not found."
            };
        }
        
        // Checking the user hasn't any active loan
        var userHasLoans = userFound.Loans.Any(l => l.State != LoanState.Returned);

        if (userHasLoans)
        {
            return new ServiceResponse<User>
            {
                Success = false,
                Message = "User has active loans."
            };
        }
        
        // Deleting user
        _context.Users.Remove(userFound);
        var response = _context.SaveChanges();
        
        // Returning responses
        if (response > 0)
        {
            return new ServiceResponse<User>
            {
                Success = true,
                Data = userFound
            };
        }

        return new ServiceResponse<User>
        {
            Success = false,
            Message = "User could not be deleted. Database Error"
        };
    }
}