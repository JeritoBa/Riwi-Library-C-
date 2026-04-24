using Exercise4.Models;

namespace Exercise4.ViewModels;

public class UserViewModel
{
    public IEnumerable<User> Users { get; set; } = new List<User>();
    public User NewUser { get; set; } = new();
    public User User { get; set; } = new();
}