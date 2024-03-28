using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Repository.IRepository;

public interface IUserRepository
{
    bool IsUniqueUser(string userName);
    
    Task<TokenDto> Login(LoginRequestDto loginRequestDto);
    
    Task<UserDto> Register(RegistrationRequestDto registrationRequestDto);
}