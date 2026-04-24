using Exercise4.Models;
using Exercise4.ViewModels;

namespace Exercise4.Controllers;

using Microsoft.AspNetCore.Mvc;
using Exercise4.Services;

public class LoansController : Controller
{
    private readonly LoansService _loansService;

    public LoansController(LoansService loansService)
    {
        _loansService = loansService;
    }
    
    // VIEWS
    public IActionResult Index()
    {
        // Getting all loans
        var loans = _loansService.GetAllLoans();

        LoanViewModel vm = new LoanViewModel
        {
            Loans = loans.Data
        };
        
        return View(vm);
    }

    public IActionResult Details(int id)
    {
        var loanFound = _loansService.GetLoanById(id);

        LoanViewModel vm = new LoanViewModel
        {
            Loan = loanFound.Data
        };
        
        return View(vm);
    }

    public IActionResult Edit(int id)
    {
        var loanFound = _loansService.GetLoanById(id);

        LoanViewModel vm = new LoanViewModel
        {
            NewLoan = loanFound.Data,
        };
        
        return View(vm);
    }
    
    // API CONTROLLERS
    [HttpPost]
    public IActionResult Create(LoanViewModel vm)
    {
        var loan = vm.NewLoan;
        
        // Checking that all fields are valid
        /* Ignored validations of model by unknown problem with ModelState ASP Entity
        if (!ModelState.IsValid)
        {
            // If not, return the view with the viewmodel that contains the error messages
            return View("Index", vm);
        } */
        
        // Adding loan
        var response = _loansService.AddLoan(loan);
        
        // Sending message
        TempData["ErrorMsg"] = response.Message;

        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public IActionResult Update(LoanViewModel vm)
    {
        var loan = vm.NewLoan;
        
        // Checking that all fields are valid
        /*if (!ModelState.IsValid)
        {
            // If not, return the view with the viewmodel that contains the error messages
            return View("Index", vm);
        }*/
        
        // Updating loan
        var response = _loansService.UpdateLoan(loan);

        // Sending message
        TempData["ErrorMsg"] = response.Message;

        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public IActionResult Delete(int id)
    {
        // Deleting loan
        var response = _loansService.DeleteLoan(id);
        
        // Sending message
        TempData["ErrorMsg"] = response.Message;

        return RedirectToAction("Index");
    }
}