using Microsoft.AspNetCore.Mvc;
using Virtus.Models;
using Virtus.Repository;
using Virtus.Services;

namespace Virtus.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly CarrinhoRepository _carrinhoRepository;
        private readonly decimal taxaEntrega = 30.00m;

        public CarrinhoController(IProdutoRepository produtoRepository, IConfiguration configuracao, CarrinhoRepository carrinhoRepository)
        {
            _produtoRepository = produtoRepository;
            _carrinhoRepository = carrinhoRepository;
 
        }

        // Página do carrinho
        public async Task <IActionResult> Index()
        {
            var itensCarrinho = await AuxiliarCarrinho.ObterItensCarrinho(Request, Response, _produtoRepository);
            decimal subtotal = AuxiliarCarrinho.ObterSubtotal(itensCarrinho);

            ViewBag.ItensCarrinho = itensCarrinho;
            ViewBag.TaxaEntrega = taxaEntrega;
            ViewBag.Subtotal = subtotal;
            ViewBag.Total = subtotal + taxaEntrega;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Infos()
        {
            // Obter itens do carrinho
            var itensCarrinho = await AuxiliarCarrinho.ObterItensCarrinho(Request, Response, _produtoRepository);
            decimal subtotal = AuxiliarCarrinho.ObterSubtotal(itensCarrinho);
            decimal taxaEntrega = 10m; // Exemplo fixo

            // Verificar se usuário está logado
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                return RedirectToAction("Login", "Usuario");
            }
            int usuarioId = Convert.ToInt32(usuarioIdStr);

            // Obter endereços do usuário
            var enderecos = _carrinhoRepository.ObterEnderecosPorUsuario(usuarioId) ?? new List<Endereco>();

            // Passar dados para a view via ViewBag
            ViewBag.Enderecos = enderecos;
            ViewBag.ItensCarrinho = itensCarrinho;
            ViewBag.TaxaEntrega = taxaEntrega;
            ViewBag.Subtotal = subtotal;
            ViewBag.Total = subtotal + taxaEntrega;
            ViewBag.Pix = ((subtotal + taxaEntrega) * 0.95m).ToString("F2");
            ViewBag.NovoEndereco = new Endereco();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SalvarEndereco(Endereco endereco)
        {
            // Garante que o model foi recebido
            if (endereco == null)
            {
                ModelState.AddModelError("", "Não foi possível processar o endereço.");
                return RedirectToAction("Infos");
            }

            // Verifica se o usuário está logado
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                return RedirectToAction("Login", "Usuario");
            }

            int usuarioId = Convert.ToInt32(usuarioIdStr);
            endereco.UsuarioId = usuarioId;

            if (!ModelState.IsValid)
            {
                // Se houver erro de validação, recarrega a view com os dados
                var itensCarrinho = await AuxiliarCarrinho.ObterItensCarrinho(Request, Response, _produtoRepository);
                decimal subtotal = AuxiliarCarrinho.ObterSubtotal(itensCarrinho);
                decimal taxaEntrega = 10m;

                var enderecos = _carrinhoRepository.ObterEnderecosPorUsuario(usuarioId) ?? new List<Endereco>();

                ViewBag.Enderecos = enderecos;
                ViewBag.ItensCarrinho = itensCarrinho;
                ViewBag.TaxaEntrega = taxaEntrega;
                ViewBag.Subtotal = subtotal;
                ViewBag.Total = subtotal + taxaEntrega;
                ViewBag.Pix = ((subtotal + taxaEntrega) * 0.95m).ToString("F2");
                ViewBag.NovoEndereco = endereco;

                return View("Infos");
            }

            try
            {
                _carrinhoRepository.AdicionarEndereco(endereco);
                return RedirectToAction("Infos");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro ao salvar o endereço: " + ex.Message);

                var itensCarrinho = await AuxiliarCarrinho.ObterItensCarrinho(Request, Response, _produtoRepository);
                decimal subtotal = AuxiliarCarrinho.ObterSubtotal(itensCarrinho);
                decimal taxaEntrega = 10m;

                var enderecos = _carrinhoRepository.ObterEnderecosPorUsuario(usuarioId) ?? new List<Endereco>();

                ViewBag.Enderecos = enderecos;
                ViewBag.ItensCarrinho = itensCarrinho;
                ViewBag.TaxaEntrega = taxaEntrega;
                ViewBag.Subtotal = subtotal;
                ViewBag.Total = subtotal + taxaEntrega;
                ViewBag.Pix = ((subtotal + taxaEntrega) * 0.95m).ToString("F2");
                ViewBag.NovoEndereco = endereco;

                return View("Infos");
            }
        }
    }
}
