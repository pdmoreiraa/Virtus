using Microsoft.AspNetCore.Mvc;
using Virtus.Repository;

namespace Virtus.Controllers
{
    public class ProdutoController : Controller
    {

        private readonly ProdutoRepository _produtoRepository;

        public ProdutoController(ProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }
        public async Task<IActionResult> Index()
        {
            var produtos = await _produtoRepository.ProdutosOrdenados();

            return View(produtos);
        }

        public IActionResult Criar()
        {
            return View();
        }
    }
}
