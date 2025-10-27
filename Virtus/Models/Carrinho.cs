namespace Virtus.Models
{
    public class Carrinho
    {
        public List<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
        public List<Endereco> Enderecos { get; set; } = new List<Endereco>();
        public List<Cartao> Cartoes { get; set; } = new List<Cartao>();

        public Endereco NovoEndereco { get; set; }
        public Cartao NovoCartao { get; set; }

        public decimal Subtotal { get; set; }
        public decimal TaxaEntrega { get; set; }
        public decimal Total => Subtotal + TaxaEntrega;
        public decimal Pix => (Subtotal + TaxaEntrega) * 0.95m;
    }
}
