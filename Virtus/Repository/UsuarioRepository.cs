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
                "SELECT * FROM tbUsuario WHERE UsuEmail = @UsuEmail OR UsuCPF = @UsuCPF",
                new { usuario.UsuEmail, usuario.UsuCPF });

            if (existente != null)
                return null;

            var sql = @"
            INSERT INTO tbUsuario 
            (UsuNome, UsuSobrenome, UsuEmail, UsuSenha, UsuCPF, UsuTelefone, UsuTipo)
            VALUES (@UsuNome, @UsuSobrenome, @UsuEmail, @UsuSenha, @UsuCPF, @UsuTelefone, @UsuTipo);
            SELECT LAST_INSERT_ID();";

            var idGerado = await connection.ExecuteScalarAsync<int>(sql, usuario);

            if (idGerado > 0)
            {
                usuario.UsuId = idGerado;
                usuario.UsuConfirmarSenha = string.Empty;
                return usuario;
            }

            return null;
        }

        public async Task<Usuario?> ObterPorEmailESenha(string email, string senha)
        {
            using var connection = new MySqlConnection(_connectionString);
            string sql = "SELECT * FROM tbUsuario WHERE UsuEmail = @UsuEmail AND UsuSenha = @UsuSenha";
            return await connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { UsuEmail = email, UsuSenha = senha });
        }


        public async Task<Usuario?> ObterPorId(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            string sql = "SELECT * FROM tbUsuario WHERE UsuId = @UsuId";
            return await connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { UsuId = id });
        }

        public async Task<bool> AtualizarPerfil(Usuario usuario)
        {
            using var connection = new MySqlConnection(_connectionString);

            var sql = @"
            UPDATE tbUsuario
            SET UsuNome = @UsuNome, UsuSobrenome = @UsuSobrenome, UsuEmail = @UsuEmail, UsuTelefone = @UsuTelefone, 
            UsuCPF = @UsuCPF, UsuTipo = @UsuTipo WHERE UsuId = @UsuId;";

            int linhasAfetadas = await connection.ExecuteAsync(sql, usuario);
            return linhasAfetadas > 0;
        }

        public async Task<bool> AtualizarSenha(Usuario usuario)
        {
            using var connection = new MySqlConnection(_connectionString);
            string sql = "UPDATE tbUsuario SET UsuSenha = @UsuSenha WHERE UsuId = @UsuId";
            var linhas = await connection.ExecuteAsync(sql, new { usuario.UsuSenha, usuario.UsuId });
            return linhas > 0;
        }

    }


}
