namespace Virtus.Models
{
    public class Pedido
    {
        public int PdId { get; set; }
        public int UsuarioId { get; set; }
        public int EnderecoId { get; set; }
        public int? MetodoPagamentoId { get; set; }
        public int? CartaoId { get; set; }
        public decimal PdTaxaEntrega { get; set; }
        public string PdStatusPagamento { get; set; } = "Aguardando Pagamento";
        public string PdStatusPedido { get; set; } = "Criado";
        public decimal PdValorTotal { get; set; }
        public DateTime PdCriadoEm { get; set; }
        public DateTime PdDataPagamento { get; set; }
        public DateTime PdExpiracao { get; set; }

        // Navegação
        public Usuario? Usuario { get; set; }
        public Endereco? Endereco { get; set; }
        public MetodoPagamento? MetodoPagamento { get; set; }
        public Cartao? Cartao { get; set; }
        public List<ItemPedido>? Itens { get; set; }
        public List<ProdutoImagem> Imagens { get; set; } = new List<ProdutoImagem>();
        public List<Estoque> Estoques { get; set; } = new();
    }
}
