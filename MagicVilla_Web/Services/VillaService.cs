using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services;

public class VillaService : IVillaService
{
    private readonly IHttpClientFactory _clientFactory;
    private string villaUrl;
    private readonly IBaseService _baseService;
    
    public VillaService(IHttpClientFactory clientFactory, IConfiguration configuration, IBaseService baseService)
    {
        _clientFactory = clientFactory;
        villaUrl = configuration.GetValue<string>("ServiceUrl:VillaAPI");
        _baseService = baseService;
    }

    public async Task<T> GetAllAsync<T>()
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.GET,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaApi"
        });
    }

    public async Task<T> GetAsync<T>(int id)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.GET,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaApi/" + id
        });
    }

    public async Task<T> CreateAsync<T>(VillaCreateDto dto)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.POST,
            Data = dto,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaApi",
            ContentType = Sd.ContentType.MultipartFormData
        });
    }

    public async Task<T> UpdateAsync<T>(VillaUpdateDto dto)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.PUT,
            Data = dto,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaApi/" + dto.Id,
            ContentType = Sd.ContentType.MultipartFormData
        });
    }

    public async Task<T> DeleteAsync<T>(int id)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.DELETE,
            Url = villaUrl + $"/api/{Sd.CurrentApiVersion}/VillaApi/" + id
        });
    }
}