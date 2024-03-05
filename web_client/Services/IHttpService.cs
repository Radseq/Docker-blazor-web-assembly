namespace web_client.Services
{
    public interface IHttpService
    {
        Task Post(string uri, object value = null);
        Task<T> Post<T>(string uri, object value = null);
        Task<T> Get<T>(string uri);
        Task Put(string uri, object value = null);
        Task<T> Put<T>(string uri, object value);
        Task Delete(string uri, object value = null);
        Task<T> Delete<T>(string uri, object value = null);
        Task Patch(string uri, object value = null);
        Task<T> Patch<T>(string uri, object value = null);
    }
}
