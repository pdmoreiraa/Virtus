using Microsoft.AspNetCore.Mvc;
using Virtus.Models;
using Virtus.Repository;

namespace Virtus.Controllers
{
    public class UsuarioPedidoController : Controller
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly int pagTam = 5;

        public UsuarioPedidoController(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<IActionResult> Index(int pagIndex)
        {
            try
            {
                // Pega o Id do usuário da Session
                var usuarioIdString = HttpContext.Session.GetString("UsuarioId");
                if (string.IsNullOrEmpty(usuarioIdString) || !int.TryParse(usuarioIdString, out int usuarioId))
                {
                    return RedirectToAction("Index", "Home");
                }

                // Busca pedidos do usuário via repository
                var pedidos = await _pedidoRepository.ObterPedidosDoUsuario(usuarioId);

                if (pagIndex < 1) pagIndex = 1;

                // Total de itens e páginas
                int count = pedidos.Count();
                int totalPag = (int)Math.Ceiling((decimal)count / pagTam);

                // Garantir que a página atual não ultrapasse o total de páginas (ou seja 1 se não houver registros)
                if (totalPag == 0) totalPag = 1;
                if (pagIndex > totalPag) pagIndex = totalPag;

                // Pegar produtos da página atual
                var pedidosPaginados = pedidos
                    .Skip((pagIndex - 1) * pagTam)
                    .Take(pagTam)
                    .ToList();


                // Dados para a view
                ViewData["PagIndex"] = pagIndex;
                ViewData["TotalPag"] = totalPag;
                return View(pedidosPaginados);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erro ao obter pedidos: {ex.Message}");
                return View(new List<Pedido>());
            }
        }
    }
}
