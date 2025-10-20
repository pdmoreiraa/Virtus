using MySql.Data.MySqlClient;
using Virtus.Models;
using Dapper;

namespace Virtus.Repository
{
    public class ProdutoRepository
    {
        private readonly string _connectionString;

        public ProdutoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Produto>> TodosProdutos()
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "SELECT Id, Nome, Marca, Categoria, Tipo, Descricao, Preco, ImageUrl, Estoque FROM Produtos";
            return await connection.QueryAsync<Produto>(sql);
        }

        public async Task<IEnumerable<Produto>> ProdutosOrdenados()
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "SELECT Id, Nome, Marca, Categoria, Tipo, Descricao, Preco, ImageUrl, Estoque FROM Produtos ORDER BY Id DESC";
            return await connection.QueryAsync<Produto>(sql);
        }

        public async Task AdicionarProduto(Produto produto)
        {
            using var connection = new MySqlConnection(_connectionString);

            var sql = @"INSERT INTO Produtos (Nome, Marca, Categoria, Tipo, Descricao, Preco, ImageUrl, Estoque)
                VALUES (@Nome, @Marca, @Categoria, @Tipo, @Descricao, @Preco, @ImageUrl, @Estoque);";

            await connection.ExecuteAsync(sql, produto);
        }

        public async Task<Produto?> ProdutosPorId(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "select Id, Nome, Marca, Categoria, Tipo, Descricao, Preco, ImageUrl, Estoque FROM produtos WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Produto>(sql, new { Id = id });
        }

        public async Task AtualizarProduto(Produto produto)
        {
            using var connection = new MySqlConnection(_connectionString);

            var sql = @"
            UPDATE Produtos
            SET Nome = @Nome, Marca = @Marca, Categoria = @Categoria, Tipo = @Tipo, Descricao = @Descricao, 
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

    }
}
