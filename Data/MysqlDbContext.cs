using Exercise4.Enums;
using Microsoft.EntityFrameworkCore;

namespace Exercise4.Data;

using Exercise4.Models;

public class MysqlDbContext : DbContext
{
    // Configuration Constructor
    public MysqlDbContext(DbContextOptions<MysqlDbContext> options) : base(options)
    {
        /* The configuration are inited by the constructor each time that the context
        is used, with the help of the service declaration in Program.cs */
    }

    // Models
    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Loan> Loans { get; set; }
    
    // Models Configurations
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Default Values
        modelBuilder.Entity<Book>()
            .Property(b => b.State)
            .HasDefaultValue(BookState.Available);
        modelBuilder.Entity<Book>()
            .Property(b => b.YearPublished)
            .HasDefaultValue(DateTime.Now.Year);
        modelBuilder.Entity<Loan>()
            .Property(l => l.LoanDate)
            .HasDefaultValue(DateOnly.FromDateTime(DateTime.Now)); // Returns just yy-mm-dd
        modelBuilder.Entity<Loan>()
            .Property(l => l.State)
            .HasDefaultValue(LoanState.Loaned);
        
        // Enum String Conversion
        modelBuilder.Entity<Book>()
            .Property(b => b.Gender)
            .HasConversion<string>();
        modelBuilder.Entity<Book>()
            .Property(b => b.State)
            .HasConversion<string>();
        modelBuilder.Entity<Loan>()
            .Property(p => p.State)
            .HasConversion<string>();
        
        // Foreign Keys
        modelBuilder.Entity<Loan>()
            .HasOne<User>(l => l.User)
            .WithMany(u => u.Loans)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Loan>()
            .HasOne<Book>(l => l.Book)
            .WithMany(b => b.Loans)
            .HasForeignKey(l => l.BookId)
            .OnDelete(DeleteBehavior.Cascade);
        /*
         EXPLANATION
         
          modelBuilder.Entity<Loan>() <-- Entity Loan
                   .HasOne<User>(l => l.User) <- Has one user, that is loan.User
                   .WithMany(u => u.Id) <- The user is waiting for many loans (1:n)
                   .HasForeignKey(l => l.UserId) <- Loan has foreign key, being the Id of user in loan.UserId
                   .OnDelete(DeleteBehavior.Cascade); <- On deleting a loan, use Delete in cascade way
           }
         */
    }
}