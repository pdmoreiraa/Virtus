namespace Virtus.Models
{
    public class ItemPedido
    {
        public int IpId { get; set; }
        public int PedidoId { get; set; }
        public int ProdutoId { get; set; }
        public int IpQuantidade { get; set; }
        public decimal IpPrecoUnitario { get; set; }

        // Navegação
        public Pedido? Pedido { get; set; }
        public Produto? Produto { get; set; }
        public ProdutoImagem? Imagem { get; set; }
    }
}
