using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Virtus.Models;
using Virtus.Repository;

namespace Virtus.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProdutoRepository _produtoRepository;

        public HomeController(ILogger<HomeController> logger, ProdutoRepository produtoRepositorio)
        {
            _logger = logger;
            _produtoRepository = produtoRepositorio;
        }

        public async Task<IActionResult> Index()
        {
            var produtos = await _produtoRepository.ProdutosOrdenados();
            return View(produtos);
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
