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
            var games = await gameService.ListGamesAsync(Request.Query);
            return View(new HomeViewModel
            {
                Games = games
            });
        }

        [Route("/DealDetail/{dealId}")]
        public async Task<IActionResult> DealDetailAsync(string dealId)
        {
            dealId = Uri.UnescapeDataString(dealId);
            var deal = await gameService.RetriveDealAsync(dealId);
            var store = await gameService.RetriveStore(deal.GameInfo.StoreID);

            return View(new DealDetailViewModel
            {
                Deal = deal,
                Store = store
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