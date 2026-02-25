using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjAvaliacao.Models;
using ProjAvaliacao.ViewModels;

namespace ProjAvaliacao.Controllers
{
    // Controlador responsável por operações administrativas relacionadas a usuários.
    // Apenas acessível para usuários no papel "Admin".
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // Gerencia operações de usuários (criar, localizar, atualizar, excluir).
        private readonly UserManager<Usuario> _userManager;
        // Gerencia operações de papéis/roles (listar roles, criar roles etc.).
        private readonly RoleManager<IdentityRole> _roleManager;

        // Injeção de dependências: UserManager e RoleManager fornecidos pelo Identity.
        public AdminController(
            UserManager<Usuario> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Exibe a página de criação de usuário (GET).
        // Preenche ViewBag.Roles para popular um seletor de roles na view.
        [HttpGet]
        public IActionResult CriarUsuario()
        {
            // Lista todos os roles disponíveis para seleção na view.
            ViewBag.Roles = _roleManager.Roles.ToList();
            return View();
        }

        // Recebe os dados do formulário de criação de usuário (POST).
        // [ValidateAntiForgeryToken] protege contra CSRF.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarUsuario(CreateUsuarioViewModel vm)
        {
            // Validação do model enviado pelo formulário.
            if (!ModelState.IsValid)
            {
                // Recarrega os roles caso haja erro para reexibir o formulário corretamente.
                ViewBag.Roles = _roleManager.Roles.ToList();
                return View(vm);
            }

            // Mapeia ViewModel para a entidade Usuario.
            var usuario = new Usuario
            {
                UserName = vm.Email,
                Email = vm.Email,
                Nome = vm.Nome,
                Sobrenome = vm.Sobrenome
            };

            // Cria o usuário juntamente com a senha (Identity valida força da senha).
            var result = await _userManager.CreateAsync(usuario, vm.Senha);

            // Se ocorreram erros na criação, adiciona mensagens ao ModelState para exibir na view.
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                // Recarrega os roles antes de retornar a View com erros.
                ViewBag.Roles = _roleManager.Roles.ToList();
                return View(vm);
            }

            // Se a criação foi bem-sucedida, associa o usuário ao role selecionado.
            // Nota: seria prudente verificar se o role existe antes de adicionar.
            await _userManager.AddToRoleAsync(usuario, vm.Role);

            // Redireciona para a home após sucesso.
            return RedirectToAction("Index", "Home");
        }

        // Exclui um usuário por id (POST).
        // Recebe apenas o id e retorna redirect ou erro.
        [HttpPost]
        public async Task<IActionResult> ExcluirUsuario(string id)
        {
            // Localiza o usuário pelo id.
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return NotFound(); // 404 quando usuário não existe.

            // Solicita remoção do usuário.
            var result = await _userManager.DeleteAsync(usuario);
            if (!result.Succeeded)
            {
                // Em caso de falha na exclusão, retorna BadRequest com detalhes.
                // Em produção, considere logar os erros e apresentar mensagem amigável.
                return BadRequest(result.Errors);
            }

            // Redireciona após exclusão bem-sucedida.
            return RedirectToAction("Index", "Home");
        }

        // Exibe o formulário de edição do usuário (GET).
        // Recebe o id do usuário, monta um EditUsuarioViewModel e retorna a View.
        [HttpGet]
        public async Task<IActionResult> EditarUsuario(string id)
        {
            // Localiza o usuário.
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return NotFound();

            // Obtém roles atuais do usuário (pode ser zero ou mais).
            var roles = await _userManager.GetRolesAsync(usuario);
            var roleAtual = roles.FirstOrDefault();

            var vm = new EditUsuarioViewModel
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Sobrenome = usuario.Sobrenome,
                Email = usuario.Email,
                Role = roleAtual
            };

            // Fornece lista de roles para o select na view.
            ViewBag.Roles = _roleManager.Roles.ToList();
            return View(vm);
        }

        // Recebe o POST do formulário de edição e persiste as alterações.
        // Usa ValidateAntiForgeryToken para proteção contra CSRF.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario(EditUsuarioViewModel vm)
        {
            // Validação do viewmodel.
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _roleManager.Roles.ToList();
                return View(vm);
            }

            // Localiza o usuário no banco.
            var usuario = await _userManager.FindByIdAsync(vm.Id);
            if (usuario == null)
                return NotFound();

            // Atualiza campos permitidos.
            usuario.Nome = vm.Nome;
            usuario.Sobrenome = vm.Sobrenome;
            usuario.Email = vm.Email;
            usuario.UserName = vm.Email; // Sincroniza UserName com Email.

            // Persiste alterações de dados do usuário.
            var updateResult = await _userManager.UpdateAsync(usuario);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError("", error.Description);

                ViewBag.Roles = _roleManager.Roles.ToList();
                return View(vm);
            }

            // Sincroniza roles: substitui roles atuais pela role selecionada no VM.
            var currentRoles = await _userManager.GetRolesAsync(usuario);
            var selectedRole = vm.Role;

            // Se uma role foi selecionada e existe no sistema, atualiza associação.
            if (!string.IsNullOrWhiteSpace(selectedRole))
            {
                // Verifica se a role selecionada existe no RoleManager.
                var roleExists = await _roleManager.RoleExistsAsync(selectedRole);
                if (!roleExists)
                {
                    ModelState.AddModelError("", $"Role '{selectedRole}' não existe.");
                    ViewBag.Roles = _roleManager.Roles.ToList();
                    return View(vm);
                }

                // Remove roles atuais (se houver) que são diferentes da selecionada.
                if (currentRoles != null && currentRoles.Count > 0)
                {
                    var rolesToRemove = currentRoles.Where(r => r != selectedRole).ToList();
                    if (rolesToRemove.Count > 0)
                    {
                        var removeResult = await _userManager.RemoveFromRolesAsync(usuario, rolesToRemove);
                        if (!removeResult.Succeeded)
                        {
                            foreach (var error in removeResult.Errors)
                                ModelState.AddModelError("", error.Description);

                            ViewBag.Roles = _roleManager.Roles.ToList();
                            return View(vm);
                        }
                    }
                }

                // Adiciona o usuário à role selecionada caso ainda não esteja.
                if (!currentRoles.Contains(selectedRole))
                {
                    var addResult = await _userManager.AddToRoleAsync(usuario, selectedRole);
                    if (!addResult.Succeeded)
                    {
                        foreach (var error in addResult.Errors)
                            ModelState.AddModelError("", error.Description);

                        ViewBag.Roles = _roleManager.Roles.ToList();
                        return View(vm);
                    }
                }
            }

            // Tudo ok: redireciona para a home (ou página de listagem desejada).
            return RedirectToAction("Index", "Home");
        }

    }
}