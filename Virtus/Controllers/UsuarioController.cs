using Microsoft.AspNetCore.Mvc;
using Virtus.Models;
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

        [HttpPost]
        public async Task<IActionResult> Registrar(Usuario usuario)
        {
            if (!ModelState.IsValid)
                return View(usuario);

            try
            {
                var usuarioCriado = await _usuarioRepository.RegistrarUsuario(usuario);

                if (usuarioCriado == null)
                {
                    ModelState.AddModelError("Email", "Já existe um usuário com esse e-mail ou CPF.");
                    return View(usuario);
                }


                HttpContext.Session.SetString("UsuarioId", usuarioCriado.Id.ToString());
                HttpContext.Session.SetString("UsuarioNome", usuarioCriado.Nome);
                HttpContext.Session.SetString("UsuarioSobrenome", usuarioCriado.Sobrenome);
                HttpContext.Session.SetString("UsuarioEmail", usuarioCriado.Email);
                HttpContext.Session.SetString("UsuarioTipo", usuarioCriado.Tipo);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erro ao registrar usuário: {ex.Message}");
                return View(usuario);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

    }
}
