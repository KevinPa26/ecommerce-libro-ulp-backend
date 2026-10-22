using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_libro_ulp_backend.Controllers;

[AllowAnonymous]
[Route("Auth")]
public class AuthController : Controller
{
    [HttpGet("Login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet("Registro")]
    public IActionResult Registro()
    {
        return View();
    }

    [HttpGet("Perfil")]
    public IActionResult Perfil()
    {
        return View();
    }
}