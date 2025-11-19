using System.ComponentModel.DataAnnotations;

namespace Virtus.Models
{
    public class Cartao
    {
        public int CarId { get; set; }
        public int UsuarioId { get; set; }
        public int MetodoPagamentoId { get; set; }
        public string CarTipo { get; set; } = string.Empty;
        public string CarNomeTitular { get; set; } = string.Empty;
        public string CarNumero { get; set; } = string.Empty;
        public string CarBandeira { get; set; } = string.Empty;
        public string CarValidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CVV é obrigatório.")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "O CVV deve ter 3 dígitos numéricos.")]
        public string CarCVV { get; set; } = string.Empty;

        // Navegação
        public Usuario? Usuario { get; set; }
    }
}
