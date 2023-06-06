using Bytewardens.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Bytewardens.Handlers
{
    public interface IGameService
    {
        Task<List<ListOfDealsResponse>> ListGamesAsync(IQueryCollection query);
        Task<DealRetriveResponse> RetriveDealAsync(string dealId);
        Task<List<Store>> ListStoresAsnyc();
        Task<Store?> RetriveStore(string storeId);
    };

    public class GameService : IGameService
    {
        private readonly HttpClient httpClient;
        private readonly IOptions<GameApiOptions> options;

        public GameService(HttpClient httpClient, IOptions<GameApiOptions> options)
        {
            this.httpClient = httpClient;
            this.options = options;
            httpClient.BaseAddress = new Uri(options.Value.ApiRoot);
        }

        private async Task<T?> SendGetRequestAsync<T>(string path, Dictionary<string, string>? query = null)
        {
            var releativePath = path;
            if (query != null)
            {
                releativePath = QueryHelpers.AddQueryString(path, query!);
            }

            var response = await httpClient.GetAsync(releativePath);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            else
            {
                return default;
            }
        }

        Dictionary<string, string> KeyMapping = new()
        {
            { "Page", "pageNumber" },
            { "MaxPrice", "upperPrice" }
        };

        public async Task<List<ListOfDealsResponse>> ListGamesAsync(IQueryCollection query)
        {
            var requestQuery = new Dictionary<string, string>();

            foreach (var key in query.Keys)
            {
                if (KeyMapping.ContainsKey(key))
                {
                    requestQuery.Add(KeyMapping[key], query[key].ToString());
                    continue;
                }
            }

            var response = await SendGetRequestAsync<List<ListOfDealsResponse>>("deals", requestQuery);

            return response ?? new();
        }

        public async Task<DealRetriveResponse> RetriveDealAsync(string dealId)
        {
            var response = await SendGetRequestAsync<DealRetriveResponse>("deals", new()
            {
                { "id", dealId }
            });

            return response ?? new();
        }

        public async Task<List<Store>> ListStoresAsnyc()
        {
            var response = await SendGetRequestAsync<List<Store>>("stores");

            return response ?? new();
        }

        public async Task<Store?> RetriveStore(string storeId)
        {
            var stores = await ListStoresAsnyc();

            return stores.FirstOrDefault(x => x.StoreID == storeId);
        }
    }
}
