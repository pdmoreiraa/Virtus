using Virtus.Models;

namespace Virtus.Repository
{
    public interface IPedidoRepository
    {
        Task<int> AdicionarPedido(Pedido pedido);
        Task<Pedido?> ObterPedidoPorId(int pedidoId);
        Task<List<Pedido>> ObterPedidosPorUsuario(int usuarioId);
        Task<int> AtualizarStatusPagamento(Pedido pedido);
    }

}
