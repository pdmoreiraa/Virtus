using Microsoft.AspNetCore.Mvc;
using Virtus.Repository;

namespace Virtus.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UsuarioRepository _usuarioRepository;

        public UsuarioController(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public IActionResult Registrar()
        {
            return View();
        }
    }
}
