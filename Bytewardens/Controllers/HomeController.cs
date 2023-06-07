using Bytewardens.Data;
using Bytewardens.Handlers;
using Bytewardens.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Diagnostics;
using System.Text.Json;

namespace Bytewardens.Controllers
{
    [Route("/[action]")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGameService gameService;
        private readonly UserManager<BytewardensUser> userManager;
        private readonly SignInManager<BytewardensUser> signInManager;

        public HomeController(ILogger<HomeController> logger, IGameService gameService, UserManager<BytewardensUser> userManager, SignInManager<BytewardensUser> signInManager)
        {
            _logger = logger;
            this.gameService = gameService;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [Route("/"), HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var model = await gameService.ListGamesAsync(Request.Query);
            model.Stores = await gameService.ListStoresAsnyc();
            if (signInManager.IsSignedIn(User))
            {
                var dbUser = await userManager.GetUserAsync(User);
                if (dbUser != null && dbUser.Favorites != null)
                {
                    model.UserFavorites = JsonSerializer.Deserialize<List<string>>(dbUser.Favorites) ?? new();
                }
                else
                {
                    model.UserFavorites = new List<string>();
                }
            }
                
            return View(model);
        }

        [Route("/AddToFavorites"), HttpPost]
        public async Task<IActionResult> AddToFavorites([FromForm] string gameId)
        {
            var dbUser = await userManager.GetUserAsync(User);
            var favorites = new List<string>();
            if (dbUser.Favorites is not null)
            {
                favorites = JsonSerializer.Deserialize<List<string>>(dbUser.Favorites) ?? new();
            }
            if (!favorites.Contains(gameId))
            {
                favorites.Add(gameId);
            }
            dbUser.Favorites = JsonSerializer.Serialize(favorites);

            await userManager.UpdateAsync(dbUser);

            return Ok();
        }

        [Route("/RemoveFromFavorites"), HttpPost]
        public async Task<IActionResult> RemoveFromFavorites([FromForm] string gameId)
        {
            var dbUser = await userManager.GetUserAsync(User);
            var favorites = new List<string>();
            if (dbUser.Favorites is not null)
            {
                favorites = JsonSerializer.Deserialize<List<string>>(dbUser.Favorites) ?? new();
            }
            if (favorites.Contains(gameId))
            {
                favorites.Remove(gameId);
            }
            dbUser.Favorites = JsonSerializer.Serialize(favorites);

            await userManager.UpdateAsync(dbUser);

            return Ok();
        }

        public async Task<IActionResult> FavoritesAsync()
        {
            if (!signInManager.IsSignedIn(User))
                return Redirect("/Account/Login");

            var dbUser = await userManager.GetUserAsync(User);
            var favorites = new List<string>();
            if (dbUser.Favorites is not null)
            {
                favorites = JsonSerializer.Deserialize<List<string>>(dbUser.Favorites) ?? new();
            }

            var deals = await gameService.RetriveDealsForGames(favorites);

            return View("/Views/Home/Index.cshtml", new HomeViewModel
            {
                Games = deals,
                IsLoggedIn = true,
                MaxPages = 1,
                Stores = await gameService.ListStoresAsnyc(),
                UserFavorites = favorites,
                ServerSide = false,
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}