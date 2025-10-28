using MySql.Data.MySqlClient;
using Virtus.Models;
using Dapper;

namespace Virtus.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly string _connectionString;


        public ProdutoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Produto>> TodosProdutos()
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "SELECT Id, Nome, Marca, Categoria, Tipo, Esporte, Descricao, Preco, ImageUrl, Estoque FROM Produtos";
            return await connection.QueryAsync<Produto>(sql);
        }

        public async Task<IEnumerable<Produto>> ProdutosOrdenados()
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "SELECT Id, Nome, Marca, Categoria, Tipo, Esporte, Descricao, Preco, ImageUrl, Estoque FROM Produtos ORDER BY Id DESC";
            return await connection.QueryAsync<Produto>(sql);
        }

        public async Task AdicionarProduto(Produto produto)
        {
            using var connection = new MySqlConnection(_connectionString);

            var sql = @"INSERT INTO Produtos (Nome, Marca, Categoria, Tipo, Esporte, Descricao, Preco, ImageUrl, Estoque)
                VALUES (@Nome, @Marca, @Categoria, @Tipo, @Esporte, @Descricao, @Preco, @ImageUrl, @Estoque);";

            await connection.ExecuteAsync(sql, produto);
        }

        public async Task<Produto?> ProdutosPorId(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "select Id, Nome, Marca, Categoria, Tipo, Esporte, Descricao, Preco, ImageUrl, Estoque FROM produtos WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Produto>(sql, new { Id = id });
        }

        public async Task AtualizarProduto(Produto produto)
        {
            using var connection = new MySqlConnection(_connectionString);

            var sql = @"
            UPDATE Produtos
            SET Nome = @Nome, Marca = @Marca, Categoria = @Categoria, Tipo = @Tipo, Esporte = @Esporte, Descricao = @Descricao, 
            Preco = @Preco, ImageUrl = @ImageUrl, Estoque = @Estoque
            WHERE Id = @Id;
            ";

            await connection.ExecuteAsync(sql, produto);
        }

        public async Task DeletarProduto(int id)
        {
            using var connection = new MySqlConnection(_connectionString);

            var sql = "DELETE FROM Produtos WHERE Id = @Id;";

            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<Dictionary<string, List<string>>> ObterCategoriasTipos()
        {
            const string sql = @"
                SELECT Categoria, Tipo
                FROM produtos
                WHERE Categoria IS NOT NULL
                  AND Tipo IS NOT NULL;
            ";

            await using var connection = new MySqlConnection(_connectionString);
            // QueryAsync sem tipagem -> retorna IEnumerable<dynamic>
            var rows = await connection.QueryAsync(sql);

            // Transformar explicitamente em strings e agrupar
            var mapped = rows
                .Select(r => new
                {
                    Categoria = (r.Categoria == null) ? string.Empty : (string)r.Categoria,
                    Tipo = (r.Tipo == null) ? string.Empty : (string)r.Tipo
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Categoria) && !string.IsNullOrWhiteSpace(x.Tipo));

            var resultado = mapped
                .GroupBy(x => x.Categoria)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Tipo).Distinct().OrderBy(t => t).ToList()
                );

            return resultado;
        }


    }
}
