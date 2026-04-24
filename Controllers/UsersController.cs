using Exercise4.Models;
using Exercise4.ViewModels;

namespace Exercise4.Controllers;

using Microsoft.AspNetCore.Mvc;
using Exercise4.Services;

public class UsersController : Controller
{
    private readonly UsersService _usersService;

    public UsersController(UsersService usersService)
    {
        _usersService = usersService;
    }
    
    // VIEWS
    public IActionResult Index()
    {
        // Getting all users
        var users = _usersService.GetAllUsers();

        UserViewModel vm = new UserViewModel
        {
            Users = users.Data
        };
        
        return View(vm);
    }

    public IActionResult Details(int id)
    {
        var userFound = _usersService.GetUserById(id);

        UserViewModel vm = new UserViewModel
        {
            User = userFound.Data
        };
        
        return View(vm);
    }

    public IActionResult Edit(int id)
    {
        var userFound = _usersService.GetUserById(id);

        UserViewModel vm = new UserViewModel
        {
            NewUser = userFound.Data,
        };
        
        return View(vm);
    }
    
    // API CONTROLLERS
    [HttpPost]
    public IActionResult Create(UserViewModel vm)
    {
        var user = vm.NewUser;
        
        Console.WriteLine(user.Name);
        Console.WriteLine(user.LastName);
        
        /*
         Ignored validations of model by unknown problem with ModelState ASP Entity
         if (!ModelState.IsValid)
        {
            return View("Index", vm);
        }*/
        
        var response = _usersService.AddUser(user);
        TempData["ErrorMsg"] = response.Message;

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Update(UserViewModel vm)
    {
        var user = vm.NewUser;
        
        // Checking that all fiedls are valid
        /*if (!ModelState.IsValid)
        {
            // If not, return the view with the viewmodel that contains the error messages
            return View("Index", vm);
        }*/
        
        // Updating user
        var response = _usersService.UpdateUser(user);

        // Sending message
        TempData["ErrorMsg"] = response.Message;

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        // Deleting user
        var response = _usersService.DeleteUser(id);
        
        // Sending message
        TempData["ErrorMsg"] = response.Message;

        return RedirectToAction("Index");
    }
}