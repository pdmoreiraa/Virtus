using Dapper;
using MySql.Data.MySqlClient;
using System.Transactions;
using Virtus.Models;

namespace Virtus.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly string _connectionString;


        public ProdutoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Produto>> TodosProdutos()
        {
            using var cnct = new MySqlConnection(_connectionString);
            await cnct.OpenAsync();

            var sql = @"
SELECT 
    p.PrdId, p.PrdNome, p.PrdMarca, p.PrdCategoria, p.PrdTipo, p.PrdEsporte, p.PrdDescricao, p.PrdPreco, p.PrdCor, p.PrdData,
    pi.PimgId, pi.ProdutoId, pi.PimgUrl, pi.PimgOrdemImagem
FROM tbProduto p
LEFT JOIN tbPrdImagem pi ON pi.ProdutoId = p.PrdId
ORDER BY p.PrdId, pi.PimgOrdemImagem;
";

            var produtoDict = new Dictionary<int, Produto>();

            var produtos = await cnct.QueryAsync<Produto, ProdutoImagem, Produto>(
                sql,
                (p, img) =>
                {
                    if (!produtoDict.TryGetValue(p.PrdId, out var produtoEntry))
                    {
                        produtoEntry = p;
                        produtoEntry.Imagens = new List<ProdutoImagem>();
                        produtoDict.Add(p.PrdId, produtoEntry);
                    }

                    // ProdutoImagem.ImagemId mapped from pi.Id AS ImagemId
                    if (img != null && img.PimgId != 0)
                        produtoEntry.Imagens.Add(img);

                    return produtoEntry;
                },
                splitOn: "PimgId"
            );

            return produtoDict.Values.ToList();
        }

        public async Task<List<Produto>> ProdutosOrdenados()
        {
            using var cnct = new MySqlConnection(_connectionString);
            await cnct.OpenAsync();

            var sql = @"
SELECT 
    p.PrdId, p.PrdNome, p.PrdMarca, p.PrdCategoria, p.PrdTipo, p.PrdEsporte, p.PrdDescricao, p.PrdPreco, p.PrdCor, p.PrdData,
    pi.PimgId, pi.ProdutoId, pi.PimgUrl, pi.PimgOrdemImagem
FROM tbProduto p
LEFT JOIN tbPrdImagem pi ON pi.ProdutoId = p.PrdId
ORDER BY p.PrdId DESC, pi.PimgOrdemImagem;
";

            var produtoDict = new Dictionary<int, Produto>();

            var produtos = await cnct.QueryAsync<Produto, ProdutoImagem, Produto>(
                sql,
                (p, img) =>
                {
                    if (!produtoDict.TryGetValue(p.PrdId, out var produtoEntry))
                    {
                        produtoEntry = p;
                        produtoEntry.Imagens = new List<ProdutoImagem>();
                        produtoDict.Add(p.PrdId, produtoEntry);
                    }

                    // Note: ProdutoImagem.ImagemId mapped from pi.Id AS ImagemId
                    if (img != null && img.PimgId != 0)
                        produtoEntry.Imagens.Add(img);

                    return produtoEntry;
                },
                splitOn: "PimgId"
            );

            return produtoDict.Values.ToList();
        }

        public async Task<int> AdicionarProduto(Produto produto)
        {
            using var cnct = new MySqlConnection(_connectionString);
            await cnct.OpenAsync();

            using var transaction = cnct.BeginTransaction();

            try
            {
                // Inserir produto (sem ImageUrl)
                var sqlProduto = @"
                INSERT INTO tbProduto (PrdNome, PrdMarca, PrdCategoria, PrdTipo, PrdEsporte, PrdPreco, PrdDescricao, PrdCor, PrdData)
                VALUES (@PrdNome, @PrdMarca, @PrdCategoria, @PrdTipo, @PrdEsporte, @PrdPreco, @PrdDescricao, @PrdCor, @PrdData);
                SELECT LAST_INSERT_ID();";

                produto.PrdId = await cnct.ExecuteScalarAsync<int>(sqlProduto, produto, transaction);

                // Inserir imagens (se existirem)
                if (produto.Imagens != null && produto.Imagens.Count > 0)
                {
                    var sqlImagem = @"
                    INSERT INTO tbPrdImagem (ProdutoId, PimgUrl, PimgOrdemImagem)
                    VALUES (@ProdutoId, @PimgUrl, @PimgOrdemImagem);";

                    foreach (var img in produto.Imagens)
                    {
                        img.ProdutoId = produto.PrdId; // garante FK correta
                                                    // Se OrdemImagem não estiver setada, coloca 1 como default
                        if (img.PimgOrdemImagem == 0) img.PimgOrdemImagem = 1;

                        await cnct.ExecuteAsync(sqlImagem, new
                        {
                            ProdutoId = img.ProdutoId,
                            PimgUrl = img.PimgUrl,
                            PimgOrdemImagem = img.PimgOrdemImagem
                        }, transaction);
                    }
                }

                if (produto.Estoques?.Any() == true)
                {
                    var sqlEstoque = @"
                INSERT INTO tbEstoque (ProdutoId, EstTamanho, EstQuantidade)
                VALUES (@ProdutoId, @EstTamanho, @EstQuantidade);
            ";

                    foreach (var est in produto.Estoques)
                    {
                        est.ProdutoId = produto.PrdId;
                        await cnct.ExecuteAsync(sqlEstoque, est, transaction);
                    }
                }


                transaction.Commit();
                return produto.PrdId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }


        public async Task<Produto?> ProdutosPorId(int id)
        {
            using var cnct = new MySqlConnection(_connectionString);

            var sql = @"
        SELECT p.*, pi.PimgId, pi.ProdutoId, pi.PimgUrl, pi.PimgOrdemImagem,
        es.EstId, es.EstTamanho, es.EstQuantidade
        FROM tbProduto p
        LEFT JOIN tbPrdImagem pi ON pi.ProdutoId = p.PrdId
        LEFT JOIN tbEstoque es ON es.ProdutoId = p.PrdId
        WHERE p.PrdId = @PrdId
        ORDER BY pi.PimgOrdemImagem, es.EstTamanho;";

            var produtoDict = new Dictionary<int, Produto>();

            var resultado = await cnct.QueryAsync<Produto, ProdutoImagem, Estoque, Produto>(
                sql,
                (p, img, est) =>
                {
                    if (!produtoDict.TryGetValue(p.PrdId, out var prod))
                    {
                        prod = p;
                        prod.Imagens = new List<ProdutoImagem>();
                        prod.Estoques = new List<Estoque>();
                        produtoDict.Add(p.PrdId, prod);
                    }

                    if (img != null && img.PimgId != 0)
                        prod.Imagens.Add(img);

                    if (est != null && !prod.Estoques.Any(x => x.EstId == est.EstId))
                        prod.Estoques.Add(est);

                    return prod;
                },
                new { PrdId = id },
                splitOn: "PimgId, EstId"
            );

            return produtoDict.Values.FirstOrDefault();
        }


        public async Task AtualizarProduto(Produto produto)
        {
            using var cnct = new MySqlConnection(_connectionString);
            await cnct.OpenAsync();

            using var transaction = await cnct.BeginTransactionAsync();
            try
            {
                // Atualiza os campos do produto
                var sql = @"
                UPDATE tbProduto
                SET PrdNome = @PrdNome, PrdMarca = @PrdMarca, PrdCategoria = @PrdCategoria, PrdTipo = @PrdTipo, PrdEsporte = @PrdEsporte, PrdDescricao = @PrdDescricao, 
                PrdPreco = @PrdPreco, PrdCor = @PrdCor
                WHERE PrdId = @PrdId;
                ";

                await cnct.ExecuteAsync(sql, produto, transaction);

                // Atualiza / insere imagens
                if (produto.Imagens != null && produto.Imagens.Count > 0)
                {
                    foreach (var img in produto.Imagens)
                    {
                        // Se a imagem já existe no banco (tem Id), podemos atualizar a URL ou ordem
                        if (img.PimgId > 0)
                        {
                            var sqlImgUpdate = @"
                    UPDATE tbPrdImagem SET PimgUrl = @PimgUrl WHERE PimgId = @PimgId";
                            await cnct.ExecuteAsync(sqlImgUpdate, img);
                        }
                        else
                        {
                            // Nova imagem
                            var sqlImgInsert = @"
                        INSERT INTO tbPrdImagem (ProdutoId, PimgUrl, PimgOrdemImagem)
                        VALUES (@ProdutoId, @PimgUrl, @PimgOrdemImagem)";
                            img.ProdutoId = produto.PrdId;
                            await cnct.ExecuteAsync(sqlImgInsert, img);
                        }
                    }
                }

                var sqlSelect = @"SELECT EstId FROM tbEstoque WHERE ProdutoId = @PrdId;";
                var estoquesDb = (await cnct.QueryAsync<int>(sqlSelect, new { produto.PrdId }, transaction)).ToList();

                foreach (var estoque in produto.Estoques)
                {
                    estoque.ProdutoId = produto.PrdId;

                    if (estoque.EstId == 0)
                    {
                        // Novo estoque
                        var sqlInsert = @"
                        INSERT INTO tbEstoque (ProdutoId, EstTamanho, EstQuantidade)
                        VALUES (@ProdutoId, @EstTamanho, @EstQuantidade);
                        ";

                        await cnct.ExecuteAsync(sqlInsert, estoque, transaction);
                    }
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Estoque?> EstPorId(int produtoId)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "SELECT * FROM tbEstoque WHERE ProdutoId = @ProdutoId";
            return await connection.QueryFirstOrDefaultAsync<Estoque>(sql, new { ProdutoId = produtoId });
        }


        public async Task DeletarProduto(int id)
        {
            using var cnct = new MySqlConnection(_connectionString);

            var sql = "DELETE FROM tbProduto WHERE PrdId = @PrdId;";

            await cnct.ExecuteAsync(sql, new { PrdId = id });
        }

        public async Task<Dictionary<string, List<string>>> ObterCategoriasTipos()
        {
            const string sql = @"
                SELECT PrdCategoria, PrdTipo
                FROM tbProduto
                WHERE PrdCategoria IS NOT NULL
                  AND PrdTipo IS NOT NULL;
            ";

            await using var connection = new MySqlConnection(_connectionString);
            // QueryAsync sem tipagem -> retorna IEnumerable<dynamic>
            var rows = await connection.QueryAsync(sql);

            // Transformar explicitamente em strings e agrupar
            var mapped = rows
                .Select(r => new
                {
                    PrdCategoria = (r.PrdCategoria == null) ? string.Empty : (string)r.Categoria,
                    PrdTipo = (r.PrdTipo == null) ? string.Empty : (string)r.PrdTipo
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.PrdCategoria) && !string.IsNullOrWhiteSpace(x.PrdTipo));

            var resultado = mapped
                .GroupBy(x => x.PrdCategoria)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.PrdTipo).Distinct().OrderBy(t => t).ToList()
                );

            return resultado;
        }

        public async Task<ProdutoImagem?> ImagemPorId(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "SELECT * FROM tbPrdImagem WHERE PimgId = @PimgId";
            return await connection.QueryFirstOrDefaultAsync<ProdutoImagem>(sql, new { PimgId = id });
        }

        public async Task DeletarImagem(int imagemId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                string sql = "DELETE FROM tbPrdImagem WHERE PimgId = @PimgId";
                await connection.ExecuteAsync(sql, new { PimgId = imagemId });
            }
        }


    }
}
