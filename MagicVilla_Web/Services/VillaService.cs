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

    public Task<T> GetAllAsync<T>()
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.GET,
            Url = villaUrl + "/api/VillaApi"
        });
    }

    public Task<T> GetAsync<T>(int id)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.GET,
            Url = villaUrl + "/api/VillaApi/" + id
        });
    }

    public Task<T> CreateAsync<T>(VillaCreateDto dto)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.POST,
            Data = dto,
            Url = villaUrl + "/api/VillaApi"
        });
    }

    public Task<T> UpdateAsync<T>(VillaUpdateDto dto)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.PUT,
            Data = dto,
            Url = villaUrl + "/api/VillaApi/" + dto.Id
        });
    }

    public Task<T> DeleteAsync<T>(int id)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.DELETE,
            Url = villaUrl + "/api/VillaApi/" + id
        });
    }
}