using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using static MagicVilla_Utility.Sd;

namespace MagicVilla_Web.Services;

public class VillaNumberService : BaseService, IVillaNumberService
{
    private readonly IHttpClientFactory _clientFactory;
    private string villaUrl;
    
    public VillaNumberService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
    {
        _clientFactory = clientFactory;
        villaUrl = configuration.GetValue<string>("ServiceUrl:VillaAPI");
    }

    public Task<T> GetAllAsync<T>(string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = ApiType.GET,
            Url =  villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaNumberApi",
            Token = token
        });
    }

    public Task<T> GetAsync<T>(int id, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = ApiType.GET,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaNumberApi/{id}",
            Token = token
        });
    }

    public Task<T> CreateAsync<T>(VillaNumberCreateDto dto, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = ApiType.POST,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaNumberApi",
            Data = dto,
            Token = token
        });
    }

    public Task<T> UpdateAsync<T>(VillaNumberUpdateDto dto, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = ApiType.PUT,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaNumberApi/{dto.VillaNo}",
            Data = dto,
            Token = token
        });
    }

    public Task<T> DeleteAsync<T>(int id, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = ApiType.DELETE,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaNumberApi/{id}",
            Token = token
        });
    }
}