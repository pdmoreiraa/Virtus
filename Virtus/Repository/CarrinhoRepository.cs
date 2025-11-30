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
            using var cnct = new MySqlConnection(_connectionString);
            var sql = "SELECT * FROM tbEndereco WHERE UsuarioId = @UsuarioId";
            return cnct.Query<Endereco>(sql, new { UsuarioId = usuarioId }).ToList();
        }

        public void AdicionarEndereco(Endereco endereco)
        {
            using var cnct = new MySqlConnection(_connectionString);
            var sql = @"INSERT INTO tbEndereco 
                (UsuarioId, EndNomeCompleto, EndLogradouro, EndNumero, EndBairro, EndCidade, EndEstado, EndCEP, EndComplemento)
                VALUES (@UsuarioId, @EndNomeCompleto, @EndLogradouro, @EndNumero, @EndBairro, @EndCidade, @EndEstado, @EndCEP, @EndComplemento)";
            cnct.Execute(sql, endereco);
        }
        public IEnumerable<Cartao> ObterCartoesPorUsuario(int usuarioId)
        {
            using var cnct = new MySqlConnection(_connectionString);
            string sql = "SELECT * FROM tbCartao WHERE UsuarioId = @UsuarioId";
            return cnct.Query<Cartao>(sql, new { UsuarioId = usuarioId });
        }

        public async Task<Cartao> ObterCartaoPorId(int cartaoId)
        {
            using var cnct = new MySqlConnection(_connectionString);
            string sql = "SELECT * FROM tbCartao WHERE CarId = @CarId";
            return await cnct.QueryFirstOrDefaultAsync<Cartao>(sql, new { CarId = cartaoId });
        }


        public void AdicionarCartao(Cartao cartao)
        {
            using var cnct = new MySqlConnection(_connectionString);
            var sql = @"INSERT INTO tbCartao 
                (UsuarioId, CarTipo, CarNomeTitular, CarNumero, CarValidade, CarCVV)
                VALUES (@UsuarioId, @CarTipo, @CarNomeTitular, @CarNumero, @CarValidade, @CarCVV)";
            cnct.Execute(sql, cartao);
        }

    }
}
