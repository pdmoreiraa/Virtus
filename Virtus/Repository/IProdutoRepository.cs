using Virtus.Models;

namespace Virtus.Repository
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<Produto>> TodosProdutos();
        Task<IEnumerable<Produto>> ProdutosOrdenados();
        Task<Produto?> ProdutosPorId(int id);
        Task AdicionarProduto(Produto produto);
        Task AtualizarProduto(Produto produto);
        Task DeletarProduto(int id);
    }
}
