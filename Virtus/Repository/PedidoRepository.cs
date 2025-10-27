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
                // 1️⃣ Inserir o pedido
                var sqlPedido = @"
            INSERT INTO pedidos 
                (UsuarioId, EnderecoId, MetodoPagamentoId, CartaoId, TaxaEntrega, StatusPedido, CriadoEm)
            VALUES 
                (@UsuarioId, @EnderecoId, @MetodoPagamentoId, @CartaoId, @TaxaEntrega, @StatusPedido, @CriadoEm);
            SELECT LAST_INSERT_ID();";

                pedido.Id = await connection.ExecuteScalarAsync<int>(sqlPedido, pedido, transaction);

                // 2️⃣ Inserir os itens do pedido (se houver)
                if (pedido.Itens != null && pedido.Itens.Any())
                {
                    var sqlItens = @"
                INSERT INTO itenspedido (PedidoId, ProdutoId, Quantidade, PrecoUnitario)
                VALUES (@PedidoId, @ProdutoId, @Quantidade, @PrecoUnitario);";

                    foreach (var item in pedido.Itens)
                    {
                        // Validação extra para garantir que não quebre o insert
                        if (item.ProdutoId <= 0 && item.Produto != null)
                            item.ProdutoId = item.Produto.Id;

                        if (item.ProdutoId <= 0)
                            throw new Exception("ProdutoId inválido ao tentar inserir um item do pedido.");

                        await connection.ExecuteAsync(sqlItens, new
                        {
                            PedidoId = pedido.Id,
                            ProdutoId = item.ProdutoId,
                            Quantidade = item.Quantidade,
                            PrecoUnitario = item.PrecoUnitario
                        }, transaction);
                    }
                }
                else
                {
                    throw new Exception("Nenhum item encontrado no pedido.");
                }

                // 3️⃣ Commit final — tudo deu certo
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // 4️⃣ Rollback em caso de falha
                await transaction.RollbackAsync();
                Console.WriteLine($"Erro ao adicionar pedido: {ex.Message}");
                throw new Exception("Erro ao salvar pedido e itens no banco: " + ex.Message, ex);
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
    }
}
