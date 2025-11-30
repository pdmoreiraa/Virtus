using System.Threading.Tasks;
using Virtus.Models;

namespace Virtus.Repository
{
    public interface IProdutoRepository
    {
        Task<List<Produto>> TodosProdutos();
        Task<List<Produto>> ProdutosOrdenados();
        Task<Produto?> ProdutosPorId(int id);
        Task<int> AdicionarProduto(Produto produto);
        Task AtualizarProduto(Produto produto);
        Task DeletarProduto(int id);
        Task DeletarImagem(int imagemId);
        Task<ProdutoImagem?> ImagemPorId(int id);

    }
}
