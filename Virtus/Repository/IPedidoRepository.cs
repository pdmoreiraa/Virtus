using Virtus.Models;

namespace Virtus.Repository
{
    public interface IPedidoRepository
    {
        Task CriarPedidoAsync(Pedido pedido);
        Task<Pedido?> ObterPedidoPorIdAsync(int pedidoId);
        Task<List<Pedido>> ObterPedidosPorUsuarioAsync(int usuarioId);
    }

}
