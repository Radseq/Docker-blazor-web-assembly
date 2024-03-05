using web_client.Models;

namespace web_client.Services
{
    public interface ILocalStorageService
    {
        Task<T?> GetItemAsync<T>(string key, StoragePlace storagePlace = StoragePlace.Memory);
        Task SetItemAsync<T>(string key, T value, StoragePlace storagePlace = StoragePlace.Memory);
        Task RemoveItemAsync(string key, StoragePlace storagePlace = StoragePlace.Memory);
    }
}
