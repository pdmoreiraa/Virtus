using Microsoft.AspNetCore.Mvc;
using System.Globalization;
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

        public CarrinhoController(IProdutoRepository produtoRepository, 
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
            var itensCarrinho = await ArmazenarCarrinho.Itens(Request, Response, _produtoRepository)
                                ?? new List<ItemPedido>();

            decimal subtotal = ArmazenarCarrinho.Subtotal(itensCarrinho);
            decimal taxaEntrega = 10m;

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
            var itensCarrinho = await ArmazenarCarrinho.Itens(Request, Response, _produtoRepository)
                                ?? new List<ItemPedido>();

            decimal subtotal = ArmazenarCarrinho.Subtotal(itensCarrinho);
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
                var itensCarrinho = await ArmazenarCarrinho.Itens(Request, Response, _produtoRepository)
                                    ?? new List<ItemPedido>();
                decimal subtotal = ArmazenarCarrinho.Subtotal(itensCarrinho);
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
                var itensCarrinho = await ArmazenarCarrinho.Itens(Request, Response, _produtoRepository)
                                    ?? new List<ItemPedido>();
                decimal subtotal = ArmazenarCarrinho.Subtotal(itensCarrinho);
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
                var itensCarrinho = await ArmazenarCarrinho.Itens(Request, Response, _produtoRepository)
                                    ?? new List<ItemPedido>();
                decimal subtotal = ArmazenarCarrinho.Subtotal(itensCarrinho);
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
                var itensCarrinho = await ArmazenarCarrinho.Itens(Request, Response, _produtoRepository)
                                    ?? new List<ItemPedido>();
                decimal subtotal = ArmazenarCarrinho.Subtotal(itensCarrinho);
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
        public async Task<IActionResult> FinalizarPedido(int EnderecoSelecionadoId, string MetodoPagamento, int? CartaoId, 
            int ParcelasSelecionadas, decimal ValorParcela, Carrinho carrinho)
        {
            // Validar endereço
            if (EnderecoSelecionadoId <= 0)
            {
                TempData["Erro"] = "Selecione um endereço antes de continuar.";
                return RedirectToAction("Infos");
            }

            // Obter usuário logado
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
                return RedirectToAction("Login", "Usuario");

            int usuarioId = Convert.ToInt32(usuarioIdStr);



            // Validar método de pagamento
            if (string.IsNullOrEmpty(MetodoPagamento))
            {
                TempData["Erro"] = "Por favor, selecione um método de pagamento antes de continuar.";
                return RedirectToAction("Infos");
            }

            // Obter itens do carrinho
            var itensCarrinho = await ArmazenarCarrinho.Itens(Request, Response, _produtoRepository);
            if (itensCarrinho == null || !itensCarrinho.Any())
            {
                TempData["Erro"] = "Seu carrinho está vazio.";
                return RedirectToAction("Index");
            }

            // Calcular valores
            decimal subtotal = itensCarrinho.Sum(i => i.IpPrecoUnitario * i.IpQuantidade);
            decimal taxaEntrega = 10m; // valor fixo, pode ajustar
            decimal valorTotal = MetodoPagamento == "Pix"
                ? (subtotal + taxaEntrega) * 0.95m // desconto de 5% para Pix
                : subtotal + taxaEntrega;

            // Definir CartaoId apenas se o pagamento for cartão
            int? cartaoIdFinal = MetodoPagamento == "Pix" ? null : CartaoId;

            string? tipoCartaoFinal = MetodoPagamento == "Pix"
    ? null
    : carrinho.NovoCartao.CarTipo;


            // Criar pedido
            var pedido = new Pedido
            {
                UsuarioId = usuarioId,
                EnderecoId = EnderecoSelecionadoId,
                MetodoPagamentoId = MetodoPagamento == "Pix" ? 2 : 1, // 2 = Pix, 1 = Cartão
                CartaoId = cartaoIdFinal,
                PdTaxaEntrega = taxaEntrega,
                PdValorTotal = valorTotal,
                PdStatusPagamento = "Aguardando Pagamento",
                PdCriadoEm = DateTime.Now,
                Itens = itensCarrinho
            };

            try
            {
                // Adicionar pedido e obter ID gerado
                int pedidoId = await _pedidoRepository.AdicionarPedido(pedido);

                // Limpar carrinho após finalizar pedido
                ArmazenarCarrinho.Salvar(Response, new Dictionary<int, int>());
                Console.WriteLine($"Pedido criado com ID: {pedidoId}");

                // Redirecionar para a tela correta de pagamento
                if (MetodoPagamento == "Pix")
                {
                    return RedirectToAction("Pix", "Carrinho", new { pedidoId = pedidoId });
                }
                else
                {
                    HttpContext.Session.SetInt32("Parcelas", ParcelasSelecionadas);

                    ValorParcela = ValorParcela / 100m;


                    HttpContext.Session.SetString("ValorParcela",
                        ValorParcela.ToString(CultureInfo.InvariantCulture));

                    HttpContext.Session.SetString("TipoCartao", carrinho.NovoCartao.CarTipo);


                    return RedirectToAction("ConfirmarCartao", "Carrinho", new { pedidoId = pedidoId });
                }
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao finalizar pedido: " + ex.Message;
                return RedirectToAction("Index");
            }
        }



        [HttpGet]
        public async Task<IActionResult> Pix(int pedidoId)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                return RedirectToAction("Login", "Usuario");
            }
            int usuarioId = Convert.ToInt32(usuarioIdStr);

            var pedido = await _pedidoRepository.ObterPedidoPorId(pedidoId);
            if (pedido == null)
            {
                TempData["Erro"] = "Pedido não encontrado.";
                return RedirectToAction("Index");
            }

            var itensCarrinho = pedido.Itens?.ToList() ?? new List<ItemPedido>();
            decimal subtotal = itensCarrinho.Sum(i => i.IpPrecoUnitario * i.IpQuantidade);
            decimal taxaEntrega = pedido.PdTaxaEntrega;

            // Buscar endereços e cartões do usuário
            var enderecos = _carrinhoRepository.ObterEnderecosPorUsuario(usuarioId) ?? new List<Endereco>();
            var cartoes = _carrinhoRepository.ObterCartoesPorUsuario(usuarioId)?.ToList() ?? new List<Cartao>();

            // Montar o carrinho completo
            var carrinho = new Carrinho
            {
                Itens = itensCarrinho,
                Enderecos = enderecos,
                Cartoes = cartoes,
                ValorTotal = pedido.PdValorTotal,
                TaxaEntrega = taxaEntrega,
                NovoEndereco = new Endereco(),
                NovoCartao = new Cartao(),
                MetodoPagamento = "Pix",
                CriadoEm = pedido.PdCriadoEm,
                Expiracao = pedido.PdCriadoEm.AddMinutes(30)
            };

            // Gerar código Pix fictício
            ViewBag.CodigoPix = $"PIX-{pedido.PdId}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
            ViewBag.PedidoId = pedido.PdId;


            return View(carrinho);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarPagamento(int pedidoId)
        {

            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                return RedirectToAction("Login", "Usuario");
            }

            int usuarioId = Convert.ToInt32(usuarioIdStr);

            try
            {
                var pedido = await _pedidoRepository.ObterPedidoPorId(pedidoId);
                if (pedido == null)
                {
                    TempData["Erro"] = "Pedido não encontrado.";
                    return RedirectToAction("Index");
                }

                // Atualizar status e data do pagamento
                pedido.PdStatusPagamento = "Pago";
                pedido.PdDataPagamento = DateTime.Now;

                var linhas = await _pedidoRepository.AtualizarStatusPagamento(pedido);
                if (linhas <= 0)
                {
                    TempData["Erro"] = "Não foi possível atualizar o status do pagamento.";
                    return RedirectToAction("Pix", new { pedidoId });
                }

                TempData["Sucesso"] = "Pagamento confirmado com sucesso!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao confirmar pagamento: " + ex.Message;
                return RedirectToAction("Pix", new { pedidoId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmarCartao(int pedidoId)
        {

            if (pedidoId <= 0)
            {
                TempData["Erro"] = "Pedido inválido.";
                return RedirectToAction("Index");
            }

            int parcelas = HttpContext.Session.GetInt32("Parcelas") ?? 1;

            string valorStr = HttpContext.Session.GetString("ValorParcela");

            decimal valorParcela = 0m;

            if (!string.IsNullOrEmpty(valorStr))
            {
                valorParcela = decimal.Parse(valorStr, CultureInfo.InvariantCulture);
            }
            // FORMATA PARA EXIBIR EM R$ NA VIEW
            ViewBag.Parcelas = parcelas;
            ViewBag.ValorParcela = valorParcela.ToString("F2", new CultureInfo("pt-BR"));

            string tipoCartao = HttpContext.Session.GetString("TipoCartao") ?? "";

            ViewBag.TipoCartao = tipoCartao;



            Console.WriteLine("SESSION ValorParcela RAW = " + HttpContext.Session.GetString("ValorParcela"));

            // Obter pedido do banco
            var pedido = await _pedidoRepository.ObterPedidoPorId(pedidoId);
            if (pedido == null)
            {
                TempData["Erro"] = "Pedido não encontrado.";
                return RedirectToAction("Index");
            }

            // Converter Pedido em Carrinho
            var carrinho = new Carrinho
            {
                Itens = pedido.Itens,
                Subtotal = pedido.Itens.Sum(i => i.IpPrecoUnitario * i.IpQuantidade),
                TaxaEntrega = pedido.PdTaxaEntrega,
                CartaoSelecionadoId = pedido.CartaoId,
                Cartoes = new List<Cartao>() // opcional, se quiser exibir cartões
            };

            ViewBag.PedidoId = pedido.PdId; // necessário para o POST

            return View(carrinho); // continua usando @model Carrinho
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarPagamentoCartao(int pedidoId)
        {

            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
                return RedirectToAction("Login", "Usuario");

            int usuarioId = Convert.ToInt32(usuarioIdStr);


            var pedido = await _pedidoRepository.ObterPedidoPorId(pedidoId);
            if (pedido == null)
            {
                TempData["Erro"] = "Pedido não encontrado.";
                return RedirectToAction("Index");
            }

            pedido.PdStatusPagamento = "Pago";
            pedido.PdDataPagamento = DateTime.Now;

            var linhas = await _pedidoRepository.AtualizarStatusPagamento(pedido);
            if (linhas <= 0)
            {
                TempData["Erro"] = "Não foi possível atualizar o status do pagamento.";
                return RedirectToAction("ConfirmarCartao", new { pedidoId });
            }

            TempData["Sucesso"] = "Pagamento confirmado com sucesso!";
            return RedirectToAction("Index", "Home");
        }

    }
}
