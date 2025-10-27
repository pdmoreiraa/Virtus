using Dapper;
using MySql.Data.MySqlClient;
using Virtus.Models;

namespace Virtus.Repository
{
    public class CarrinhoRepository
    {
        private readonly string _connectionString;

        public CarrinhoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Endereco> ObterEnderecosPorUsuario(int usuarioId)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "SELECT * FROM enderecos WHERE UsuarioId = @UsuarioId";
            return connection.Query<Endereco>(sql, new { UsuarioId = usuarioId }).ToList();
        }

        public void AdicionarEndereco(Endereco endereco)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"INSERT INTO enderecos 
                (UsuarioId, NomeCompleto, Logradouro, Numero, Bairro, Cidade, Estado, CEP, Complemento)
                VALUES (@UsuarioId, @NomeCompleto, @Logradouro, @Numero, @Bairro, @Cidade, @Estado, @CEP, @Complemento)";
            connection.Execute(sql, endereco);
        }
    }
}
