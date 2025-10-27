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

        public async Task<int> AdicionarPedido(Pedido pedido)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            // 1️⃣ Inserir o pedido
            var sqlInsert = @"
        INSERT INTO pedidos (UsuarioId, EnderecoId, MetodoPagamentoId, CartaoId, TaxaEntrega, StatusPedido, CriadoEm, ValorTotal)
        VALUES (@UsuarioId, @EnderecoId, @MetodoPagamentoId, @CartaoId, @TaxaEntrega, @StatusPedido, @CriadoEm, @ValorTotal);
    ";


            await connection.ExecuteAsync(sqlInsert, pedido);

            // 2️⃣ Obter o ID gerado
            int pedidoId = await connection.ExecuteScalarAsync<int>("SELECT LAST_INSERT_ID();");

            pedido.Id = pedidoId;

            // 3️⃣ Inserir itens
            const string sqlItem = @"
        INSERT INTO itensPedido (PedidoId, ProdutoId, Quantidade, PrecoUnitario)
        VALUES (@PedidoId, @ProdutoId, @Quantidade, @PrecoUnitario);
    ";
            Console.WriteLine($"🟢 ID gerado (pedidoId): {pedidoId}");

            foreach (var item in pedido.Itens)
            {
                await connection.ExecuteAsync(sqlItem, new
                {
                    PedidoId = pedidoId,
                    item.ProdutoId,
                    item.Quantidade,
                    item.PrecoUnitario
                });
            }

            return pedidoId;
        }


        public async Task<Pedido?> ObterPedidoPorId(int pedidoId)
        {
            using var connection = new MySqlConnection(_connectionString);

            // 🔹 Buscar pedido pelo Id
            var sqlPedido = "SELECT * FROM pedidos WHERE Id = @Id";
            var pedido = await connection.QueryFirstOrDefaultAsync<Pedido>(
                sqlPedido,
                new { Id = pedidoId }  // ✅ o nome do parâmetro deve coincidir com o @Id
            );

            if (pedido != null)
            {
                // 🔹 Buscar itens do pedido junto com os produtos
                var sqlItens = @"
            SELECT i.*, p.*
            FROM itensPedido i
            INNER JOIN produtos p ON i.ProdutoId = p.Id
            WHERE i.PedidoId = @PedidoId
        ";

                var itens = await connection.QueryAsync<ItemPedido, Produto, ItemPedido>(
                    sqlItens,
                    (item, produto) => { item.Produto = produto; return item; },
                    new { PedidoId = pedidoId }  // ✅ aqui o nome deve coincidir com @PedidoId
                );

                pedido.Itens = itens.ToList();
            }

            return pedido;
        }

        public async Task<int> AtualizarStatusPagamento(Pedido pedido)
        {
            using var connection = new MySqlConnection(_connectionString);

            string sql = @"
        UPDATE pedidos 
        SET StatusPedido = @StatusPedido, DataPagamento = @DataPagamento 
        WHERE Id = @Id
    ";

            return await connection.ExecuteAsync(sql, new { pedido.StatusPedido, pedido.DataPagamento, pedido.Id });
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
