using Dapper;
using MySql.Data.MySqlClient;
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

        public async Task AdicionarPedido(Pedido pedido)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Inserir o pedido
                var sqlPedido = @"
            INSERT INTO pedidos 
            (UsuarioId, EnderecoId, MetodoPagamentoId, CartaoId, TaxaEntrega, ValorTotal, StatusPedido, CriadoEm)
            VALUES 
            (@UsuarioId, @EnderecoId, @MetodoPagamentoId, @CartaoId, @TaxaEntrega, @ValorTotal, @StatusPedido, @CriadoEm);
        ";

                await connection.ExecuteAsync(sqlPedido, pedido, transaction);

                // Recuperar o último ID inserido — precisa ser uma query separada!
                var pedidoId = await connection.ExecuteScalarAsync<int>("SELECT LAST_INSERT_ID();", transaction: transaction);
                pedido.Id = pedidoId;

                // Inserir itens do pedido
                var sqlItem = @"
            INSERT INTO itenspedido (PedidoId, ProdutoId, Quantidade, PrecoUnitario)
            VALUES (@PedidoId, @ProdutoId, @Quantidade, @PrecoUnitario);
        ";

                foreach (var item in pedido.Itens)
                {
                    item.PedidoId = pedidoId;
                    await connection.ExecuteAsync(sqlItem, item, transaction);
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Erro ao salvar pedido e itens no banco: " + ex.Message);
            }
        }




        public async Task<Pedido?> ObterPedidoPorId(int pedidoId)
        {
            using var connection = new MySqlConnection(_connectionString);

            var sqlPedido = "SELECT * FROM pedidos WHERE Id = @Id";
            var pedido = await connection.QueryFirstOrDefaultAsync<Pedido>(sqlPedido, new { Id = pedidoId });

            if (pedido != null)
            {
                var sqlItens = @"
                SELECT i.*, p.*
                FROM itensPedido i
                INNER JOIN produtos p ON i.ProdutoId = p.Id
                WHERE i.PedidoId = @PedidoId";

                var itens = await connection.QueryAsync<ItemPedido, Produto, ItemPedido>(
                    sqlItens,
                    (item, produto) => { item.Produto = produto; return item; },
                    new { PedidoId = pedidoId }
                );

                pedido.Itens = itens.ToList();
            }

            return pedido;
        }

        public async Task<List<Pedido>> ObterPedidosPorUsuario(int usuarioId)
        {
            using var connection = new MySqlConnection(_connectionString);

            var sql = "SELECT * FROM pedidos WHERE UsuarioId = @UsuarioId ORDER BY CriadoEm DESC";
            var pedidos = await connection.QueryAsync<Pedido>(sql, new { UsuarioId = usuarioId });

            return pedidos.ToList();
        }

        public async Task<int> AtualizarStatusPagamento(Pedido pedido)
        {
            using var connection = new MySqlConnection(_connectionString);

            var sql = @"
        UPDATE pedidos
        SET StatusPedido = @StatusPedido,
            DataPagamento = @DataPagamento
        WHERE Id = @Id;
    ";

            var linhas = await connection.ExecuteAsync(sql, new
            {
                Id = pedido.Id,
                StatusPedido = pedido.StatusPedido,
                DataPagamento = pedido.DataPagamento
            });

            return linhas;
        }

    }
}
