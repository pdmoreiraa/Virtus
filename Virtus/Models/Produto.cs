using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Virtus.Models
{
    public class Produto
    {
        public int PrdId { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [StringLength(100)]
        public string PrdNome { get; set; }

        [Required(ErrorMessage = "A marca do produto é obrigatória.")]
        [StringLength(50)]
        public string PrdMarca { get; set; }

        [Required(ErrorMessage = "A categoria do produto é obrigatória.")]
        [StringLength(70)]
        public string PrdCategoria { get; set; }

        [Required(ErrorMessage = "O tipo do produto é obrigatório.")]
        [StringLength(70)]
        public string PrdTipo { get; set; }

        [Required(ErrorMessage = "O esporte do produto é obrigatório.")]
        [StringLength(70)]
        public string PrdEsporte { get; set; }

        [Required(ErrorMessage = "A descrição do produto é obrigatória.")]
        public string PrdDescricao { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal PrdPreco { get; set; }

        [Required(ErrorMessage = "A cor é obrigatória.")]
        public string PrdCor { get; set; }

        [Display(Name = "Data de Criação")]
        [DataType(DataType.Date)]
        public DateTime PrdData { get; set; } = DateTime.Now;


        public List<ProdutoImagem> Imagens { get; set; } = new List<ProdutoImagem>();
        public List<Estoque> Estoques { get; set; } = new();
    }
}
