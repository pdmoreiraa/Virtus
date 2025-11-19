namespace Virtus.Models
{
    public class Endereco
    {
        public int EndId { get; set; }
        public int UsuarioId { get; set; }
        public string EndNomeCompleto { get; set; } = string.Empty;
        public string EndLogradouro { get; set; } = string.Empty;
        public string EndNumero { get; set; } = string.Empty;
        public string EndBairro { get; set; } = string.Empty;
        public string EndCidade { get; set; } = string.Empty;
        public string EndEstado { get; set; } = string.Empty;
        public string EndCEP { get; set; } = string.Empty;
        public string? EndComplemento { get; set; }

        // Navegação
        public Usuario? Usuario { get; set; }
    }
}
