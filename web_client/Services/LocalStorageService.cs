using Microsoft.JSInterop;
using System.Text.Json;
using web_client.Models;

namespace web_client.Services
{
    /// <summary>
    /// Used to create/remove/Taking value from ex. browser Local Storage.
    /// </summary>
    public class LocalStorageService : ILocalStorageService
    {
        private readonly IJSRuntime JsRuntime;

        public LocalStorageService(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }

        private static string GetSorageByEnum(StoragePlace storagePlace)
        {
            return storagePlace == StoragePlace.Memory ? "localStorage" : "sessionStorage";
        }

        public async Task<T?> GetItemAsync<T>(string key, StoragePlace storagePlace)
        {
            var json = await JsRuntime.InvokeAsync<string>(GetSorageByEnum(storagePlace) + ".getItem", key);

            if (json == null)
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task SetItemAsync<T>(string key, T value, StoragePlace storagePlace)
        {
            await JsRuntime.InvokeVoidAsync(GetSorageByEnum(storagePlace) + ".setItem", key, JsonSerializer.Serialize(value));
        }

        public async Task RemoveItemAsync(string key, StoragePlace storagePlace)
        {
            await JsRuntime.InvokeVoidAsync(GetSorageByEnum(storagePlace) + ".removeItem", key);
        }
    }
}
