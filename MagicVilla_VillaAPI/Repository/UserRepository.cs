using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MagicVilla_VillaAPI.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;
    private string secretKey;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private IMapper _mapper;
    
    public UserRepository(ApplicationDbContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
    {
        _db = db;
        secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        _userManager = userManager;
        _mapper = mapper;
        _roleManager = roleManager;
    }

    public bool IsUniqueUser(string userName)
    {
        var user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName == userName);
        
        if (user == null)
        {
            return true;
        }
        return false;
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
    {
        var user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName.ToLower() == loginRequestDto.UserName.ToLower());

        var isValid = await _userManager.CheckPasswordAsync(user!, loginRequestDto.Password);
        
        if (user == null || isValid == false)
        {
            return new LoginResponseDto()
            {
                Token = string.Empty,
                User = null
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault()!)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        LoginResponseDto loginResponseDto = new()
        {
            User = _mapper.Map<UserDto>(user),
            Token = tokenHandler.WriteToken(token),
            Role = roles.FirstOrDefault()
        };
        
        return loginResponseDto;
    }

    public async Task<UserDto> Register(RegistrationRequestDto registrationRequestDto)
    {
        ApplicationUser user = new()
        {
            UserName = registrationRequestDto.UserName,
            Email = registrationRequestDto.UserName,
            NormalizedEmail = registrationRequestDto.UserName.ToUpper(),
            Name = registrationRequestDto.Name
        };

        try
        {
            var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
            if (result.Succeeded)
            {
                if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole("admin"));
                    await _roleManager.CreateAsync(new IdentityRole("custom"));
                }
                await _userManager.AddToRoleAsync(user, "admin");
                var userToReturn =
                    _db.ApplicationUsers.FirstOrDefault(u => u.UserName == registrationRequestDto.UserName);
                return _mapper.Map<UserDto>(userToReturn);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return new UserDto();
    }
}