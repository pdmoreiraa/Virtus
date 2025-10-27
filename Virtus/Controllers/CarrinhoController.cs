using Microsoft.AspNetCore.Mvc;
using Virtus.Models;
using Virtus.Repository;
using Virtus.Services;

namespace Virtus.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly CarrinhoRepository _carrinhoRepository;
        private readonly decimal taxaEntrega = 30.00m;

        public CarrinhoController(IProdutoRepository produtoRepository, IConfiguration configuracao, 
            CarrinhoRepository carrinhoRepository, IPedidoRepository pedidoRepository)
        {
            _produtoRepository = produtoRepository;
            _carrinhoRepository = carrinhoRepository;
            _pedidoRepository = pedidoRepository;
 
        }

        // Página do carrinho
        public async Task<IActionResult> Index()
        {
            // Obter itens do carrinho
            var itensCarrinho = await AuxiliarCarrinho.ObterItensCarrinho(Request, Response, _produtoRepository)
                                ?? new List<ItemPedido>();

            decimal subtotal = AuxiliarCarrinho.ObterSubtotal(itensCarrinho);
            decimal taxaEntrega = 10m; // valor fixo, pode ajustar se necessário

            // Montar o model Carrinho
            var carrinho = new Carrinho
            {
                Itens = itensCarrinho,
                Subtotal = subtotal,
                TaxaEntrega = taxaEntrega
            };

            return View(carrinho);
        }


        [HttpGet]
        public async Task<IActionResult> Infos()
        {
            // Verificar se usuário está logado
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                return RedirectToAction("Login", "Usuario");
            }
            int usuarioId = Convert.ToInt32(usuarioIdStr);

            // Obter itens do carrinho (ItemPedido)
            var itensCarrinho = await AuxiliarCarrinho.ObterItensCarrinho(Request, Response, _produtoRepository)
                                ?? new List<ItemPedido>();

            decimal subtotal = AuxiliarCarrinho.ObterSubtotal(itensCarrinho);
            decimal taxaEntrega = 10m; // Valor fixo

            // Obter endereços do usuário
            var enderecos = _carrinhoRepository.ObterEnderecosPorUsuario(usuarioId) ?? new List<Endereco>();

            // Obter cartões do usuário
            var cartoes = _carrinhoRepository.ObterCartoesPorUsuario(usuarioId)?.ToList() ?? new List<Cartao>();


            // Montar o model Carrinho
            var carrinho = new Carrinho
            {
                Itens = itensCarrinho,   // <== agora é List<ItemPedido>
                Enderecos = enderecos,
                Cartoes = cartoes,
                Subtotal = subtotal,
                TaxaEntrega = taxaEntrega,
                NovoEndereco = new Endereco(),
                NovoCartao = new Cartao()
            };

            return View(carrinho);
        }



        [HttpPost]
        public async Task<IActionResult> SalvarEndereco(Endereco endereco)
        {
            // Verifica se o usuário está logado
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
                return RedirectToAction("Login", "Usuario");

            int usuarioId = Convert.ToInt32(usuarioIdStr);
            endereco.UsuarioId = usuarioId;

            // Se o model estiver inválido
            if (!ModelState.IsValid)
            {
                // Recarrega o carrinho
                var itensCarrinho = await AuxiliarCarrinho.ObterItensCarrinho(Request, Response, _produtoRepository)
                                    ?? new List<ItemPedido>();
                decimal subtotal = AuxiliarCarrinho.ObterSubtotal(itensCarrinho);
                decimal taxaEntrega = 10m;

                var enderecos = _carrinhoRepository.ObterEnderecosPorUsuario(usuarioId)?.ToList() ?? new List<Endereco>();
                var cartoes = _carrinhoRepository.ObterCartoesPorUsuario(usuarioId)?.ToList() ?? new List<Cartao>();

                var carrinho = new Carrinho
                {
                    Itens = itensCarrinho,
                    Enderecos = enderecos,
                    Cartoes = cartoes,
                    Subtotal = subtotal,
                    TaxaEntrega = taxaEntrega,
                    NovoEndereco = endereco,   // mantém os dados preenchidos
                    NovoCartao = new Cartao()
                };

                return View("Infos", carrinho);
            }

            try
            {
                _carrinhoRepository.AdicionarEndereco(endereco);
                return RedirectToAction("Infos");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro ao salvar o endereço: " + ex.Message);

                // Recarrega o carrinho em caso de erro
                var itensCarrinho = await AuxiliarCarrinho.ObterItensCarrinho(Request, Response, _produtoRepository)
                                    ?? new List<ItemPedido>();
                decimal subtotal = AuxiliarCarrinho.ObterSubtotal(itensCarrinho);
                decimal taxaEntrega = 10m;

                var enderecos = _carrinhoRepository.ObterEnderecosPorUsuario(usuarioId)?.ToList() ?? new List<Endereco>();
                var cartoes = _carrinhoRepository.ObterCartoesPorUsuario(usuarioId)?.ToList() ?? new List<Cartao>();

                var carrinho = new Carrinho
                {
                    Itens = itensCarrinho,
                    Enderecos = enderecos,
                    Cartoes = cartoes,
                    Subtotal = subtotal,
                    TaxaEntrega = taxaEntrega,
                    NovoEndereco = endereco,
                    NovoCartao = new Cartao()
                };

                return View("Infos", carrinho);
            }
        }


        [HttpPost]
        public async Task<IActionResult> SalvarCartao(Cartao cartao)
        {
            // Garante que o model foi recebido
            if (cartao == null)
            {
                ModelState.AddModelError("", "Não foi possível processar o cartão.");
                return RedirectToAction("Infos");
            }

            // Verifica se o usuário está logado
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
                return RedirectToAction("Login", "Usuario");

            int usuarioId = Convert.ToInt32(usuarioIdStr);
            cartao.UsuarioId = usuarioId;

            // Se o ModelState for inválido
            if (!ModelState.IsValid)
            {
                // Recarrega o carrinho completo
                var itensCarrinho = await AuxiliarCarrinho.ObterItensCarrinho(Request, Response, _produtoRepository)
                                    ?? new List<ItemPedido>();
                decimal subtotal = AuxiliarCarrinho.ObterSubtotal(itensCarrinho);
                decimal taxaEntrega = 10m;

                var enderecos = _carrinhoRepository.ObterEnderecosPorUsuario(usuarioId)?.ToList() ?? new List<Endereco>();
                var cartoes = _carrinhoRepository.ObterCartoesPorUsuario(usuarioId)?.ToList() ?? new List<Cartao>();

                var carrinho = new Carrinho
                {
                    Itens = itensCarrinho,
                    Enderecos = enderecos,
                    Cartoes = cartoes,
                    Subtotal = subtotal,
                    TaxaEntrega = taxaEntrega,
                    NovoEndereco = new Endereco(),
                    NovoCartao = cartao // mantém os dados preenchidos do cartão
                };

                return View("Infos", carrinho);
            }

            try
            {
                // Salvar cartão no banco
                _carrinhoRepository.AdicionarCartao(cartao);

                return RedirectToAction("Infos");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro ao salvar o cartão: " + ex.Message);

                // Recarrega o carrinho completo em caso de erro
                var itensCarrinho = await AuxiliarCarrinho.ObterItensCarrinho(Request, Response, _produtoRepository)
                                    ?? new List<ItemPedido>();
                decimal subtotal = AuxiliarCarrinho.ObterSubtotal(itensCarrinho);
                decimal taxaEntrega = 10m;

                var enderecos = _carrinhoRepository.ObterEnderecosPorUsuario(usuarioId)?.ToList() ?? new List<Endereco>();
                var cartoes = _carrinhoRepository.ObterCartoesPorUsuario(usuarioId)?.ToList() ?? new List<Cartao>();

                var carrinho = new Carrinho
                {
                    Itens = itensCarrinho,
                    Enderecos = enderecos,
                    Cartoes = cartoes,
                    Subtotal = subtotal,
                    TaxaEntrega = taxaEntrega,
                    NovoEndereco = new Endereco(),
                    NovoCartao = cartao
                };

                return View("Infos", carrinho);
            }


        }

        [HttpPost]
        public async Task<IActionResult> FinalizarPedido(int EnderecoSelecionadoId, string MetodoPagamento, int? CartaoSelecionadoId)
        {
            // Validar endereço
            if (EnderecoSelecionadoId <= 0)
                TempData["Erro"] = "Selecione um endereço antes de continuar.";

            // Obter usuário logado
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
                return RedirectToAction("Login", "Usuario");

            int usuarioId = Convert.ToInt32(usuarioIdStr);

            if (string.IsNullOrEmpty(MetodoPagamento))
            {
                ViewBag.ErroPagamento = "Por favor, selecione um método de pagamento antes de continuar.";
            }

            // Obter itens do carrinho
            var itensCarrinho = await AuxiliarCarrinho.ObterItensCarrinho(Request, Response, _produtoRepository);
            if (itensCarrinho == null || !itensCarrinho.Any())
                TempData["Erro"] = "Seu carrinho está vazio."; // carrinho vazio

            // Criar pedido
            var pedido = new Pedido
            {
                UsuarioId = usuarioId,
                EnderecoId = EnderecoSelecionadoId,
                MetodoPagamentoId = MetodoPagamento == "Pix" ? 2 : 1, // 1 = Cartão, 2 = Pix
                CartaoId = CartaoSelecionadoId,
                TaxaEntrega = 10m,
                StatusPedido = "Aguardando Pagamento",
                CriadoEm = DateTime.Now,
                Itens = itensCarrinho
            };

            try
            {
                await _pedidoRepository.AdicionarPedido(pedido);

                // Limpar carrinho
                AuxiliarCarrinho.SalvarCarrinho(Response, new Dictionary<int, int>());

                if (MetodoPagamento == "Pix")
                    return RedirectToAction("Pix", "Carrinho", new { pedidoId = pedido.Id });
                else
                    return RedirectToAction("ConfirmarCartao", "Carrinho", new { pedidoId = pedido.Id });
            }
            catch (Exception ex)
            {
                // Tratar erro
                TempData["Erro"] = "Erro ao finalizar pedido: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

    }
}
