using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Virtus.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A marca do produto é obrigatória.")]
        [StringLength(50)]
        public string Marca { get; set; }

        [Required(ErrorMessage = "A categoria do produto é obrigatória.")]
        [StringLength(70)]
        public string Categoria { get; set; }

        [Required(ErrorMessage = "O tipo do produto é obrigatório.")]
        [StringLength(70)]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "O esporte do produto é obrigatório.")]
        [StringLength(70)]
        public string Esporte { get; set; }

        [Required(ErrorMessage = "A descrição do produto é obrigatória.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Preco { get; set; }

        [StringLength(255)]
        [Display(Name = "URL da Imagem")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "A quantidade em estoque é obrigatória.")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade deve ser um número positivo.")]
        public int? Estoque { get; set; }

        [Display(Name = "Data de Criação")]
        [DataType(DataType.Date)]
        public DateTime DataCriada { get; set; } = DateTime.Now;
    }
}
