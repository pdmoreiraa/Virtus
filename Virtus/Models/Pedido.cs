namespace Virtus.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int EnderecoId { get; set; }
        public int? MetodoPagamentoId { get; set; }
        public int? CartaoId { get; set; }
        public decimal TaxaEntrega { get; set; }
        public string StatusPedido { get; set; } = "Pendente";
        public DateTime CriadoEm { get; set; }

        // Navegação
        public Usuario? Usuario { get; set; }
        public Endereco? Endereco { get; set; }
        public MetodoPagamento? MetodoPagamento { get; set; }
        public Cartao? Cartao { get; set; }
        public List<ItemPedido>? Itens { get; set; }
    }
}
