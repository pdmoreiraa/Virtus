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
            using var cnct = new MySqlConnection(_connectionString);
            await cnct.OpenAsync();

            // Inserir o pedido
            var sqlInsert = @"
            INSERT INTO tbPedido (UsuarioId, EnderecoId, MetodoPagamentoId, CartaoId, PdTaxaEntrega, 
            PdStatusPagamento, PdStatusPedido, PdCriadoEm, PdValorTotal)
            VALUES (@UsuarioId, @EnderecoId, @MetodoPagamentoId, @CartaoId, @PdTaxaEntrega, @PdStatusPagamento, 
            @PdStatusPedido, @PdCriadoEm, @PdValorTotal);";


            await cnct.ExecuteAsync(sqlInsert, pedido);

            // Obter o ID gerado
            int pedidoId = await cnct.ExecuteScalarAsync<int>("SELECT LAST_INSERT_ID();");

            pedido.PdId = pedidoId;

            // Inserir itens
            const string sqlItem = @"
            INSERT INTO tbItemPedido (PedidoId, ProdutoId, IpQuantidade, IpPrecoUnitario)
            VALUES (@PedidoId, @ProdutoId, @IpQuantidade, @IpPrecoUnitario);";

            foreach (var item in pedido.Itens)
            {
                await cnct.ExecuteAsync(sqlItem, new
                {
                    PedidoId = pedidoId,
                    item.ProdutoId,
                    item.IpQuantidade,
                    item.IpPrecoUnitario
                });
            }

            return pedidoId;
        }


        public async Task<Pedido?> ObterPedidoPorId(int pedidoId)
        {
            using var cnct = new MySqlConnection(_connectionString);

            // Buscar pedido pelo Id
            var sqlPedido = "SELECT * FROM tbPedido WHERE PdId = @PdId";
            var pedido = await cnct.QueryFirstOrDefaultAsync<Pedido>(
                sqlPedido,
                new { PdId = pedidoId }  // o nome do parâmetro deve coincidir com o @Id
            );

            if (pedido != null)
            {
                // Buscar itens do pedido junto com os produtos
                var sqlItens = @"
            SELECT i.*, p.*
            FROM tbItemPedido i
            INNER JOIN tbProduto p ON i.ProdutoId = p.PrdId
            WHERE i.PedidoId = @PedidoId";

                var itens = await cnct.QueryAsync<ItemPedido, Produto, ItemPedido>(
                    sqlItens,
                    (item, produto) => { item.Produto = produto; return item; },
                    new { PedidoId = pedidoId },
                    splitOn: "PrdId"
                );

                pedido.Itens = itens.ToList();
            }

            return pedido;
        }

        public async Task<int> AtualizarStatusPagamento(Pedido pedido)
        {
            using var cnct = new MySqlConnection(_connectionString);

            string sql = @"
        UPDATE tbPedido 
        SET PdStatusPagamento = @PdStatusPagamento, PdDataPagamento = @PdDataPagamento 
        WHERE PdId = @PdId";

            return await cnct.ExecuteAsync(sql, new { pedido.PdStatusPagamento, pedido.PdDataPagamento, pedido.PdId });
        }




        public async Task<List<Pedido>> ObterPedidosPorUsuario(int usuarioId)
        {
            using var cnct = new MySqlConnection(_connectionString);

            var sql = "SELECT * FROM tbPedido WHERE UsuarioId = @UsuarioId ORDER BY PdCriadoEm DESC";
            var pedidos = await cnct.QueryAsync<Pedido>(sql, new { UsuarioId = usuarioId });

            return pedidos.ToList();
        }


        public async Task<List<Pedido>> ObterTodosPedidos()
        {
            using var cnct = new MySqlConnection(_connectionString);
            await cnct.OpenAsync();

            var sqlPedidos = @"
            SELECT 
            p.PdId, p.UsuarioId, p.EnderecoId, p.MetodoPagamentoId, p.CartaoId, 
            p.PdTaxaEntrega, p.PdStatusPagamento, p.PdStatusPedido, p.PdCriadoEm, p.PdDataPagamento, p.PdValorTotal,
            u.UsuId, u.UsuNome, u.UsuSobrenome, u.UsuEmail, u.UsuCPF, u.UsuTelefone, u.UsuTipo,
            e.EndId, e.EndNomeCompleto, e.EndLogradouro, e.EndNumero, e.EndBairro, e.EndCidade, e.EndEstado, e.EndCEP, e.EndComplemento,
            m.MpId, m.MpDescricao,
            c.CarId, c.CarNomeTitular, c.CarNumero, c.CarTipo
            FROM tbPedido p
            LEFT JOIN tbUsuario u ON p.UsuarioId = u.UsuId
            LEFT JOIN tbEndereco e ON p.EnderecoId = e.EndId
            LEFT JOIN tbMetPagamento m ON p.MetodoPagamentoId = m.MpId
            LEFT JOIN tbCartao c ON p.CartaoId = c.CarId
            ORDER BY p.PdCriadoEm DESC;";

            var pedidos = (await cnct.QueryAsync<Pedido, Usuario, Endereco, MetodoPagamento, Cartao, Pedido>(
                sqlPedidos,
                (pedido, usuario, endereco, metodoPagamento, cartao) =>
                {
                    pedido.Usuario = usuario;
                    pedido.Endereco = endereco;
                    pedido.MetodoPagamento = metodoPagamento;
                    pedido.Cartao = cartao;
                    return pedido;
                },
                splitOn: "UsuId,EndId,MpId,CarId"
            )).ToList();

            if (!pedidos.Any()) return pedidos;

            // Obter itens e produtos
            var sqlItens = @"
            SELECT 
            ip.IpId AS ItemPedidoId, ip.PedidoId, ip.ProdutoId, ip.IpQuantidade, ip.IpPrecoUnitario, 
            p.PrdId, p.PrdNome, p.PrdMarca, p.PrdCategoria, p.PrdTipo, p.PrdEsporte, p.PrdPreco,
            (
            SELECT pi.PimgUrl
            FROM tbPrdImagem pi
            WHERE pi.ProdutoId = p.PrdId
            ORDER BY pi.PimgOrdemImagem ASC
            LIMIT 1
            ) AS ImagemUrl
            FROM tbItemPedido ip
            INNER JOIN tbProduto p ON ip.ProdutoId = p.PrdId
            WHERE ip.PedidoId IN @PedidoIds;";

            var itens = await cnct.QueryAsync<ItemPedido, Produto, ItemPedido>(
                sqlItens,
                (item, produto) =>
                {
                    item.Produto = produto;
                    return item;
                },
                new { PedidoIds = pedidos.Select(p => p.PdId).ToArray() },
                splitOn: "ProdutoId"
            );

            // 3️⃣ Atribuir itens a cada pedido
            foreach (var pedido in pedidos)
            {
                pedido.Itens = itens.Where(i => i.PedidoId == pedido.PdId).ToList();
            }

            return pedidos;
        }




        public async Task<Pedido?> ObterPedidoPorIdAdm(int pedidoId)
        {
            using var cnct = new MySqlConnection(_connectionString);

            var sql = @"
            SELECT 
            p.PdId, p.UsuarioId, p.EnderecoId, p.MetodoPagamentoId, p.CartaoId,
            p.PdValorTotal, p.PdTaxaEntrega, p.PdStatusPagamento, p.PdStatusPedido, p.PdCriadoEm, p.PdDataPagamento,
            u.UsuId, u.UsuNome, u.UsuSobrenome, u.UsuEmail, u.UsuTelefone,
            e.EndId, e.EndNomeCompleto, e.EndLogradouro, e.EndNumero, e.EndBairro, e.EndCidade, e.EndEstado, e.EndCEP, e.EndComplemento,
            m.MpId, m.MpDescricao,
            c.CarId, c.CarNomeTitular, c.CarNumero, c.CarValidade, c.CarTipo
            FROM tbPedido p
            LEFT JOIN tbUsuario u ON p.UsuarioId = u.UsuId
            LEFT JOIN tbEndereco e ON p.EnderecoId = e.EndId
            LEFT JOIN tbMetPagamento m ON p.MetodoPagamentoId = m.MpId
            LEFT JOIN tbCartao c ON p.CartaoId = c.CarId
            WHERE p.PdId = @PedidoId;";

            var pedido = await cnct.QueryAsync<Pedido, Usuario, Endereco, MetodoPagamento, Cartao, Pedido>(
                sql,
                (p, u, e, m, c) =>
                {
                    p.Usuario = u ?? new Usuario { UsuNome = "Desconhecido", UsuSobrenome = "" };
                    p.Endereco = e ?? new Endereco { EndNomeCompleto = "Desconhecido" };
                    p.MetodoPagamento = m ?? new MetodoPagamento { MpDescricao = "Desconhecido" };
                    p.Cartao = c; // pode ser null
                    return p;
                },
                new { PedidoId = pedidoId },
                splitOn: "UsuId,EndId,MpId,CarId"
            );

            var pedidoResult = pedido.FirstOrDefault();

            if (pedidoResult != null)
            {
                // Carregar itens e produtos
                var sqlItens = @"
                SELECT 
                i.*, pr.*, pi.PimgId, pi.PimgUrl, pi.PimgOrdemImagem
                FROM tbItemPedido i
                INNER JOIN tbProduto pr ON i.ProdutoId = pr.PrdId
                LEFT JOIN tbPrdImagem pi ON pr.PrdId = pi.ProdutoId
                WHERE i.PedidoId = @PedidoId
                ORDER BY pi.PimgOrdemImagem ASC;";

                var itemDict = new Dictionary<int, ItemPedido>();

                var itens = await cnct.QueryAsync<ItemPedido, Produto, ProdutoImagem, ItemPedido>(
                    sqlItens,
                    (i, pr, pi) =>
                    {
                        if (!itemDict.TryGetValue(i.IpId, out var item))
                        {
                            item = i;
                            item.Produto = pr;
                            item.Produto.Imagens = new List<ProdutoImagem>();
                            itemDict.Add(item.IpId, item);
                        }

                        if (pi != null)
                            item.Produto.Imagens.Add(pi);

                        return item;
                    },
                    new { PedidoId = pedidoId },
                    splitOn: "PrdId,PimgId"
                );

                pedidoResult.Itens = itemDict.Values.ToList();

                pedidoResult.Itens = itens.ToList();

                // Garante valor total consistente
                if (pedidoResult.PdValorTotal == 0 && pedidoResult.Itens.Any())
                {
                    pedidoResult.PdValorTotal = pedidoResult.Itens.Sum(x => x.IpQuantidade * x.IpPrecoUnitario) + pedidoResult.PdTaxaEntrega;
                }
            }

            return pedidoResult;
        }

        public async Task AtualizarStatus(int pedidoId, string statusPedido)
        {
            using var cnct = new MySqlConnection(_connectionString);

            var sql = "UPDATE tbPedido SET PdStatusPedido = @PdStatusPedido WHERE PdId = @PdId";
            var parameters = new { PdId = pedidoId, StatusPedido = statusPedido };

            await cnct.ExecuteAsync(sql, parameters);
        }

        public async Task<List<Pedido>> ObterPedidosDoUsuario(int usuarioId)
        {
            using var cnct = new MySqlConnection(_connectionString);

            var sqlPedidos = @"
            SELECT 
            p.PdId, p.UsuarioId, p.PdCriadoEm, p.PdValorTotal, p.PdStatusPedido, p.PdDataPagamento,
            p.MetodoPagamentoId, m.MpId, m.MpDescricao
            FROM tbPedido p
            LEFT JOIN tbMetPagamento m ON p.MetodoPagamentoId = m.MpId
            WHERE p.UsuarioId = @UsuarioId
            ORDER BY p.PdCriadoEm DESC";

            var pedidos = await cnct.QueryAsync<Pedido, MetodoPagamento, Pedido>(
                sqlPedidos,
                (pedido, metodoPagamento) =>
                {
                    pedido.MetodoPagamento = metodoPagamento ?? new MetodoPagamento { MpDescricao = "Desconhecido" };
                    return pedido;
                },
                new { UsuarioId = usuarioId },
                splitOn: "MpId"
            );

            var listaPedidos = pedidos.ToList();

            if (listaPedidos.Count > 0)
            {
                var pedidoIds = listaPedidos.Select(p => p.PdId).ToArray(); // <--- usa Id
                var sqlItens = @"
                SELECT PedidoId, IpQuantidade
                FROM tbItemPedido
                WHERE PedidoId IN @PedidoIds";

                var itens = await cnct.QueryAsync<ItemPedido>(sqlItens, new { PedidoIds = pedidoIds });

                foreach (var pedido in listaPedidos)
                {
                    pedido.Itens = itens.Where(i => i.PedidoId == pedido.PdId).ToList();
                }
            }

            return listaPedidos;
        }


    }
}
