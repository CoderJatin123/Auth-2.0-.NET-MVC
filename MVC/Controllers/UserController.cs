using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers;

public class UserController : Controller
{
    public IActionResult Login()
    {
        return View();
    }
}