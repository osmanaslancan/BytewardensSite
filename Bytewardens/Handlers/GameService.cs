using Bytewardens.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Bytewardens.Handlers
{
    public interface IGameService 
    {
        Task<List<ListOfDealsResponse>> ListGamesAsync(int page);
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

        public async Task<List<ListOfDealsResponse>> ListGamesAsync(int page)
        {
            var response = await SendGetRequestAsync<List<ListOfDealsResponse>>("deals", new() {
                { "pageNumber", page.ToString() }
            });

            return response ?? new();
        }
    }
}
