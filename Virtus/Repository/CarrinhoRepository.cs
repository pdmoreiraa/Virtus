using Dapper;
using MySql.Data.MySqlClient;
using Virtus.Models;

namespace Virtus.Repository
{
    public class CarrinhoRepository
    {
        private readonly string _connectionString;


        public CarrinhoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Endereco> ObterEnderecosPorUsuario(int usuarioId)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "SELECT * FROM tbEndereco WHERE UsuarioId = @UsuarioId";
            return connection.Query<Endereco>(sql, new { UsuarioId = usuarioId }).ToList();
        }

        public void AdicionarEndereco(Endereco endereco)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"INSERT INTO tbEndereco 
                (UsuarioId, EndNomeCompleto, EndLogradouro, EndNumero, EndBairro, EndCidade, EndEstado, EndCEP, EndComplemento)
                VALUES (@UsuarioId, @EndNomeCompleto, @EndLogradouro, @EndNumero, @EndBairro, @EndCidade, @EndEstado, @EndCEP, @EndComplemento)";
            connection.Execute(sql, endereco);
        }
        public IEnumerable<Cartao> ObterCartoesPorUsuario(int usuarioId)
        {
            using var connection = new MySqlConnection(_connectionString);
            string sql = "SELECT * FROM tbCartao WHERE UsuarioId = @UsuarioId";
            return connection.Query<Cartao>(sql, new { UsuarioId = usuarioId });
        }

        public async Task<Cartao> ObterCartaoPorId(int cartaoId)
        {
            using var connection = new MySqlConnection(_connectionString);
            string sql = "SELECT * FROM tbCartao WHERE CarId = @CarId";
            return await connection.QueryFirstOrDefaultAsync<Cartao>(sql, new { CarId = cartaoId });
        }


        public void AdicionarCartao(Cartao cartao)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"INSERT INTO tbCartao 
                (UsuarioId, CarTipo, CarNomeTitular, CarNumero, CarValidade, CarCVV, CarBandeira)
                VALUES (@UsuarioId, @CarTipo, @CarNomeTitular, @CarNumero, @CarValidade, @CarCVV, @CarBandeira)";
            connection.Execute(sql, cartao);
        }

    }
}
