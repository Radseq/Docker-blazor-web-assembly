using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace web_client.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient HttpClient;
        private readonly NavigationManager NavigationManager;
        private readonly ILocalStorageService LocalStorageService;

        public HttpService(HttpClient httpClient, NavigationManager navigationManager,
            ILocalStorageService localStorageService)
        {
            HttpClient = httpClient;
            NavigationManager = navigationManager;
            LocalStorageService = localStorageService;
        }
        public async Task Post(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Post, uri, value);
            await SendRequest(request);
        }

        public async Task<T> Post<T>(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Post, uri, value);
            return await SendRequest<T>(request);
        }

        public async Task Put(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Put, uri, value);
            await SendRequest(request);
        }

        public async Task<T> Put<T>(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Put, uri, value);
            return await SendRequest<T>(request);
        }

        private static HttpRequestMessage CreateRequest(HttpMethod method, string uri, object value = null)
        {
            var request = new HttpRequestMessage(method, uri);
            if (value != null)
            {
                var modelAsJson = JsonConvert.SerializeObject(value);
                request.Content = new StringContent(modelAsJson, Encoding.UTF8, "application/json");
            }
            return request;
        }

        private async Task SendRequest(HttpRequestMessage request)
        {
            await AddHeaderRequest(request);

            // send request
            using var response = await HttpClient.SendAsync(request);
            //Console.WriteLine($"authentication/login?returnUrl={Uri.EscapeDataString(NavigationManager.BaseUri)}");
            // auto logout on 401 response
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                NavigationManager.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(NavigationManager.BaseUri)}");
                await LocalStorageService.RemoveItemAsync("token");
                return;
            }

            await HandleErrors(response);
        }

        private async Task<T> SendRequest<T>(HttpRequestMessage request)
        {
            await AddHeaderRequest(request);
            //Console.WriteLine($"authentication/login?returnUrl={Uri.EscapeDataString(NavigationManager.BaseUri)}");
            // send request
            using var response = await HttpClient.SendAsync(request);

            // auto logout on 401 response
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                //NavigationManager.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(NavigationManager.BaseUri)}");
                //await LocalStorageService.RemoveItemAsync("token");
                return default;
            }
            var a = await response.Content.ReadAsStringAsync();
            Console.WriteLine(a);
            await HandleErrors(response);

            //var options = new JsonSerializerOptions
            //{
            //    PropertyNameCaseInsensitive = true
            //};
            //options.Converters.Add(new StringConverter());

            return await response.Content.ReadFromJsonAsync<T>();
        }

        private async Task AddHeaderRequest(HttpRequestMessage request)
        {
            request.Headers.Add("Access-Control-Allow-Origin", "*");
            request.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
            request.Headers.Add("Accept", "application/json");
            await AddAuthorization(request);
            //var result = await LocalStorageService.GetItemAsync<string>("blazorCulture");
            //if (!string.IsNullOrWhiteSpace(result))
            //    request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(result));
        }


        private async Task AddAuthorization(HttpRequestMessage request)
        {
            var result = await LocalStorageService.GetItemAsync<string>("token");

            if (!string.IsNullOrWhiteSpace(result))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result);
        }

        private static async Task HandleErrors(HttpResponseMessage response)
        {
            // throw exception on error response
            if (!response.IsSuccessStatusCode)
            {
                //ApiRequestRespondModelError errorResultModel = null;

                //using (StreamReader sr = new(new MemoryStream(await response.Content.ReadAsByteArrayAsync())))
                //{
                //    string content = sr.ReadToEnd(); // to debug
                //    byte[] bytes = Encoding.Default.GetBytes(content);
                //    string result = Encoding.UTF8.GetString(bytes);
                //    errorResultModel = global::System.Text.Json.JsonSerializer.Deserialize<ApiRequestRespondModelError>(result);
                //}

                //if (errorResultModel != null)
                //    throw new Exception(JsonConvert.SerializeObject(errorResultModel));
                //else
                //Console.WriteLine(response.er);
                throw new Exception("unknown error");
            }
        }

        public async Task<T> Get<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await SendRequest<T>(request);
        }

        public async Task<T> Delete<T>(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Delete, uri);
            return await SendRequest<T>(request);
        }

        public async Task Delete(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Delete, uri, value);
            await SendRequest(request);
        }

        public async Task Patch(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Patch, uri, value);
            await SendRequest(request);
        }

        public async Task<T> Patch<T>(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Patch, uri);
            return await SendRequest<T>(request);
        }
    }
}