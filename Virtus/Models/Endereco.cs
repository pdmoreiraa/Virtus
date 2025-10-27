﻿namespace Virtus.Models
{
    public class Endereco
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string CEP { get; set; } = string.Empty;
        public string? Complemento { get; set; }

        // Navegação
        public Usuario? Usuario { get; set; }
    }
}
