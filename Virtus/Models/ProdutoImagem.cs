namespace Virtus.Models
{
    public class ProdutoImagem
    {
        public int PimgId { get; set; }
        public int ProdutoId { get; set; }
        public string PimgUrl { get; set; } = string.Empty;
        public int PimgOrdemImagem { get; set; }
    }
}
