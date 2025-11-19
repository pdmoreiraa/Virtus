using System.ComponentModel.DataAnnotations;

namespace Virtus.Models
{
    public class Usuario
    {
        public int UsuId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório."), MaxLength(100)]
        public string UsuNome { get; set; } = "";

        [Required(ErrorMessage = "O sobrenome é obrigatório."), MaxLength(100)]
        public string UsuSobrenome { get; set; } = "";

        [Required(ErrorMessage = "O email é obrigatório."), EmailAddress, MaxLength(100)]
        public string UsuEmail { get; set; } = "";

        [Required(ErrorMessage = "A senha é obrigatória."), MaxLength(100)]
        public string UsuSenha { get; set; } = "";

        [Required(ErrorMessage = "Confirmar a senha é obrigatório.")]
        [Compare("UsuSenha", ErrorMessage = "As senhas não coincidem")]
        public string UsuConfirmarSenha { get; set; } = "";

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        public string UsuCPF { get; set; } = "";

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        public string UsuTelefone { get; set; } = "";

        public string UsuTipo { get; set; } = "cliente";

        public List<Endereco>? Enderecos { get; set; }
        public List<Cartao>? Cartoes { get; set; }
        public List<Pedido>? Pedidos { get; set; }
    }
}
