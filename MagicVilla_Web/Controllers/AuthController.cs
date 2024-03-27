using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(responseDto.Token);
            
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type=="unique_name")!.Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type=="role")!.Value));
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
        var roleList = new List<SelectListItem>()
        {
            new() { Text = Sd.Admin, Value = Sd.Admin },
            new() { Text = Sd.Customer, Value = Sd.Customer }
        };
        ViewBag.RoleList = roleList;
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegistrationRequestDto model)
    {
        if (string.IsNullOrEmpty(model.Role))
        {
            model.Role = Sd.Customer;
        }
            
        var response = await _authService.RegisterAsync<ApiResponse>(model);
        if (response != null && response.IsSuccess)
        {
           return RedirectToAction("Login");
        }
        var roleList = new List<SelectListItem>()
        {
            new() { Text = Sd.Admin, Value = Sd.Admin },
            new() { Text = Sd.Customer, Value = Sd.Customer }
        };
        ViewBag.RoleList = roleList;
        ModelState.AddModelError("CustomError", response.ErrorMessages.FirstOrDefault());
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