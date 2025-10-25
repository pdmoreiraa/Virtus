using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mysqlx.Crud;
using Virtus.Models;
using Virtus.Repository;

namespace Virtus.Controllers
{
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class ProdutoController : Controller
    {

        private readonly ProdutoRepository _produtoRepository;
        private readonly int pagTam = 5;

        public ProdutoController(ProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }
        public async Task<IActionResult> Index(int pagIndex, string? buscar = null, string? coluna = "Id", string? ordPor = "desc")
        {
            var produtos = await _produtoRepository.ProdutosOrdenados();

            // Busca
            if (!string.IsNullOrEmpty(buscar))
            {
                produtos = produtos
                    .Where(p => p.Nome.Contains(buscar, StringComparison.OrdinalIgnoreCase)
                             || p.Marca.Contains(buscar, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                pagIndex = 1; // reset para página 1
            }

            // Normaliza coluna
            coluna ??= "Id";
            ordPor ??= "desc";

            // Ordenação
            produtos = coluna switch
            {
                "Nome" => ordPor == "asc" ? produtos.OrderBy(p => p.Nome).ToList() : produtos.OrderByDescending(p => p.Nome).ToList(),
                "Marca" => ordPor == "asc" ? produtos.OrderBy(p => p.Marca).ToList() : produtos.OrderByDescending(p => p.Marca).ToList(),
                "Categoria" => ordPor == "asc" ? produtos.OrderBy(p => p.Categoria).ToList() : produtos.OrderByDescending(p => p.Categoria).ToList(),
                "Tipo" => ordPor == "asc" ? produtos.OrderBy(p => p.Tipo).ToList() : produtos.OrderByDescending(p => p.Tipo).ToList(),
                "Preco" => ordPor == "asc" ? produtos.OrderBy(p => p.Preco).ToList() : produtos.OrderByDescending(p => p.Preco).ToList(),
                "Estoque" => ordPor == "asc" ? produtos.OrderBy(p => p.Estoque).ToList() : produtos.OrderByDescending(p => p.Estoque).ToList(),
                "DataCriada" => ordPor == "asc" ? produtos.OrderBy(p => p.DataCriada).ToList() : produtos.OrderByDescending(p => p.DataCriada).ToList(),
                _ => ordPor == "asc" ? produtos.OrderBy(p => p.Id).ToList() : produtos.OrderByDescending(p => p.Id).ToList(),
            };

            if (pagIndex < 1) pagIndex = 1;

            // Total de itens e páginas
            int count = produtos.Count();
            int totalPag = (int)Math.Ceiling((decimal)count / pagTam);

            // Garantir que a página atual não ultrapasse o total de páginas (ou seja 1 se não houver registros)
            if (totalPag == 0) totalPag = 1;
            if (pagIndex > totalPag) pagIndex = totalPag;

            // Pegar produtos da página atual
            var produtosPaginados = produtos
                .Skip((pagIndex - 1) * pagTam)
                .Take(pagTam)
                .ToList();


            // Dados para a view
            ViewData["PagIndex"] = pagIndex;
            ViewData["TotalPag"] = totalPag;

            ViewData["Buscar"] = buscar ?? "";

            ViewData["Coluna"] = coluna;
            ViewData["OrdPor"] = ordPor;

            return View(produtosPaginados);
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

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var produto = await _produtoRepository.ProdutosPorId(id);
            if (produto == null)
            {
                return RedirectToAction("Index", "Produto");
            }

            return View(produto);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Produto produto, IFormFile imagemArquivo)
        {
            var produtos = await _produtoRepository.ProdutosPorId(produto.Id);

            if (produtos == null)
            {
                return RedirectToAction("Index", "Produto");
            }

            var caminhoPasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");

            if (imagemArquivo != null && imagemArquivo.Length > 0)
            {
                // Apagar imagem antiga
                if (!string.IsNullOrEmpty(produtos.ImageUrl))
                {
                    var caminhoAntigo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", produtos.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(caminhoAntigo))
                    {
                        System.IO.File.Delete(caminhoAntigo);
                    }
                }

                // Criar pasta se necessário
                if (!Directory.Exists(caminhoPasta))
                    Directory.CreateDirectory(caminhoPasta);

                // Salvar nova imagem
                var nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(imagemArquivo.FileName);
                var caminhoCompleto = Path.Combine(caminhoPasta, nomeArquivo);

                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await imagemArquivo.CopyToAsync(stream);
                }

                produto.ImageUrl = "/img/" + nomeArquivo;
            }
            else
            {
                produto.ImageUrl = produtos.ImageUrl;
            }

            await _produtoRepository.AtualizarProduto(produto);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Deletar(int id)
        {
            var produto = await _produtoRepository.ProdutosPorId(id);
            if (produto == null)
            {
                return RedirectToAction("Index");
            }

            // Apaga imagem associada, se existir
            if (!string.IsNullOrEmpty(produto.ImageUrl))
            {
                var caminhoImagem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", produto.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(caminhoImagem))
                {
                    System.IO.File.Delete(caminhoImagem);
                }
            }

            await _produtoRepository.DeletarProduto(id);

            return RedirectToAction("Index");
        }
    }
}
