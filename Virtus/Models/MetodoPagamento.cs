namespace Virtus.Models
{
    public class MetodoPagamento
    {
        public int MpId { get; set; }
        public string MpDescricao { get; set; } = string.Empty;

        // Relacionamentos
        public List<Cartao>? Cartoes { get; set; }
        public List<Pedido>? Pedidos { get; set; }
    }
}
