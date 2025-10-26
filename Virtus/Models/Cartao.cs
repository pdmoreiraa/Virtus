namespace Virtus.Models
{
    public class Cartao
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int MetodoPagamentoId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string NomeTitular { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Bandeira { get; set; } = string.Empty;
        public int ValidadeMes { get; set; }
        public int ValidadeAno { get; set; }
        public string CVV { get; set; } = string.Empty;

        // Navegação
        public Usuario? Usuario { get; set; }
        public MetodoPagamento? MetodoPagamento { get; set; }
    }
}
