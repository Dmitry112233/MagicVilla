using System.Security.Claims;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        LoginRequestDto obj = new ();
        return View(obj);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequestDto model)
    {
        var response = await _authService.LoginAsync<ApiResponse>(model);
        if (response != null && response.IsSuccess)
        {
            LoginResponseDto responseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result)!);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, responseDto.User.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, responseDto.User.Role));
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            
            HttpContext.Session.SetString(Sd.SessionToken, responseDto.Token);
            return RedirectToAction("Index", "Home");
        }
        ModelState.AddModelError("CustomError", response.ErrorMessages.FirstOrDefault());
        return View(model);
    }
    
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegistrationRequestDto model)
    {
        var response = await _authService.RegisterAsync<ApiResponse>(model);
        if (response != null && response.IsSuccess)
        {
            RedirectToAction(nameof(Login));
        }
        return View();
    }
    
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        HttpContext.Session.SetString(Sd.SessionToken, "");
        return RedirectToAction("Index", "Home");
    }
    
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}