using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services;

public class AuthService: BaseService, IAuthService
{
    private readonly IHttpClientFactory _clientFactory;
    private string villaUrl;
    
    public AuthService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
    {
        _clientFactory = clientFactory;
        villaUrl = configuration.GetValue<string>("ServiceUrl:VillaAPI");
    }

    public Task<T> LoginAsync<T>(LoginRequestDto loginModel)
    { 
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.POST,
            Data = loginModel,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/UsersApi/login"
        });
    }

    public Task<T> RegisterAsync<T>(RegistrationRequestDto registerModel)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.POST,
            Data = registerModel,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/UsersApi/register"
        });
    }
}