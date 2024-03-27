using System.Net;
using System.Net.Http.Headers;
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
            if (apiRequest.ContentType == ContentType.MultipartFormData)
            {
                message.Headers.Add("Accept", "*/*");
            }
            else
            {
                message.Headers.Add("Accept", "application/json");   
            }
            message.RequestUri = new Uri(apiRequest.Url);

            if (apiRequest.ContentType == ContentType.MultipartFormData)
            {
                var content = new MultipartFormDataContent();
                foreach (var prop in apiRequest.Data.GetType().GetProperties())
                {
                    var value = prop.GetValue(apiRequest.Data);
                    if (value is FormFile)
                    {
                        var file = (FormFile)value;
                        if (file != null)
                        {
                            content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
                        }
                    }
                    else
                    {
                        content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
                    }
                }

                message.Content = content;
            }
            else
            {
                if (apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8,
                        "application/json");
                }   
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

            if (!string.IsNullOrEmpty(apiRequest.Token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.Token);
            }

            apiResponse = await client.SendAsync(message);
            var apiContent = await apiResponse.Content.ReadAsStringAsync();

            try
            {
                ApiResponse responseApi = JsonConvert.DeserializeObject<ApiResponse>(apiContent);
                if (responseApi != null && (apiResponse.StatusCode == HttpStatusCode.BadRequest ||
                                            apiResponse.StatusCode == HttpStatusCode.NotFound))
                {
                    responseApi.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    responseApi.IsSuccess = false;
                    var serializedDto = JsonConvert.SerializeObject(responseApi);
                    var returnObj = JsonConvert.DeserializeObject<T>(serializedDto);

                    return returnObj;
                }
            }
            catch (Exception e)
            {
                var exceptionResponse = JsonConvert.DeserializeObject<T>(apiContent);
                return exceptionResponse;
            }

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