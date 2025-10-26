using Microsoft.AspNetCore.Mvc;
using Virtus.Repository;
using Virtus.Services;

namespace Virtus.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly decimal taxaEntrega = 30.00m;

        public CarrinhoController(IProdutoRepository produtoRepository, IConfiguration configuracao)
        {
            _produtoRepository = produtoRepository;
 
        }

        // Página do carrinho
        public async Task<IActionResult> Index()
        {
            var itensCarrinho = await AuxiliarCarrinho.ObterItensCarrinho(Request, Response, _produtoRepository);
            decimal subtotal = AuxiliarCarrinho.ObterSubtotal(itensCarrinho);

            ViewBag.ItensCarrinho = itensCarrinho;
            ViewBag.TaxaEntrega = taxaEntrega;
            ViewBag.Subtotal = subtotal;
            ViewBag.Total = subtotal + taxaEntrega;

            return View();
        }
    }
}
