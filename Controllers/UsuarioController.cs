using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;
using PitStop_Parts_Inventario.Models;
using Microsoft.AspNetCore.Authorization;

namespace PitStop_Parts_Inventario.Controllers
{
    [Authorize]
    public class UsuarioController : BaseController
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public async Task<IActionResult> Index(UsuarioFilterOptions? filters = null)
        {
            return await ExecuteIfAdmin(async () =>
            {
                try
                {
                    filters ??= new UsuarioFilterOptions();
                    
                    var usuarios = await _usuarioService.GetPagedAsync(
                        filters.PageNumber, 
                        filters.PageSize, 
                        filters.SearchTerm);

                    ViewBag.Filters = filters;
                    return View(usuarios);
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al cargar usuarios: {ex.Message}";
                    return View(new PagedResult<UsuarioModel>(new List<UsuarioModel>(), 0, 1, 10));
                }
            });
        }

        public async Task<IActionResult> Create()
        {
            return await ExecuteIfAdmin(async () =>
            {
                var viewModel = new UsuarioViewModel
                {
                    Roles = await _usuarioService.GetAllRolesAsync(),
                    Estados = await _usuarioService.GetAllStatusAsync(),
                    IdEstado = 1, // Activo por defecto
                    FechaDeIngreso = DateTime.Now
                };

                return View(viewModel);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioViewModel viewModel)
        {
            return await ExecuteIfAdmin(async () =>
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(viewModel.Password))
                        {
                            ModelState.AddModelError("Password", "La contraseña es requerida para crear un usuario");
                            viewModel.Roles = await _usuarioService.GetAllRolesAsync();
                            viewModel.Estados = await _usuarioService.GetAllStatusAsync();
                            return View(viewModel);
                        }

                        var usuario = new UsuarioModel
                        {
                            UserName = viewModel.UserName,
                            Email = viewModel.Email,
                            IdRol = viewModel.IdRol,
                            IdEstado = viewModel.IdEstado,
                            FechaDeIngreso = DateTime.Now
                        };

                        await _usuarioService.CreateAsync(usuario, viewModel.Password);
                        TempData["Success"] = "Usuario creado exitosamente";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = $"Error al crear usuario: {ex.Message}";
                    }
                }

                viewModel.Roles = await _usuarioService.GetAllRolesAsync();
                viewModel.Estados = await _usuarioService.GetAllStatusAsync();
                return View(viewModel);
            });
        }

        public async Task<IActionResult> Edit(string id)
        {
            return await ExecuteIfAdmin(async () =>
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var usuario = await _usuarioService.GetByIdAsync(id);
                if (usuario == null)
                    return NotFound();

                var viewModel = new UsuarioViewModel
                {
                    Id = usuario.Id,
                    UserName = usuario.UserName ?? "",
                    Email = usuario.Email ?? "",
                    IdRol = usuario.IdRol,
                    IdEstado = usuario.IdEstado,
                    FechaDeIngreso = usuario.FechaDeIngreso,
                    Roles = await _usuarioService.GetAllRolesAsync(),
                    Estados = await _usuarioService.GetAllStatusAsync()
                };

                return View(viewModel);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuarioViewModel viewModel)
        {
            return await ExecuteIfAdmin(async () =>
            {
                // Remover validación de contraseña para edición
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
                
                if (ModelState.IsValid)
                {
                    try
                    {
                        var usuario = new UsuarioModel
                        {
                            Id = viewModel.Id,
                            UserName = viewModel.UserName,
                            Email = viewModel.Email,
                            IdRol = viewModel.IdRol,
                            IdEstado = viewModel.IdEstado
                        };

                        await _usuarioService.UpdateAsync(usuario);
                        TempData["Success"] = "Usuario actualizado exitosamente";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = $"Error al actualizar usuario: {ex.Message}";
                    }
                }
                else
                {
                    // Agregar información de depuración
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    TempData["Error"] = $"Errores de validación: {string.Join(", ", errors)}";
                }

                viewModel.Roles = await _usuarioService.GetAllRolesAsync();
                viewModel.Estados = await _usuarioService.GetAllStatusAsync();
                return View(viewModel);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            return await ExecuteIfAdmin(async () =>
            {
                try
                {
                    var result = await _usuarioService.DeleteAsync(id);
                    if (result)
                    {
                        TempData["Success"] = "Usuario desactivado exitosamente";
                    }
                    else
                    {
                        TempData["Error"] = "No se pudo desactivar el usuario";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al desactivar usuario: {ex.Message}";
                }

                return RedirectToAction(nameof(Index));
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(string userId, int statusId)
        {
            return await ExecuteIfAdmin(async () =>
            {
                try
                {
                    var result = await _usuarioService.UpdateStatusAsync(userId, statusId);
                    if (result)
                    {
                        return Json(new { success = true, message = "Estado actualizado exitosamente" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "No se pudo actualizar el estado" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Error: {ex.Message}" });
                }
            });
        }

        public async Task<IActionResult> ChangePassword(string id)
        {
            return await ExecuteIfAdmin(async () =>
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var usuario = await _usuarioService.GetByIdAsync(id);
                if (usuario == null)
                    return NotFound();

                var viewModel = new ChangePasswordViewModel
                {
                    UserId = usuario.Id,
                    UserName = usuario.UserName ?? ""
                };

                return View(viewModel);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            return await ExecuteIfAdmin(async () =>
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var result = await _usuarioService.ChangePasswordAsync(
                            viewModel.UserId, 
                            viewModel.NewPassword);

                        if (result)
                        {
                            TempData["Success"] = "Contraseña cambiada exitosamente";
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ModelState.AddModelError("", "No se pudo cambiar la contraseña. Intente nuevamente.");
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = $"Error al cambiar la contraseña: {ex.Message}";
                    }
                }

                return View(viewModel);
            });
        }
    }
}
