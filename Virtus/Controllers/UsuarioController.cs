using Microsoft.AspNetCore.Authorization;
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


                HttpContext.Session.SetString("UsuarioId", usuarioCriado.UsuId.ToString());
                HttpContext.Session.SetString("UsuarioNome", usuarioCriado.UsuNome);
                HttpContext.Session.SetString("UsuarioSobrenome", usuarioCriado.UsuSobrenome);
                HttpContext.Session.SetString("UsuarioEmail", usuarioCriado.UsuEmail);
                HttpContext.Session.SetString("UsuarioTipo", usuarioCriado.UsuTipo);

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

        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            // Busca o usuário no banco pelo email e senha
            var usuarioL = await _usuarioRepository.ObterPorEmailESenha(login.Email, login.Senha);

            if (usuarioL != null)
            {
                HttpContext.Session.SetString("UsuarioId", usuarioL.UsuId.ToString());
                HttpContext.Session.SetString("UsuarioNome", usuarioL.UsuNome);
                HttpContext.Session.SetString("UsuarioSobrenome", usuarioL.UsuSobrenome);
                HttpContext.Session.SetString("UsuarioEmail", usuarioL.UsuEmail);
                HttpContext.Session.SetString("UsuarioTipo", usuarioL.UsuTipo);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Email ou senha inválidos.";
            return View(login);
        }

        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            var usuarioIdString = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdString))
                return RedirectToAction("Login", "Usuario");

            int usuarioId = int.Parse(usuarioIdString);

            var usuario = await _usuarioRepository.ObterPorId(usuarioId);
            if (usuario == null)
                return RedirectToAction("Index", "Home");

            var perfil = new Perfil
            {
                Nome = usuario.UsuNome,
                Sobrenome = usuario.UsuSobrenome,
                Email = usuario.UsuEmail,
                Telefone = usuario.UsuTelefone,
                CPF = usuario.UsuCPF,
                Tipo = usuario.UsuTipo
            };

            return View(perfil);
        }


        // POST: Perfil
        [HttpPost]
        public async Task<IActionResult> Perfil(Perfil perfil)
        {
            var usuarioIdString = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdString))
                return RedirectToAction("Login", "Usuario");

            int usuarioId = int.Parse(usuarioIdString);
            var usuarioL = await _usuarioRepository.ObterPorId(usuarioId);
            if (usuarioL == null)
                return RedirectToAction("Index", "Home");

            // Remove máscara antes da validação
            perfil.CPF = new string(perfil.CPF.Where(char.IsDigit).ToArray());
            perfil.Telefone = new string(perfil.Telefone.Where(char.IsDigit).ToArray());

            // Limpa ModelState antigo para evitar erros anteriores
            ModelState.Clear();

            // Revalida os campos
            TryValidateModel(perfil);

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ViewBag.ErrorMessage = string.Join("<br/>", erros);
                return View(perfil);
            }


            // Atualiza campos permitidos
            usuarioL.UsuNome = perfil.Nome;
            usuarioL.UsuSobrenome = perfil.Sobrenome;
            usuarioL.UsuEmail = perfil.Email;
            usuarioL.UsuTelefone = perfil.Telefone;
            usuarioL.UsuCPF = perfil.CPF;

            var sucesso = await _usuarioRepository.AtualizarPerfil(usuarioL);

            ModelState.Clear(); // limpa erros antigos

            if (sucesso)
                ViewBag.SuccessMessage = "Perfil atualizado com sucesso!";
            else
                ViewBag.ErrorMessage = "Erro ao atualizar o perfil. Tente novamente.";

            return View(perfil);
        }

        public IActionResult Senha()
        {
            return View(new SenhaNova());
        }

        [HttpPost]
        public async Task<IActionResult> Senha(SenhaNova senhaNova)
        {
            var usuarioIdString = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdString))
                return RedirectToAction("Login", "Usuario");

            int usuarioId = int.Parse(usuarioIdString);
            var usuarioL = await _usuarioRepository.ObterPorId(usuarioId);
            if (usuarioL == null)
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(senhaNova);

            // Verifica senha atual
            if (usuarioL.UsuSenha != senhaNova.SenhaAtual)
            {
                ModelState.AddModelError("", "A senha atual está incorreta.");
                return View(senhaNova);
            }

            // Atualiza senha
            usuarioL.UsuSenha = senhaNova.NovaSenha;
            var sucesso = await _usuarioRepository.AtualizarSenha(usuarioL);

            if (sucesso)
            {
                return RedirectToAction("Perfil", "Usuario");
            }
            else
            {
                ViewBag.ErrorMessage = "Erro ao alterar a senha.";

                return View(senhaNova);
            }
        }

    }
}
