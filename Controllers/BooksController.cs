using Exercise4.Models;
using Exercise4.ViewModels;

namespace Exercise4.Controllers;

using Microsoft.AspNetCore.Mvc;
using Exercise4.Services;

public class BooksController : Controller
{
    private readonly BooksService _booksService;

    public BooksController(BooksService booksService)
    {
        _booksService = booksService;
    }
    
    // VIEWS
    public IActionResult Index()
    {
        // Getting all books
        var books = _booksService.GetAllBooks();

        BookViewModel vm = new BookViewModel
        {
            Books = books.Data
        };
        
        return View(vm);
    }

    public IActionResult Details(int id)
    {
        var bookFound = _booksService.GetBookById(id);

        BookViewModel vm = new BookViewModel
        {
            Book = bookFound.Data
        };
        
        return View(vm);
    }

    public IActionResult Edit(int id)
    {
        var bookFound = _booksService.GetBookById(id);

        BookViewModel vm = new BookViewModel
        {
            NewBook = bookFound.Data,
        };
        
        return View(vm);
    }
    
    // API CONTROLLERS
    [HttpPost]
    public IActionResult Create(BookViewModel vm)
    {
        var book = vm.NewBook;
        
        // Checking that all fields are valid
        /* Ignored validations of model by unknown problem with ModelState ASP Entity
        if (!ModelState.IsValid)
        {
            // If not, return the view with the viewmodel that contains the error messages
            return View("Index", vm);
        } */
        
        // Adding book
        var response = _booksService.AddBook(book);
        
        // Sending message
        TempData["ErrorMsg"] = response.Message;

        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public IActionResult Update(BookViewModel vm)
    {
        var book = vm.NewBook;
        
        // Checking that all fields are valid
        /*if (!ModelState.IsValid)
        {
            // If not, return the view with the viewmodel that contains the error messages
            return View("Index", vm);
        }*/
        
        // Updating book
        var response = _booksService.UpdateBook(book);

        // Sending message
        TempData["ErrorMsg"] = response.Message;

        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public IActionResult Delete(int id)
    {
        // Deleting book
        var response = _booksService.DeleteBook(id);
        
        // Sending message
        TempData["ErrorMsg"] = response.Message;

        return RedirectToAction("Index");
    }
}