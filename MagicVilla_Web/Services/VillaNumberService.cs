using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using static MagicVilla_Utility.Sd;

namespace MagicVilla_Web.Services;

public class VillaNumberService : IVillaNumberService
{
    private readonly IHttpClientFactory _clientFactory;
    private string villaUrl;
    private readonly IBaseService _baseService;
    
    public VillaNumberService(IHttpClientFactory clientFactory, IConfiguration configuration, IBaseService baseService)
    {
        _clientFactory = clientFactory;
        villaUrl = configuration.GetValue<string>("ServiceUrl:VillaAPI");
        _baseService = baseService;
    }

    public async Task<T> GetAllAsync<T>()
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = ApiType.GET,
            Url =  villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaNumberApi"
        });
    }

    public async Task<T> GetAsync<T>(int id)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = ApiType.GET,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaNumberApi/{id}"
        });
    }

    public async Task<T> CreateAsync<T>(VillaNumberCreateDto dto)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = ApiType.POST,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaNumberApi",
            Data = dto
        });
    }

    public async Task<T> UpdateAsync<T>(VillaNumberUpdateDto dto)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = ApiType.PUT,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaNumberApi/{dto.VillaNo}",
            Data = dto
        });
    }

    public async Task<T> DeleteAsync<T>(int id)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = ApiType.DELETE,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaNumberApi/{id}"
        });
    }
}