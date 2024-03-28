using System.Net;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersionNeutral]
[ApiController]
public class UsersApiController : Controller
{
    private readonly IUserRepository _userRepo;
    private ApiResponse _response;

    public UsersApiController(IUserRepository userRepo)
    {
        _userRepo = userRepo;
        _response = new ApiResponse();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
    {
        var tokenDto = await _userRepo.Login(model);
        
        if (tokenDto == null || string.IsNullOrEmpty(tokenDto.AccessToken))
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Username or password is incorrect");
            return BadRequest(_response);
        }
        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.Result = tokenDto;
        return Ok(_response);
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
    {
        bool isUserNameUnique = _userRepo.IsUniqueUser(model.UserName);
        if (!isUserNameUnique)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Username is already exist");
            return BadRequest(_response);
        }

        var user = await _userRepo.Register(model);

        if (user == null)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Error while registering");
            return BadRequest(_response);
        }
        
        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        return Ok(_response);
    }
}