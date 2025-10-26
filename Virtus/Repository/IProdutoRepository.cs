using Virtus.Models;

namespace Virtus.Repository
{
    public interface IProdutoRepository
    {
        Produto ProdutosPorId(int id);
    }
}
