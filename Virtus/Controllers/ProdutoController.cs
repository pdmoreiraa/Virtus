using Microsoft.AspNetCore.Mvc;

namespace Virtus.Controllers
{
    public class ProdutoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
