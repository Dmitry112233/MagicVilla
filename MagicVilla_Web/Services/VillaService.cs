using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services;

public class VillaService : BaseService, IVillaService
{
    private readonly IHttpClientFactory _clientFactory;
    private string villaUrl;
    
    public VillaService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
    {
        _clientFactory = clientFactory;
        villaUrl = configuration.GetValue<string>("ServiceUrl:VillaAPI");
    }

    public Task<T> GetAllAsync<T>(string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.GET,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaApi",
            Token = token
        });
    }

    public Task<T> GetAsync<T>(int id, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.GET,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaApi/" + id,
            Token = token
        });
    }

    public Task<T> CreateAsync<T>(VillaCreateDto dto, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.POST,
            Data = dto,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaApi",
            Token = token
        });
    }

    public Task<T> UpdateAsync<T>(VillaUpdateDto dto, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.PUT,
            Data = dto,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaApi/" + dto.Id,
            Token = token
        });
    }

    public Task<T> DeleteAsync<T>(int id, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.DELETE,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaApi/" + id,
            Token = token
        });
    }
}