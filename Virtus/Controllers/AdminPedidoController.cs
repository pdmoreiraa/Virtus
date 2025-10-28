using Microsoft.AspNetCore.Mvc;
using Virtus.Repository;

namespace Virtus.Controllers
{
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class AdminPedidoController : Controller
    {
        private readonly IPedidoRepository _pedidoRepository;

        public AdminPedidoController(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<IActionResult> Index()
        {
            var pedidos = await _pedidoRepository.ObterTodosPedidos();
            return View(pedidos);
        }
    }
}
