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

    public Task<T> GetAllAsync<T>()
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = ApiType.GET,
            Url =  villaUrl + "/api/VillaNumberApi"
        });
    }

    public Task<T> GetAsync<T>(int id)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = ApiType.GET,
            Url = villaUrl + $"/api/VillaNumberApi/{id}"
        });
    }

    public Task<T> CreateAsync<T>(VillaNumberCreateDto dto)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = ApiType.POST,
            Url = villaUrl + "/api/VillaNumberApi",
            Data = dto
        });
    }

    public Task<T> UpdateAsync<T>(VillaNumberUpdateDto dto)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = ApiType.PUT,
            Url = villaUrl + $"/api/VillaNumberApi/{dto.VillaNo}",
            Data = dto
        });
    }

    public Task<T> DeleteAsync<T>(int id)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = ApiType.DELETE,
            Url = villaUrl + $"/api/VillaNumberApi/{id}"
        });
    }
}