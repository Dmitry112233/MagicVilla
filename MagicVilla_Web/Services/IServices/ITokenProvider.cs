using MagicVilla_Web.Models.Dto;

namespace MagicVilla_Web.Services.IServices;

public interface ITokenProvider
{
    void SetToken(TokenDto tokenDto);
    TokenDto? GetToken();
    void ClearToken();
}