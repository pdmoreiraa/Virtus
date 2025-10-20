using Microsoft.AspNetCore.Mvc;
using Virtus.Repository;

namespace Virtus.Controllers
{
    public class LojaController : Controller
    {

        private readonly ProdutoRepository _produtoRepository;

        public LojaController(ProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<IActionResult> Index()
        {
            var produtos = await _produtoRepository.ProdutosOrdenados();

            return View(produtos);
        }
    }
}
