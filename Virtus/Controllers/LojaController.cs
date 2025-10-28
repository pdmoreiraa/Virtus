using Microsoft.AspNetCore.Mvc;
using Virtus.Models;
using Virtus.Repository;

namespace Virtus.Controllers
{
    public class LojaController : Controller
    {

        private readonly IProdutoRepository _produtoRepository;
        private readonly int pagTam = 8;

        public LojaController(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<IActionResult> Index(int pagIndex)
        {
            var produtos = await _produtoRepository.ProdutosOrdenados();

            if (pagIndex < 1) pagIndex = 1;

            // Total de itens e páginas
            int count = produtos.Count();
            int totalPag = (int)Math.Ceiling((decimal)count / pagTam);

            // Garantir que a página atual não ultrapasse o total de páginas (ou seja 1 se não houver registros)
            if (totalPag == 0) totalPag = 1;
            if (pagIndex > totalPag) pagIndex = totalPag;

            // Pegar produtos da página atual
            var produtosPaginados = produtos
                .Skip((pagIndex - 1) * pagTam)
                .Take(pagTam)
                .ToList();


            // Dados para a view
            ViewData["PagIndex"] = pagIndex;
            ViewData["TotalPag"] = totalPag;

            return View(produtosPaginados);
        }

        public async Task<IActionResult> Detalhes(Produto produto)
        {
            var produtos = await _produtoRepository.ProdutosPorId(produto.Id);
            if (produtos == null)
            {
                return RedirectToAction("index", "Store");
            }

            return View(produtos);
        }
    }
}
