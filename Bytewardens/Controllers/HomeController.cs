using Bytewardens.Handlers;
using Bytewardens.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bytewardens.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGameService gameService;

        public HomeController(ILogger<HomeController> logger, IGameService gameService)
        {
            _logger = logger;
            this.gameService = gameService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var games = await gameService.ListGamesAsync(0);
            return View(new HomeViewModel
            {
                Games = games
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