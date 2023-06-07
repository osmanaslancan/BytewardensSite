using Bytewardens.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Bytewardens.Handlers
{
    public interface IGameService
    {
        Task<HomeViewModel> ListGamesAsync(IQueryCollection query);
        Task<DealRetriveResponse> RetriveDealAsync(string dealId);
        Task<List<Store>> ListStoresAsnyc();
        Task<Store?> RetriveStore(string storeId);
        Task<List<ListOfDealsResponse>?> RetriveDealsForGames(List<string> games);
    };

    public class GameService : IGameService
    {
        private class GameApiResponse<T>
        {
            public T? Body { get; set; }
            public HttpResponseMessage? Response { get; set; }
        }

        private readonly HttpClient httpClient;
        private readonly IOptions<GameApiOptions> options;
        private List<Store> stores;

        public GameService(HttpClient httpClient, IOptions<GameApiOptions> options)
        {
            this.httpClient = httpClient;
            this.options = options;
            httpClient.BaseAddress = new Uri(options.Value.ApiRoot);
        }

        private async Task<GameApiResponse<T>?> SendGetRequestAsync<T>(string path, Dictionary<string, string>? query = null)
        {
            var releativePath = path;
            if (query != null)
            {
                releativePath = QueryHelpers.AddQueryString(path, query!);
            }

            var response = await httpClient.GetAsync(releativePath);
            if (response.IsSuccessStatusCode)
            {
                return new GameApiResponse<T>
                {
                    Body = await response.Content.ReadFromJsonAsync<T>(),
                    Response = response,
                };
            }
            else
            {
                return default;
            }
        }

        Dictionary<string, string> KeyMapping = new()
        {
            { "Page", "pageNumber" },
            { "MaxPrice", "upperPrice" },
            { "Sort", "sortBy" },
            { "Desc", "desc" },
            { "FilterTitle", "title" }
        };

        public async Task<HomeViewModel> ListGamesAsync(IQueryCollection query)
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
            string? maxPagesString = response?.Response?.Headers.GetValues("X-Total-Page-Count")
                .FirstOrDefault();
            return new HomeViewModel() { Games = response?.Body ?? new(), MaxPages = maxPagesString != null ? int.Parse(maxPagesString) : null };
        }

        public async Task<DealRetriveResponse> RetriveDealAsync(string dealId)
        {
            var response = await SendGetRequestAsync<DealRetriveResponse>("deals", new()
            {
                { "id", dealId }
            });

            return response?.Body ?? new();
        }

        public async Task<List<Store>> ListStoresAsnyc()
        {
            if (stores == null)
            {
                var response = await SendGetRequestAsync<List<Store>>("stores");
                stores = response?.Body ?? new(); ;
            }

            return stores;
        }

        public async Task<Store?> RetriveStore(string storeId)
        {
            var stores = await ListStoresAsnyc();

            return stores.FirstOrDefault(x => x.StoreID == storeId);
        }

        public async Task<List<ListOfDealsResponse>?> RetriveDealsForGames(List<string> games)
        {
            var lookupResponse = await SendGetRequestAsync<List<GameLookupResponse>>("games", new Dictionary<string, string>
            {
                { "format", "array" },
                { "ids", string.Join(",", games) }
            });

            var deals = lookupResponse?.Body?.SelectMany(game => game.Deals.Select(deal => new ListOfDealsResponse
            {
                DealID = deal.DealID,
                DealRating = null,
                GameID = game.Info.GameID,
                StoreID = deal.StoreID,
                NormalPrice = deal.RetailPrice,
                SalePrice = deal.Price,
                Savings = deal.Savings,
                Thumb = game.Info.Thumb,
                SteamAppID = game.Info.SteamAppID,
                Title = game.Info.Title,
            })).ToList();

            return deals ?? new();
        }
    }
}
