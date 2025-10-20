using Microsoft.AspNetCore.Mvc;
using Virtus.Models;
using Virtus.Repository;

namespace Virtus.Controllers
{
    public class ProdutoController : Controller
    {

        private readonly ProdutoRepository _produtoRepository;

        public ProdutoController(ProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }
        public async Task<IActionResult> Index()
        {
            var produtos = await _produtoRepository.ProdutosOrdenados();

            return View(produtos);
        }

        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Criar(Produto produto, IFormFile imagemArquivo)
        {
            if (!ModelState.IsValid)
                return View(produto);

            if (imagemArquivo == null || imagemArquivo.Length == 0)
                ModelState.AddModelError("ImageUrl", "A imagem é obrigatória.");


            if (imagemArquivo != null && imagemArquivo.Length > 0)
            {
                // Define o caminho para salvar
                var caminhoPasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");

                // Garante que a pasta exista
                if (!Directory.Exists(caminhoPasta))
                    Directory.CreateDirectory(caminhoPasta);

                // Gera um nome único para o arquivo
                var nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(imagemArquivo.FileName);
                var caminhoCompleto = Path.Combine(caminhoPasta, nomeArquivo);

                // Salva o arquivo
                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await imagemArquivo.CopyToAsync(stream);
                }

                // Atualiza o campo ImageUrl com o caminho público
                produto.ImageUrl = "/img/" + nomeArquivo;
            }

            // Chama o método de repositório para salvar no banco
            await _produtoRepository.AdicionarProduto(produto);

            return RedirectToAction("Index");
        }
    }
}
