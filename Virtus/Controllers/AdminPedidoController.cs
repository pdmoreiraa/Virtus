using Microsoft.AspNetCore.Mvc;
using Virtus.Models;
using Virtus.Repository;

namespace Virtus.Controllers
{
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class AdminPedidoController : Controller
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly int pagTam = 8;

        public AdminPedidoController(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<IActionResult> Index(int pagIndex)
        {
            var pedidos = await _pedidoRepository.ObterTodosPedidos();

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
    }
}
