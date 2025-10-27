using Virtus.Models;

namespace Virtus.Repository
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly string _connectionString;

        public PedidoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task CriarPedidoAsync(Pedido pedido)
        {
            throw new NotImplementedException();
        }

        public Task<Pedido?> ObterPedidoPorIdAsync(int pedidoId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Pedido>> ObterPedidosPorUsuarioAsync(int usuarioId)
        {
            throw new NotImplementedException();
        }
    }
}
