using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services;

public class AuthService: IAuthService
{
    private readonly IHttpClientFactory _clientFactory;
    private string villaUrl;
    private readonly IBaseService _baseService;
    
    public AuthService(IHttpClientFactory clientFactory, IConfiguration configuration, IBaseService baseService)
    {
        _clientFactory = clientFactory;
        villaUrl = configuration.GetValue<string>("ServiceUrl:VillaAPI");
        _baseService = baseService;
    }

    public async Task<T> LoginAsync<T>(LoginRequestDto loginModel)
    { 
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.POST,
            Data = loginModel,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/UsersApi/login"
        }, withBearer:false);
    }

    public async Task<T> RegisterAsync<T>(RegistrationRequestDto registerModel)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.POST,
            Data = registerModel,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/UsersApi/register"
        }, withBearer:false);
    }
}