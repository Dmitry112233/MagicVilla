using MagicVilla_Utility;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services;

public class TokenProvider : ITokenProvider
{
    private readonly IHttpContextAccessor _contextAccessor;
    
    public TokenProvider(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }
    
    public void SetToken(TokenDto tokenDto)
    {
        var cookieOption = new CookieOptions { Expires = DateTime.UtcNow.AddDays(60) };
        _contextAccessor.HttpContext?.Response.Cookies.Append(Sd.AccessToken, tokenDto.AccessToken, cookieOption);
    }

    public TokenDto GetToken()
    {
        try
        {
            bool hasAccessToken =
                _contextAccessor.HttpContext.Request.Cookies.TryGetValue(Sd.AccessToken, out string accessToken);
            var tokenDto = new TokenDto()
            {
                AccessToken = accessToken
            };
            return hasAccessToken ? tokenDto : null;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public void ClearToken()
    {
        _contextAccessor.HttpContext?.Response.Cookies.Delete(Sd.AccessToken);
    }
}