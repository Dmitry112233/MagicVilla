using System.Text;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;
using static MagicVilla_Utility.Sd;

namespace MagicVilla_Web.Services;

public class BaseService : IBaseService
{
    public ApiResponse responseModel { get; set; }
    
    public IHttpClientFactory httpClient { get; set; }

    public BaseService(IHttpClientFactory httpClient)
    {
        responseModel = new();
        this.httpClient = httpClient;
    }
    
    public async Task<T> SendAsync<T>(ApiRequest apiRequest)
    {
        try
        {
            var client = httpClient.CreateClient("MagicApi");
            HttpRequestMessage message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new Uri(apiRequest.Url);
            
            if (apiRequest.Data != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
            }

            switch (apiRequest.ApiType)
            {
                case ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;
                case ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                case ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            HttpResponseMessage apiResponse = null;
            apiResponse = await client.SendAsync(message);
            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            
            var response = JsonConvert.DeserializeObject<T>(apiContent);

            return response;
        }
        catch (Exception e)
        {
            var dto = new ApiResponse
            {
                ErrorMessages = new List<string> { e.Message },
                IsSuccess = false
            };

            var serializedDto = JsonConvert.SerializeObject(dto);
            var response = JsonConvert.DeserializeObject<T>(serializedDto);
            
            return response;
        }
    }
}