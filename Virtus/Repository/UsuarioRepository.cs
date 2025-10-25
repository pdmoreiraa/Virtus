using Dapper;
using MySql.Data.MySqlClient;
using Virtus.Models;

namespace Virtus.Repository
{
    public class UsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Usuario?> RegistrarUsuario(Usuario usuario)
        {
            using var connection = new MySqlConnection(_connectionString);

            // Verifica se já existe usuário com mesmo e-mail ou CPF
            var existente = await connection.QueryFirstOrDefaultAsync<Usuario>(
                "SELECT * FROM usuarios WHERE Email = @Email OR CPF = @CPF",
                new { usuario.Email, usuario.CPF });

            if (existente != null)
                return null;

            var sql = @"
            INSERT INTO usuarios 
            (Nome, Sobrenome, Email, Senha, CPF, Telefone, Tipo)
            VALUES (@Nome, @Sobrenome, @Email, @Senha, @CPF, @Telefone, @Tipo);
            SELECT LAST_INSERT_ID();";

            var idGerado = await connection.ExecuteScalarAsync<int>(sql, usuario);

            if (idGerado > 0)
            {
                usuario.Id = idGerado;
                usuario.ConfirmarSenha = string.Empty;
                return usuario;
            }

            return null;
        }

        public async Task<Usuario?> ObterPorEmailESenha(string email, string senha)
        {
            using var connection = new MySqlConnection(_connectionString);
            string sql = "SELECT * FROM Usuarios WHERE Email = @Email AND Senha = @Senha";
            return await connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { Email = email, Senha = senha });
        }

    }
}
