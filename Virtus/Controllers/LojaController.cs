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

        public async Task<IActionResult> Index(int pagIndex, string? buscar)
        {
            var produtos = await _produtoRepository.ProdutosOrdenados();

            // Busca
            if (!string.IsNullOrEmpty(buscar))
            {
                produtos = produtos
                    .Where(p => p.PrdNome.Contains(buscar, StringComparison.OrdinalIgnoreCase)
                             )
                    .ToList();

                pagIndex = 1; // reset para página 1
            }

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

        public async Task<IActionResult> Detalhes(int id)
        {
            var produtos = await _produtoRepository.ProdutosPorId(id);
            if (produtos == null)
            {
                return RedirectToAction("Index", "Loja");
            }

            return View(produtos);
        }

        public async Task<IActionResult> Filtro()
        {
            var categoriasETipos = await _produtoRepository.ObterCategoriasTipos();
            return View(categoriasETipos);
        }
    }
}
