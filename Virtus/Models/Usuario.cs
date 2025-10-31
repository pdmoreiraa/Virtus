using System.ComponentModel.DataAnnotations;

namespace Virtus.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório."), MaxLength(100)]
        public string Nome { get; set; } = "";

        [Required(ErrorMessage = "O sobrenome é obrigatório."), MaxLength(100)]
        public string Sobrenome { get; set; } = "";

        [Required(ErrorMessage = "O email é obrigatório."), EmailAddress, MaxLength(100)]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "A senha é obrigatória."), MaxLength(100)]
        public string Senha { get; set; } = "";

        [Required(ErrorMessage = "Confirmar a senha é obrigatório.")]
        [Compare("Senha", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmarSenha { get; set; } = "";

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "O CPF deve conter 11 números.")]
        public string CPF { get; set; } = "";

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "O telefone deve conter 11 números (DDD + número).")]
        public string Telefone { get; set; } = "";

        public string Tipo { get; set; } = "cliente";

        public List<Endereco>? Enderecos { get; set; }
        public List<Cartao>? Cartoes { get; set; }
        public List<Pedido>? Pedidos { get; set; }
    }
}
